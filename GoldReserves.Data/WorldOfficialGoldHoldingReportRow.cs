using Com.Jab.LibCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Data
{
    public class WorldOfficialGoldHoldingReportRow
    {
        public int Id { get; set; }
        
        public string Note { get; set; }

        public PoliticalEntity PoliticalEntity { get; set; }
        public int PoliticalEntityId { get; set; }
        
        public WorldOfficialGoldHoldingReport Report { get; set; }
        internal short ReportDataTimePointInternal { get; set; }
        [NotMapped]
        public DateTime ReportDataTimePoint
        {
            get
            {
                return new TimePointInQuarters(ReportDataTimePointInternal).ToDateTime();
            }
            set
            {
                ReportDataTimePointInternal = new TimePointInQuarters(value).Value;
            }
        }

        public decimal? Tons { get; set; }
    }
}
