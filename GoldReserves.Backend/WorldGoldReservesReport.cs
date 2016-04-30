using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    public class WorldGoldReservesReport
    {
        public DateTime PublishDate { get; set; }
        public List<WorldGoldReservesReportEntry> Entries { get; set; }
        public int DataYear { get; set; }
        public int DataQuarter { get; set; }
    }

}
