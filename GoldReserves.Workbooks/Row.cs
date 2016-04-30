using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoldReserves.Workbooks
{
    public class Row
    {
        internal List<Cell> CellsInternal_NonEmpty;
        internal Row()
        {
            CellsInternal_NonEmpty = new List<Cell>();
            Cells_NonEmpty = new ReadOnlyCollection<Cell>(CellsInternal_NonEmpty);
        }
        public IReadOnlyList<Cell> Cells_NonEmpty { get; }
        public int RowIndex { get; internal set; }
        public Worksheet Worksheet { get; internal set; }
    }

}
