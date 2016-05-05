using Com.Jab.LibCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldReserves.Data
{
    public class GeoRegion
    {
        private string m_id_alpha3Cache;

        internal short Id_Alpha3Internal { get; set; }

        [NotMapped]
        public string Id_Alpha3
        {
            get
            {
                if (m_id_alpha3Cache != null) return m_id_alpha3Cache;
                m_id_alpha3Cache = DbUtil.Alpha3EncodedShort_Get(Id_Alpha3Internal);
                return m_id_alpha3Cache;
            }
            set
            {
                Id_Alpha3Internal = DbUtil.Alpha3EncodedShort_Set(value);
                m_id_alpha3Cache = value;
            }
        }

        public ICollection<PoliticalEntity> PoliticalEntities { get; set; }
    }
}
