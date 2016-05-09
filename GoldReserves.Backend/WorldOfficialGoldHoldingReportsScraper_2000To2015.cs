using System;
using System.Collections.Generic;
using System.Linq;
using GoldReserves.Data;
using Com.Jab.LibOffice.Workbooks;
using Com.Jab.LibCore;
using System.Text.RegularExpressions;
using System.Globalization;

namespace GoldReserves.Backend
{
    internal class WorldOfficialGoldHoldingReportsScraper_2000To2015
    {
        private static Regex s_regexYearQuarter = new Regex("^Q([1-4])\\s+(20(?:1[0-5]|0[0-9]))$", RegexOptions.CultureInvariant | RegexOptions.Compiled);  

        internal List<WorldOfficialGoldHoldingReport_Raw> Run()
        {
            var t = typeof(WorldOfficialGoldHoldingReportsScraper_2000To2015);
            Workbook wb;
            using (var stream = t.Assembly.GetManifestResourceStream(t, "quarterly_gold_and_fx_reserves_2000_2015.xlsx"))
            {
                wb = Workbook.LoadExcel(stream);
            }

            var ws2 = wb.Worksheets.Single(ws1 => 
                0 <= ws1.Name.IndexOf("gold", StringComparison.OrdinalIgnoreCase) 
                && 0 <= ws1.Name.IndexOf("ton", StringComparison.OrdinalIgnoreCase));

            int i = 0;
            int _2000Q1ColIdx = -1;
            while (true)
            {
                _2000Q1ColIdx = ws2.Rows[i].Cells_NonEmpty.FindIndex(c => c.ValueUnformatted != null && s_regexYearQuarter.IsMatch(c.ValueUnformatted));
                if (0 <= _2000Q1ColIdx) break;
                i += 1;
            }
            int firstRowIdx = i + 1;
            List<WorldOfficialGoldHoldingReport_Raw> reports = new List<WorldOfficialGoldHoldingReport_Raw>();
            for (i = 0; i < 4 * 16; i++)
            {
                var m = s_regexYearQuarter.Match(ws2.Rows[firstRowIdx - 1].Cells_NonEmpty[i + _2000Q1ColIdx].ValueUnformatted);
                bool flag = false;
                if (m.Success)
                {
                    var g1c0 = m.Groups[1].Captures.Cast<Capture>().Single().Value;
                    int i1;
                    if (int.TryParse(g1c0, NumberStyles.None, NumberFormatInfo.InvariantInfo, out i1) && i1 - 1 == (i & 3))
                    {
                        var g2c0 = m.Groups[2].Captures.Cast<Capture>().Single().Value;
                        int i2;
                        if (int.TryParse(g2c0, NumberStyles.None, NumberFormatInfo.InvariantInfo, out i2) && i2 == (i >> 2) + 2000)
                        {
                            flag = true;
                        }
                    }
                }
                if (!flag)
                {
                    throw new Exception();
                }
                reports.Add(new WorldOfficialGoldHoldingReport_Raw());
                reports[i].PublishTimePoint = new DateTime(2016, 3, 11, 0, 0, 0, DateTimeKind.Unspecified);
                reports[i].DataTimePoint = new DateTime(2000 + (i >> 2), 1 + (i & 3) * 3, 1, 0, 0, 0, DateTimeKind.Unspecified);
                reports[i].Rows = new List<WorldOfficialGoldHoldingReportRow_Raw>();
            }
            for (i = firstRowIdx; ;)
            {
                string politicalEntityName = ws2.Rows[i].Cells_NonEmpty[0].ValueUnformatted;
                if (new Regex("\\d+\\)$").IsMatch(politicalEntityName))
                {
                    throw new ArgumentException();
                }
                for (int j = 0; j < 4 * 16; j++)
                {
                    var ronsStr = ws2.Rows[i].Cells_NonEmpty[j + _2000Q1ColIdx].ValueUnformatted;
                    var row = new WorldOfficialGoldHoldingReportRow_Raw()
                    {
                        Name = politicalEntityName,
                    };
                    if (ronsStr != "-")
                    {
                        row.Tons = decimal.Parse(ronsStr, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
                    }
                    reports[j].Rows.Add(row);
                }

                i += 1;
                if (ws2.Rows.Count <= i
                    || ws2.Rows[i].Cells_NonEmpty.Count < 4 * 16 + _2000Q1ColIdx) break;
            }

            return reports;
        }
        
    }
}
