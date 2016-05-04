using Com.Jab.LibCore;
using System;
using System.Collections.Generic;

namespace GoldReserves.Data
{
    public class GeoRegion
    {
        private string m_id_alpha3Cache;

        internal short Id_Alpha3Internal { get; set; }

        public unsafe string Id_Alpha3
        {
            get
            {
                if (m_id_alpha3Cache != null) return m_id_alpha3Cache;
                int i = Id_Alpha3Internal;
                string s = Util.StringAllocate(3);
                fixed (char* sChPtr = s)
                {
                    sChPtr[0] = (char)((i >> 10) + 'A');
                    sChPtr[1] = (char)(((i >> 5) & 0x1F) + 'A');
                    sChPtr[2] = (char)((i & 0x1F) + 'A');
                }
                m_id_alpha3Cache = s;
                return s;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (value.Length != 3) throw new ArgumentException();
                int i;
                fixed (char* sChPtr = value)
                {
                    int v = sChPtr[0] - 'A';
                    if (26 <= v) throw new ArgumentException();
                    i = v << 10;
                    v = sChPtr[1] - 'A';
                    if (26 <= v) throw new ArgumentException();
                    i |= v << 5;
                    v = sChPtr[2] - 'A';
                    if (26 <= v) throw new ArgumentException();
                    i |= v;
                }
                Id_Alpha3Internal = unchecked((short)i);
                m_id_alpha3Cache = value;
            }
        }

        public ICollection<PoliticalEntity> PoliticalEntities { get; set; }
    }
}
