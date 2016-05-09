using GoldReserves.Backend;
using GoldReserves.Data;
using System.Collections.Generic;

namespace GoldReserves.Web.Models
{
    public class HomeIndexViewModel
    {
        public List<GeoRegion> GeoRegions { get; set; }
        public List<PoliticalEntity> PoliticalEntities { get; set; }
    }
}