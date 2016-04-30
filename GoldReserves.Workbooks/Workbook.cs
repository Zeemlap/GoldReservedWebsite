using GoldReserves.Workbooks.Excel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace GoldReserves.Workbooks
{
    public class Workbook
    {
        internal List<Worksheet> WorksheetsInternal;
        
        internal Workbook()
        {
            WorksheetsInternal = new List<Worksheet>();
            Worksheets = new ReadOnlyCollection<Worksheet>(WorksheetsInternal);
        }

        public IReadOnlyList<Worksheet> Worksheets { get; }

        public static Workbook LoadExcel(Stream excelStream)
        {
            using (var excelWbReader = new ExcelWorkbookReader(excelStream))
            {
                return excelWbReader.Read();
            }
        }

    }
}
