using GoldReserves.Backend;
using GoldReserves.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.WorldOfficialGoldHoldingImporter
{
    class Program
    {
        static void Main(string[] args)
        {

            var connStrSetting = ConfigurationManager.ConnectionStrings["GoldReserves"];
            var connStr = connStrSetting.ConnectionString;
            var reports_raw = new Bla().GetWorldGoldReservesReports_2000To2015();
            reports_raw.Add(new Bla().GetWorldGoldReservesReportAsync_Latest().Result);
            using (var db = new GoldReservesDbContext(connStr))
            {
                foreach(var report_raw in reports_raw)
                {
                    var report = new WorldOfficialGoldHoldingReport();
                    report.DataTimePoint = report_raw.DataTimePoint;
                    report.PublishTimePoint = report_raw.PublishTimePoint;
                    db.WorldOfficialGoldHoldingReports.Add(report);
                    db.SaveChanges();
                    db.Entry(report).State = EntityState.Detached;
                    foreach (var reportRow_raw in report_raw.Rows)
                    {
                        var row = new WorldOfficialGoldHoldingReportRow();
                        row.ReportDataTimePoint = report.DataTimePoint;
                        row.Tons = reportRow_raw.Tons;

                        var politicalEntity = db.GetPoliticalEntity(reportRow_raw.Name);
                        if (politicalEntity == null)
                        {
                        }
                        row.PoliticalEntityId = politicalEntity.Id;
                        db.WorldOfficialGoldHoldingReportRows.Add(row);
                        db.SaveChanges();
                        db.Entry(row).State = EntityState.Detached;
                    }
                }
            }
            
        }
    }
}
