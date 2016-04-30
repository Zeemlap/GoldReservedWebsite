using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    public class WorldGoldReservesReportEntry
    {
        public string EntryName { get; set; }
        public decimal Tons { get; set; }
        public decimal? PortionOfReserves { get; set; }
        public string Note { get; set; }
    }
}
