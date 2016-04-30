using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoldReserves.Workbooks
{
    public class Worksheet
    {
        internal List<Drawing> DrawingsInternal;
        internal List<Row> RowsInternal;
        
        internal Worksheet()
        {
            DrawingsInternal = new List<Drawing>();
            Drawings = new ReadOnlyCollection<Drawing>(DrawingsInternal);
            RowsInternal = new List<Row>();
            Rows = new ReadOnlyCollection<Row>(RowsInternal);
        }

        
        public IReadOnlyList<Drawing> Drawings { get; }
        public string Name { get; internal set; }
        public IReadOnlyList<Row> Rows { get; }
        public Workbook Workbook { get; internal set; }
    }
}
