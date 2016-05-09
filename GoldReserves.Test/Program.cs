using GoldReserves.Backend;
using GoldReserves.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

            var bla = TypeDescriptor.GetConverter(typeof(DateTime));


            //var rl1 = Enumerable.Empty<WorldOfficialGoldHoldingReport_Raw>();
            var rl1 = new Bla().GetWorldGoldReservesReports_UpTo2000();
            var rl2 = Enumerable.Empty<WorldOfficialGoldHoldingReport_Raw>();
            //var rl2 = new Bla().GetWorldGoldReservesReports_2000To2015();

            var connStrSettings = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Single(x => x.Name == "GoldReserves");
            var connStr = connStrSettings.ConnectionString; 
            List<string> names1;
            using (var db = new GoldReservesDbContext(connStr))
            {
                names1 = (from pen in db.PoliticalEntityNames
                          select pen).ToList().Select(pen => pen.Name).ToList();
            }
            var names2 = new SortedSet<string>(rl1.SelectMany(r => r.Rows).Concat(rl2.SelectMany(r => r.Rows)).Select(r => r.Name).Distinct());
            names2.ExceptWith(names1);
            StringBuilder sb = new StringBuilder();
            foreach (var name in names2)
            {
                sb.AppendLine(name);
            }
            File.WriteAllText("bla.txt", sb.ToString());
            Console.Read();
        }
    }
}
