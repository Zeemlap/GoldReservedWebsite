using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Data
{
    public class PoliticalEntity
    {

        public GeoRegion GeoRegion { get; set; }
        public int? GeoRegionId { get; set; }

        public int Id { get; set; }

        public ICollection<PoliticalEntityName> Names { get; set; }
    }
}
