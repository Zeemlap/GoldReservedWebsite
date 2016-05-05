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
using System.Threading.Tasks;

namespace GoldReserves.Web
{
    public class DatabaseInitializer : IDatabaseInitializer<GoldReservesDbContext>
    {

    
        public void InitializeDatabase(GoldReservesDbContext context)
        {
            bool exists;
            if (exists = context.Database.Exists())
            {
                if (context.Database.CompatibleWithModel(false))
                {
                    return;
                }
                if (context.Database.Delete())
                {
                    exists = false;
                }

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
                AddPoliticalEntityAlias(context, "Czech Rep.", "Czech Republic");
                AddPoliticalEntityAlias(context, "Brunei", "Brunei Darussalam");
                AddPoliticalEntityAlias(context, "Kyrgyzstan", "Kyrgyz Republic");
                AddPoliticalEntityAlias(context, "Bosnia and Herz.", "Bosnia and Herzegovina");
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