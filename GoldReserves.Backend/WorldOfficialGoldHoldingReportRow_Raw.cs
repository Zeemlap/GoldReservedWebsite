using GoldReserves.Data;
using System;

namespace GoldReserves.Backend
{
    public class WorldOfficialGoldHoldingReportRow_Raw
    {
        public string Name { get; set; }

        public string Note { get; set; }
        
        public decimal? Tons { get; set; }
    }
}
