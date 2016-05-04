using GoldReserves.Backend;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace GoldReserves.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var bla1 = new Bla().GetWorldGoldReservesReportAsync().Result;
            var rowNames = bla1.Rows.Select(e => e.Name);
            
            var assem = typeof(Program).Assembly;
            var rna = assem.GetManifestResourceNames();
            var dict = new Dictionary<string, string>();
            using (var stream = assem.GetManifestResourceStream(rna.Single(s => s.EndsWith(".topojson"))))
            using (var textReader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var jsonSer = new JsonSerializer();
                jsonSer.Converters.Add(new Newtonsoft.Json.Converters.ExpandoObjectConverter());
                dynamic topology = jsonSer.Deserialize<ExpandoObject>(jsonReader);
                var geometries_d = topology.objects.units.geometries as List<object>;
                foreach(dynamic geometry_d in geometries_d )
                {
                    dict.Add(geometry_d.id, geometry_d.properties.name);
                }
            }
        }
    }
}
