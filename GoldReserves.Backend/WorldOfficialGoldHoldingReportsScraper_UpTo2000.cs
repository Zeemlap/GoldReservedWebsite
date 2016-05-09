using Com.Jab.LibCore;
using Com.Jba.LibCore.Unicode;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    internal class WorldOfficialGoldHoldingReportsScraper_UpTo2000
    {
        private static Regex s_regexTableInfo = new Regex("^Table ([1-9][0-9]*)(?: continued)?:", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex s_regexNote = new Regex("<sup>\\d+</sup>$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex s_regexEstimate = new Regex("<sup>\\(e\\)</sup>$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        public List<WorldOfficialGoldHoldingReport_Raw> Run()
        {
            List<string> textLines;
            var t = typeof(WorldOfficialGoldHoldingReportsScraper_UpTo2000);
            using (var stream = t.Assembly.GetManifestResourceStream(t, "annual_time_series_on_world_official_gold_reserves.pdf"))
            using (var pdfReader = new PdfReader(stream))
            {
                var textExtractor = new TextExtractor();
                for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                {
                    PdfTextExtractor.GetTextFromPage(pdfReader, i, textExtractor);
                    textExtractor.NextPage();
                }
                textExtractor.Lines.Sort((l1, l2) =>
                {
                    int c = l1.PageNumber.CompareTo(l2.PageNumber);
                    if (c != 0) return c;
                    return l2.Y.CompareTo(l1.Y);
                });
                textLines = textExtractor.Lines.Select(l => l.Value).ToList();
            }
            List<WorldOfficialGoldHoldingReport_Raw> reports1 = new List<WorldOfficialGoldHoldingReport_Raw>();
            List<WorldOfficialGoldHoldingReport_Raw> reports2 = null;
            int tableNo1 = 0;
            List<string> textLinesNonTabular = new List<string>();
            for (int i = 0; i < textLines.Count; i++)
            {
                var m1 = s_regexTableInfo.Match(textLines[i]);
                if (m1.Success)
                {
                    int tableNo2 = int.Parse(m1.Groups[1].Captures.Cast<Capture>().Single().Value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
                    if (tableNo1 != tableNo2)
                    {
                        if (3 <= tableNo2) goto winning;
                        tableNo1 = tableNo2;
                    }
                    if (++i == textLines.Count) goto winning;
                    var yearPerColumn = textLines[i].Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s =>
                        {
                            var m2 = s_regexNote.Match(s);
                            return int.Parse(m2.Success
                                ? s.Substring(0, m2.Index)
                                : s, NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo);
                        }).ToList();
                    if (yearPerColumn.Any(year => 2000 <= year)) throw new Exception();
                    if (reports2 != null) reports1.AddRange(reports2);
                    reports2 = yearPerColumn.Select(year => new WorldOfficialGoldHoldingReport_Raw()
                    {
                        DataTimePoint = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                        PublishTimePoint = new DateTime(2011, 8, 10, 0, 0, 0, DateTimeKind.Unspecified),
                        Rows = new List<WorldOfficialGoldHoldingReportRow_Raw>(),
                    }).ToList();
                    continue;
                }
                var reportRowPerCell = TryGetCells(textLines[i], reports2.Count);
                if (reportRowPerCell != null)
                {
                    for (int j = 0; j < reports2.Count; j++)
                    {
                        reports2[j].Rows.Add(reportRowPerCell[j]);
                    }
                    continue;
                }
                textLinesNonTabular.Add(textLines[i]);
            }
            winning:
            if (reports2 != null) reports1.AddRange(reports2);
            return reports1;
        }

        private static List<WorldOfficialGoldHoldingReportRow_Raw> TryGetCells(string textLine, int noCols)
        {
            var cellValues = textLine.Split(' ');
            var name_colSpan = cellValues.Length - noCols;
            if (name_colSpan <= 0) return null;
            var name = string.Join(" ", cellValues.Take(name_colSpan));
            var reportRowPerCell = new List<WorldOfficialGoldHoldingReportRow_Raw>();
            for (int colIdx = 0; colIdx < noCols; colIdx++)
            {
                var cellVal = cellValues[name_colSpan + colIdx];
                int cellValLen = cellVal.Length;
                var match = s_regexNote.Match(cellVal);
                if (match.Success) cellValLen = match.Index;
                var reportRow = new WorldOfficialGoldHoldingReportRow_Raw();
                reportRow.Name = name;
                // In table 1 "0" means less than 0.01 tons and "-" means zero.
                if (1 == cellValLen && string.CompareOrdinal(cellVal, 0, "-", 0, 1) == 0)
                {
                    reportRow.Tons = 0M;
                }
                else if (cellValLen != 3 || string.CompareOrdinal(cellVal, 0, "n/a", 0, 3) != 0)
                {
                    if (cellValLen == cellVal.Length && (match = s_regexEstimate.Match(cellVal)).Success)
                    {
                        cellValLen = match.Index;
                    }
                    decimal cellValNum;
                    if (!decimal.TryParse(cellVal.Substring(0, cellValLen), NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out cellValNum))
                    {
                        return null;
                    }
                    reportRow.Tons = cellValNum;
                }
                reportRowPerCell.Add(reportRow);
            }
            return reportRowPerCell;
        }

        
        private class TextRenderInfoSimple
        {
            public float OriginX;
            public float OriginY;
            public float Advancement;
            public float Descent;
            public float Ascent;
            // x-height?
            // cap-height?

            public bool TrySet(TextRenderInfo renderInfo)
            {
                var p1 = renderInfo.GetBaseline().GetStartPoint();
                var p2 = renderInfo.GetBaseline().GetEndPoint();
                if (p1[1] != p2[1]
                    || p1[2] != p2[2]
                    || p1[2] != 1)
                    return false;
                OriginX = p1[0];
                OriginY = p1[1];
                Advancement = p2[0] - p1[0];
                p1 = renderInfo.GetAscentLine().GetStartPoint();
                p2 = renderInfo.GetAscentLine().GetEndPoint();
                if (p1[1] != p2[1]
                    || p1[2] != p2[2]
                    || p1[2] != 1
                    || Advancement != p2[0] - p1[0])
                    return false;
                Ascent = p1[1] - OriginY;
                if (Ascent < 0) return false;
                p1 = renderInfo.GetDescentLine().GetStartPoint();
                p2 = renderInfo.GetDescentLine().GetEndPoint();
                if (p1[1] != p2[1]
                    || p1[2] != p2[2]
                    || p1[2] != 1
                    || Advancement != p2[0] - p1[0])
                    return false;
                Descent = OriginY - p1[1];
                if (Descent < 0) return false;
                return true;
            }

        }

        private class CharGroup
        {
            public float OriginX;
            public float OriginXPlusAdvancement;
            public string Value;
            public float SingleSpaceWidth;
            public bool AlwaysInsertSpaceBefore;
            public bool CanBeSuperscript;
        }

        private class LineBuilder
        {
            public float MinY, MaxY;
            public List<CharGroup> m_charGroups;

            public LineBuilder()
            {
                m_charGroups = new List<CharGroup>();
            }

            public CharGroup AddCharGroup(float originX)
            {
                int lo = 0;
                int hi = m_charGroups.Count - 1;
                int i = -1;
                CharGroup charGroup2;
                while (lo <= hi)
                {
                    int mi = lo + ((hi - lo) >> 1);
                    var charGroup1 = m_charGroups[mi];
                    if (charGroup1.OriginX < originX)
                    {
                        lo = mi + 1;
                    }
                    else if (originX < charGroup1.OriginX)
                    {
                        hi = mi - 1;
                    }
                    else
                    {
                        if (charGroup1.Value.IsWhiteSpace())
                        {
                            charGroup1.AlwaysInsertSpaceBefore = true;
                            return charGroup1;
                        }
                        throw new NotImplementedException();
                    }
                }
                i = m_charGroups.Count == 0 ? 0 : lo < 0 ? ~lo : lo;
                charGroup2 = new CharGroup()
                {
                    OriginX = originX,
                };
                m_charGroups.Insert(i, charGroup2);
                return charGroup2;
            }

            public override string ToString()
            {
                return ToStringInternal();
            }

            internal string ToStringInternal()
            {

                var sb = new StringBuilder();
                ToString(sb);
                return sb.ToString();
            }

            internal void ToString(StringBuilder sb)
            {
                int n = m_charGroups.Count;
                if (n == 0) return;
                sb.Append(m_charGroups[0].Value);
                if (m_charGroups[0].CanBeSuperscript) throw new NotImplementedException();
                bool isInSuperscript = false;
                for (int i = 1; i < n; i++)
                {
                    float widthOfSpacingBefore = m_charGroups[i].OriginX - m_charGroups[i - 1].OriginXPlusAdvancement;
                    if (isInSuperscript && !m_charGroups[i].CanBeSuperscript)
                    {
                        sb.Append("</sup>");
                        isInSuperscript = false;
                    }
                    bool flag1 = m_charGroups[i - 1].SingleSpaceWidth <= widthOfSpacingBefore;
                    bool flag2 = m_charGroups[i].SingleSpaceWidth <= widthOfSpacingBefore;
                    if (flag1 != flag2) throw new NotImplementedException();
                    if ((flag1 || m_charGroups[i].AlwaysInsertSpaceBefore)
                        && !CodePoint.IsWhiteSpace(sb.CodePointsReverse().FirstOrDefault()))
                    {
                        sb.Append(' ');
                    }
                    if (!isInSuperscript && m_charGroups[i].CanBeSuperscript)
                    {
                        sb.Append("<sup>");
                        isInSuperscript = true;
                    }
                    sb.Append(m_charGroups[i].Value);
                }
                if (isInSuperscript)
                {
                    sb.Append("</sup>");
                }
            }
        }
                
        private class Line
        {
            public int PageNumber;
            public float Y;
            public string Value;
        }

        private class TextExtractor : ITextExtractionStrategy
        {
            private TextRenderInfoSimple m_textRenderInfoSimple;
            private List<LineBuilder> m_lineBuilders;
            private List<Line> m_lines;
            private StringBuilder m_sbCache;
            private int m_pageNumber;

            public TextExtractor()
            {
                m_pageNumber = 1;
                m_lineBuilders = new List<LineBuilder>();
                m_textRenderInfoSimple = new TextRenderInfoSimple();
                m_lines = new List<Line>();
                m_sbCache = new StringBuilder();
            }
            
            public List<Line> Lines
            {
                get
                {
                    return m_lines;
                }
            }

            public void NextPage()
            {
                foreach (var lineBuilder in m_lineBuilders)
                {
                    try
                    {
                        lineBuilder.ToString(m_sbCache);
                        m_lines.Add(new Line()
                        {
                            PageNumber = m_pageNumber,
                            Y = lineBuilder.MinY,
                            Value = m_sbCache.ToString(),
                        });
                    }
                    finally
                    {
                        m_sbCache.Clear();
                    }
                }
                m_pageNumber += 1;
                m_lineBuilders.Clear();
            }

            public void BeginTextBlock()
            {
            }

            public void EndTextBlock()
            {    
            }

            public string GetResultantText()
            {
                return string.Empty;
            }

            public void RenderImage(ImageRenderInfo renderInfo)
            {
            }

            private int GetLineBuilderIndex(float min, float max)
            {
                int lo = 0;
                int hi = m_lineBuilders.Count - 1;
                while (lo <= hi)
                {
                    int mi = lo + ((hi - lo) >> 1);
                    var line = m_lineBuilders[mi];
                    if (max < line.MinY)
                    {
                        hi = mi - 1;
                    }
                    else if (line.MaxY < min)
                    {
                        lo = mi + 1;
                    }
                    else
                    {
                        if (min < line.MinY)
                        {
                            if (0 < mi && min <= m_lineBuilders[mi - 1].MaxY) throw new NotImplementedException();
                            line.MinY = min;
                        }
                        if (line.MaxY < max)
                        {
                            if (mi < m_lineBuilders.Count - 1 && m_lineBuilders[mi + 1].MinY <= max) throw new NotImplementedException();
                            line.MaxY = max;
                        }
                        return mi;
                    }
                }
                return ~lo;
            }

            public void RenderText(TextRenderInfo renderInfo)
            {
                float rise = renderInfo.GetRise();
                if (rise != 0) throw new NotImplementedException();
                if (!m_textRenderInfoSimple.TrySet(renderInfo))
                {
                    throw new NotImplementedException();
                }
                var text = renderInfo.GetText();
                float descentY = m_textRenderInfoSimple.OriginY - m_textRenderInfoSimple.Descent;
                float ascentY = m_textRenderInfoSimple.OriginY + m_textRenderInfoSimple.Ascent;
                int lineBuilderIndex = GetLineBuilderIndex(descentY, ascentY);
                LineBuilder lineBuilder;
                if (lineBuilderIndex < 0)
                {
                    lineBuilderIndex = ~lineBuilderIndex;
                    if (0 < lineBuilderIndex && descentY <= m_lineBuilders[lineBuilderIndex - 1].MaxY) throw new NotImplementedException();
                    if (lineBuilderIndex < m_lineBuilders.Count && m_lineBuilders[lineBuilderIndex].MinY <= ascentY) throw new NotImplementedException();
                    lineBuilder = new LineBuilder();
                    lineBuilder.MinY = descentY;
                    lineBuilder.MaxY = ascentY;
                    m_lineBuilders.Insert(lineBuilderIndex, lineBuilder);
                }
                lineBuilder = m_lineBuilders[lineBuilderIndex];
                var charGroup = lineBuilder.AddCharGroup(m_textRenderInfoSimple.OriginX);
                charGroup.OriginXPlusAdvancement = m_textRenderInfoSimple.OriginX + m_textRenderInfoSimple.Advancement;
                charGroup.Value = text;
                charGroup.SingleSpaceWidth = renderInfo.GetSingleSpaceWidth();
                charGroup.CanBeSuperscript = lineBuilder.MinY < descentY && !IsCloseTo(lineBuilder.MinY, descentY) && IsCloseTo(lineBuilder.MaxY, ascentY);
            }
        }

        private static bool IsCloseTo(float x, float y)
        {
            return Math.Abs(x - y) < 0.04;
        }
    }
}