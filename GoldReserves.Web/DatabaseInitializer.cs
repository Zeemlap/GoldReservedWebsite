using GoldReserves.Data;
using System.Data.Entity;
using System;
using System.Web.Hosting;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using Newtonsoft.Json;
using System.Dynamic;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using System.Configuration;

namespace GoldReserves.Web
{
    public class DatabaseInitializer : IDatabaseInitializer<GoldReservesDbContext>
    {


        public void InitializeDatabase(GoldReservesDbContext context)
        {
            bool exists;
            if (exists = context.Database.Exists())
            {
                bool recreateAlways = false;
#if DEBUG
                string recreateAlwaysSetting = ConfigurationManager.AppSettings["GoldReservesDbRecreateAlways"];
                if (recreateAlwaysSetting == null || !bool.TryParse(recreateAlwaysSetting, out recreateAlways))
                {
                    recreateAlways = false;
                }
#endif
                if (!recreateAlways && context.Database.CompatibleWithModel(false))
                {
                    return;
                }
                context.Database.Delete();
                exists = false;
            }
            if (!exists)    
            {
                context.Database.Create();
                context.InitializeAndSeed();
            }
            bool ex = true;
            try
            {
                var path = HostingEnvironment.MapPath("~/Content/geoRegionsTopoJson.json");
                using (var fileStream = new FileStream(
                    path,
                    FileMode.Open,
                    FileSystemRights.ReadData,
                    FileShare.Read,
                    4096,
                    FileOptions.SequentialScan))
                using (var textReader = new StreamReader(fileStream, Encoding.UTF8, false, 1024, true))
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    var jsonSer = new JsonSerializer();
                    jsonSer.Converters.Add(new Newtonsoft.Json.Converters.ExpandoObjectConverter());
                    dynamic topology = jsonSer.Deserialize<ExpandoObject>(jsonReader);
                    var geometries_d = topology.objects.units.geometries as List<object>;
                    foreach (dynamic geometry_d in geometries_d)
                    {
                        var g = new GeoRegion();
                        g.Id_Alpha3 = geometry_d.id;
                        string geometry_d_props_name = geometry_d.properties.name;
                        var p = context.GetPoliticalEntity(geometry_d_props_name);
                        if (p != null) throw new NotImplementedException();
                        p = new PoliticalEntity();
                        p.GeoRegion = g;
                        context.GeoRegions.Add(g);
                        context.PoliticalEntities.Add(p);
                        var pen = new PoliticalEntityName();
                        pen.LanguageId = GoldReservesDbContext.LanguageId_English;
                        pen.PoliticalEntity = p;
                        pen.Name = geometry_d_props_name;
                        context.PoliticalEntityNames.Add(pen);
                        context.SaveChanges();
                        context.Entry(g).State = EntityState.Detached;
                        context.Entry(p).State = EntityState.Detached;
                        context.Entry(pen).State = EntityState.Detached;
                    }
                }

                ImportPoliticalEntities(context);
                ImportPoliticalEntityAliases(context);
                ex = false;
            }
            finally
            {
                if (ex)
                {
                    try
                    {
                        context.Database.Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void ImportPoliticalEntities(GoldReservesDbContext context)
        {
            var filePath = HostingEnvironment.MapPath("~/Content/PoliticalEntities.txt");
            var re_lineTerminator = new Regex("\\r\\n?|\\n", RegexOptions.CultureInvariant);
            List<string> list;
            using (var fs = new FileStream(filePath, FileMode.Open, FileSystemRights.ReadData, FileShare.Read, 4096, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8, true, 1024, true))
            {
                list = re_lineTerminator
                    .Split(sr.ReadToEnd())
                    .ToList();
            }
            foreach (var item in list)
            {
                AddPoliticalEntity(context, item);
            }
        }

        private void ImportPoliticalEntityAliases(GoldReservesDbContext context)
        {

            var filePath = HostingEnvironment.MapPath("~/Content/PoliticalEntityAliases.txt");
            var re_lineTerminator = new Regex("\\r\\n?|\\n", RegexOptions.CultureInvariant);
            var re_manyTabs = new Regex("\\t+", RegexOptions.CultureInvariant);
            List<Tuple<string, string>> list;
            using (var fs = new FileStream(filePath, FileMode.Open, FileSystemRights.ReadData, FileShare.Read, 4096, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8, true, 1024, true))
            {
                list = re_lineTerminator
                    .Split(sr.ReadToEnd())
                    .Select(s =>
                    {
                        var sa = re_manyTabs.Split(s);
                        return new Tuple<string, string>(sa[0], sa[1]);
                    })
                    .ToList();
            }
            foreach (var item in list)
            {
                AddPoliticalEntityAlias(context, item.Item1, item.Item2);
            }
        }

        private void AddPoliticalEntity(GoldReservesDbContext context, string name)
        {
            var p = context.GetPoliticalEntity(name);
            if (p != null) throw new ArgumentException();
            p = new PoliticalEntity();
            var pen = new PoliticalEntityName();
            pen.LanguageId = GoldReservesDbContext.LanguageId_English;
            pen.PoliticalEntity = p;
            pen.Name = name;
            context.PoliticalEntities.Add(p);
            context.PoliticalEntityNames.Add(pen);
            context.SaveChanges();
            context.Entry(p).State = EntityState.Detached;
            context.Entry(pen).State = EntityState.Detached;
        }

        private void AddPoliticalEntityAlias(GoldReservesDbContext context, string pen_name, string pen_name_alias)
        {
            var p = context.GetPoliticalEntity(pen_name);
            if (p == null) throw new ArgumentException();
            var pen_alias = new PoliticalEntityName()
            {
                LanguageId = GoldReservesDbContext.LanguageId_English,
                Name = pen_name_alias,
                PoliticalEntityId = p.Id,
            };
            context.PoliticalEntityNames.Add(pen_alias);
            context.SaveChanges();
            context.Entry(pen_alias).State = EntityState.Detached;
        }
    }
}