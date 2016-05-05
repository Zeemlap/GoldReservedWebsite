using Com.Jab.LibOffice.Workbooks;
using GoldReserves.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    internal class WorldOfficialGoldHoldingReportScraper
    {
        private const string urlBase = "http://www.gold.org";
        private static readonly Regex dateRegex = new Regex("^([0-9]{1,2})(st|nd|rd|th) ([^ ]+) ([0-9]+)$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex noteRefSuffixRegex = new Regex("(\\d+)\\)$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private int m_isRunning;
        private Worksheet m_worksheet;
        private Func<int, string> m_getNoteFunc;
        private WorldOfficialGoldHoldingReport m_report;

        private static DateTime ParsePublishDate(string s)
        {
            var match1 = dateRegex.Match(s);
            var dayOfMonth = int.Parse(match1.Groups[1].Captures.Cast<Capture>().Single().Value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
            var dayOfMonthSuffix_actual = match1.Groups[2].Captures.Cast<Capture>().Single().Value;
            string dayOfMonthSuffix_expected;
            switch (dayOfMonth % 10)
            {
                case 1:
                    dayOfMonthSuffix_expected = "st";
                    break;
                case 2:
                    dayOfMonthSuffix_expected = "nd";
                    break;
                case 3:
                    dayOfMonthSuffix_expected = "rd";
                    break;
                default:
                    dayOfMonthSuffix_expected = "th";
                    break;
            }
            if (!dayOfMonthSuffix_expected.Equals(dayOfMonthSuffix_actual)) throw new FormatException();
            var monthName = match1.Groups[3].Captures.Cast<Capture>().Single().Value;
            var monthNames = DateTimeFormatInfo.InvariantInfo.MonthNames;
            int i = Array.FindIndex(monthNames, e => monthName.Equals(e, StringComparison.OrdinalIgnoreCase));
            if (i < 0) throw new FormatException();
            var month = i + 1;
            var year = int.Parse(match1.Groups[4].Captures.Cast<Capture>().Single().Value);
            return new DateTime(year, month, dayOfMonth, 0, 0, 0, DateTimeKind.Unspecified);
        }

        public async Task<WorldOfficialGoldHoldingReport> RunAsync()
        {
            if (Interlocked.CompareExchange(ref m_isRunning, 1, 0) != 0) throw new InvalidOperationException();

            m_report = new WorldOfficialGoldHoldingReport();
            Stream xlsxStream = null;
            Workbook workbook;
            try
            {
#if DEBUG
                {
                    var n2 = "world_official_gold_holdings_as_of_april2016_ifs.xlsx";
                    var assem = typeof(WorldOfficialGoldHoldingReportScraper).Assembly;
                    var rn = assem.GetManifestResourceNames().Single(n1 => n1.EndsWith(n2));
                    xlsxStream = assem.GetManifestResourceStream(rn);
                    m_report.PublishTimePoint = new DateTime(2016, 4, 1, 0, 0, 0, DateTimeKind.Unspecified);
                }
#else
                using (var httpClient = new HttpClient())
                {
                    var htmlDoc = await httpClient.GetHtmlDocumentAsync(urlBase + "/research/latest-world-official-gold-reserves");
                    var hostElem1 = htmlDoc.QuerySelectorAll("#block-system-main").Single();
                    var hostElem3 = hostElem1.QuerySelectorAll(".download.xlsx").Single();
                    var xlsxUrlPath = hostElem3.GetAttributeValue("href", null);
                    var getXlsxMessage = new HttpRequestMessage();
                    getXlsxMessage.RequestUri = new Uri(urlBase + xlsxUrlPath);
                    var getXlsxTask = httpClient.SendAsync(getXlsxMessage);
                    var hostElem2 = hostElem1.QuerySelectorAll(".date-display-single").Single();
                    m_report.PublishTimePoint = ParsePublishDate(hostElem2.InnerText);
                    var xlsxMessage = await getXlsxTask;
                    byte[] xlsxBytes = await xlsxMessage.Content.ReadAsByteArrayAsync();
                    xlsxStream = new MemoryStream(xlsxBytes, 0, xlsxBytes.Length, true, true);
                }
#endif
                workbook = Workbook.LoadExcel(xlsxStream);
            }
            finally
            {
                if (xlsxStream != null)
                {
                    xlsxStream.Dispose();
                }
            }
            m_worksheet = workbook.Worksheets.Single();
            InitializeGetNoteFunc();
            
            var i = FindIndex(m_worksheet.Rows, r => r.Cells_NonEmpty.Where(c => c.ValueUnformatted != null && c.ValueUnformatted.Trim().Equals("Tonnes", StringComparison.OrdinalIgnoreCase)).Any());
            m_report.Rows = new List<WorldOfficialGoldHoldingReportRow>();

            while (++i < m_worksheet.Rows.Count)
            {
                WorldOfficialGoldHoldingReportRow rl, rr;
                rl = TryParseEntry(m_worksheet.Rows[i], 0);
                rr = TryParseEntry(m_worksheet.Rows[i], 4);
                if (rl != null) m_report.Rows.Add(rl);
                if (rr != null) m_report.Rows.Add(rr);
                if (rl == null && rr == null) break;
            }


            var cal = CultureInfo.InvariantCulture.Calendar;
            var dataTimePoint = cal.AddMonths(m_report.PublishTimePoint, -2);
            int dataTimePoint_month = cal.GetMonth(dataTimePoint);
            m_report.DataTimePoint = new DateTime(
                cal.GetYear(dataTimePoint), (dataTimePoint_month - 1) / 3 * 3 + 1, 1, 
                0, 0, 0, 
                DateTimeKind.Unspecified);
            return m_report;
        }
        
        private void InitializeGetNoteFunc()
        {
            var textWithNotes = m_worksheet.Drawings.OfType<TextDrawing>().SingleOrDefault();
            if (textWithNotes != null)
            {
                int indexOfParagraphWithFirstNote = FindIndex(textWithNotes.Paragraphs, p => p.Text_Unformatted != null && p.Text_Unformatted.StartsWith("1."));
                if (0 <= indexOfParagraphWithFirstNote)
                {
                    int j = indexOfParagraphWithFirstNote;
                    while (++j < textWithNotes.Paragraphs.Count)
                    {
                        var noteNumberString = (j + 1).ToString(null, NumberFormatInfo.InvariantInfo) + ".";
                        var p = textWithNotes.Paragraphs[j];
                        if (p.Text_Unformatted == null
                            || !p.Text_Unformatted.StartsWith(noteNumberString))
                        {
                            break;
                        }
                    }
                    int noteCount = textWithNotes.Paragraphs.Count - indexOfParagraphWithFirstNote;
                    m_getNoteFunc = noteIndex_oneBased =>
                    {
                        if (noteIndex_oneBased < 1 || noteCount < noteIndex_oneBased)
                        {
                            throw new NotImplementedException();
                        }
                        int k = checked((int)Math.Ceiling(Math.Log10(noteIndex_oneBased)) + 1);
                        return textWithNotes
                            .Paragraphs[indexOfParagraphWithFirstNote - 1 + noteIndex_oneBased]
                            .Text_Unformatted
                            .Substring(k)
                            .Trim();
                    };
                }
            }
        }

        private WorldOfficialGoldHoldingReportRow TryParseEntry(Row wbRow, int firstColumnIndex)
        {
            int firstCellIndex = FindColumn(wbRow.Cells_NonEmpty, firstColumnIndex);
            int lastCellIndex = FindColumn(wbRow.Cells_NonEmpty, firstColumnIndex + 4);
            if (lastCellIndex < 0) lastCellIndex = ~lastCellIndex;
            if (lastCellIndex - firstCellIndex != 4) return null;
            int rank;
            if (!int.TryParse(wbRow.Cells_NonEmpty[firstCellIndex].ValueUnformatted, NumberStyles.None, NumberFormatInfo.InvariantInfo, out rank))
            {
                return null;
            }
            var reportRow = new WorldOfficialGoldHoldingReportRow();
            var reportRow_nameRaw = wbRow.Cells_NonEmpty[firstCellIndex + 1].ValueUnformatted;
            if (reportRow_nameRaw == null) return null;
            var m = noteRefSuffixRegex.Match(reportRow_nameRaw);
            if (m.Success)
            {
                var noteRefCapture = m.Groups[1].Captures.Cast<Capture>().Single();
                var noteIndex_oneBased = int.Parse(
                    noteRefCapture.Value,
                    NumberStyles.None,
                    NumberFormatInfo.InvariantInfo);
                reportRow.Name = reportRow_nameRaw.Substring(0, noteRefCapture.Index);
                if (m_getNoteFunc == null) throw new NotImplementedException();
                reportRow.Note = m_getNoteFunc(noteIndex_oneBased);
            }
            else
            {
                reportRow.Name = reportRow_nameRaw;
            }
            decimal t;
            if (!decimal.TryParse(
                wbRow.Cells_NonEmpty[firstCellIndex + 2].ValueUnformatted,
                NumberStyles.AllowDecimalPoint,
                NumberFormatInfo.InvariantInfo, out t))
                return null;
            reportRow.Tons = t;
            if (decimal.TryParse(
                wbRow.Cells_NonEmpty[firstCellIndex + 3].ValueUnformatted,
                NumberStyles.AllowDecimalPoint,
                NumberFormatInfo.InvariantInfo,
                out t))
            {
                reportRow.PortionOfReserves = t;
            }
            return reportRow;
        }

        private static int FindColumn(IReadOnlyList<Cell> cells, int columnIndex)
        {
            int lo, hi;
            lo = 0;
            hi = cells.Count - 1;
            while (lo <= hi)
            {
                int mi = lo + ((hi - lo) >> 1);
                int c = cells[mi].ColumnIndex.CompareTo(columnIndex);
                if (c < 0)
                {
                    lo = mi + 1;
                }
                else if (0 < c)
                {
                    hi = mi - 1;
                }
                else
                {
                    return mi;
                }
            }
            return ~lo;
        }

        private static int FindIndex<T>(IReadOnlyList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i])) return i;
            }
            return -1;
        }

    }
}
