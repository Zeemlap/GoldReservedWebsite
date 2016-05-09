using Com.Jab.LibCore;
using GoldReserves.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    public class WorldOfficialGoldHoldingReport_Raw
    {

        internal short DataTimePointInternal { get; set; }
        internal int PublishTimePointInternal { get; set; }
        
        public DateTime DataTimePoint
        {
            get
            {
                return new TimePointInQuarters(DataTimePointInternal).ToDateTime();
            }
            set
            {
                DataTimePointInternal = new TimePointInQuarters(value).Value;
            }
        }

        public DateTime PublishTimePoint
        {
            get
            {
                return new TimePointInMinutes(PublishTimePointInternal).ToDateTime();
            }
            set
            {
                PublishTimePointInternal = new TimePointInMinutes(value).Value;
            }
        }

        public List<WorldOfficialGoldHoldingReportRow_Raw> Rows { get; set; }

    }
}
