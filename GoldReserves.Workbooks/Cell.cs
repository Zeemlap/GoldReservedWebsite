using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Workbooks
{
    public class Cell
    {
        internal Cell() { }
        public int ColumnIndex { get; internal set; }
        public Row Row { get; internal set; }
        public int RowIndex { get { return Row.RowIndex; } }
        public string ValueUnformatted { get; set; }
    }
}
