using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Data
{
    public class PoliticalEntity
    {
        private string m_geoRegionId_alpha3Cache;
        private bool m_geoRegionId_alpha3Cache_isValid;

        public GeoRegion GeoRegion { get; set; }
        internal short? GeoRegionId { get; set; }
        [NotMapped]
        public unsafe string GeoRegionId_Alpha3
        {
            get
            {

                if (m_geoRegionId_alpha3Cache_isValid) return m_geoRegionId_alpha3Cache;
                m_geoRegionId_alpha3Cache = GeoRegionId == null ? null : DbUtil.Alpha3EncodedShort_Get((short)GeoRegionId);
                m_geoRegionId_alpha3Cache_isValid = true;
                return m_geoRegionId_alpha3Cache;
            }
            set
            {
                if (value != null)
                {
                    GeoRegionId = DbUtil.Alpha3EncodedShort_Set(value);
                }
                else
                {
                    GeoRegionId = null;
                }
                m_geoRegionId_alpha3Cache = value;
                m_geoRegionId_alpha3Cache_isValid = true;
            }
        }

        public int Id { get; set; }

        public ICollection<PoliticalEntityName> Names { get; set; }
    }
}
