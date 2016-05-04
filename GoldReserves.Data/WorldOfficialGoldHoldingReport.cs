using Com.Jab.LibCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldReserves.Data
{
    public class WorldOfficialGoldHoldingReport
    {
        internal short DataTimePointInternal { get; set; }
        internal int PublishTimePointInternal { get; set; }
        
        [NotMapped]
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
        
        [NotMapped]
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

        public ICollection<WorldOfficialGoldHoldingReportRow> Rows { get; set; }

    }
}
