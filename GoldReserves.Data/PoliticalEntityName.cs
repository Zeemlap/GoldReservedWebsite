using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Data
{
    public class PoliticalEntityName
    {
        private string m_nameCache;
        private bool m_nameCacheIsValid;

        public int Id { get; set; }
        
        internal string NamePrefix { get; set; }
        internal string NameSuffix { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                if (m_nameCacheIsValid) return m_nameCache;
                m_nameCache = DbUtil.IndexedVarChar_Merge(NamePrefix, NameSuffix);
                m_nameCacheIsValid = true;
                return m_nameCache;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();
                string namePrefix, nameSuffix;
                DbUtil.IndexedVarChar_Split(value,
                    DbUtil.CharLength_PoliticalEntityName_NamePrefix,
                    out namePrefix,
                    out nameSuffix);
                NamePrefix = namePrefix;
                NameSuffix = nameSuffix;
                m_nameCache = value;
                m_nameCacheIsValid = true;
            }
        }

        public Language Language { get; set; }
        public short LanguageId { get; set; }

        public PoliticalEntity PoliticalEntity { get; set; }
        public int PoliticalEntityId { get; set; }
    }
}
