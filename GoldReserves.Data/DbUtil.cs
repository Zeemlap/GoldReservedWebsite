using Com.Jab.LibCore;
using System;

namespace GoldReserves.Data
{
    internal static class DbUtil
    {
        public const char Char_FixedStringTypePadding = ' ';
        public const int CharLength_PoliticalEntityName_NamePrefix = 16;
        public const string ColumnName_PoliticalEntityName_LanguageId = "LanguageId";
        public const string ColumnName_PoliticalEntityName_NamePrefix = "NamePrefix";

        internal unsafe static string Alpha3EncodedShort_Get(int id)
        {
            string s = Util.StringAllocate(3);
            fixed (char* sChPtr = s)
            {
                sChPtr[0] = (char)((id >> 10) + 'A');
                sChPtr[1] = (char)(((id >> 5) & 0x1F) + 'A');
                sChPtr[2] = (char)((id & 0x1F) + 'A');
            }
            return s;
        }

        internal unsafe static short Alpha3EncodedShort_Set(string id_alpha3)
        {
            if (id_alpha3 == null) throw new ArgumentNullException();
            if (id_alpha3.Length != 3) throw new ArgumentException();
            int id;
            fixed (char* sChPtr = id_alpha3)
            {
                int v = sChPtr[0] - 'A';
                if (26 <= v) throw new ArgumentException();
                id = v << 10;
                v = sChPtr[1] - 'A';
                if (26 <= v) throw new ArgumentException();
                id |= v << 5;
                v = sChPtr[2] - 'A';
                if (26 <= v) throw new ArgumentException();
                id |= v;
            }
            return unchecked((short)id);
        }
        
        public const string ColumnName_PoliticalEntityName_NameSuffix = "NameSuffix";
        public const string IndexName_PoliticalEntityName_LanguageId_NamePrefix = "IX_NamePrefix_LanguageId";
        public const string IndexName_WorldOfficialGoldHoldingReportRow_PoliticalEntityId_ReportDataTimePointInternal = "IX_PoliticalEntityId_ReportDataTimePointInternal";
        public const string LanguageName_English = "english";

        internal static string IndexedVarChar_Merge(string valuePrefix, string valueSuffix)
        {
            if (valuePrefix != null)
            {
                if (valueSuffix == null)
                {
                    int n = valuePrefix.TrimEndWhile(0, valuePrefix.Length, cp => cp == Char_FixedStringTypePadding);
                    return valuePrefix.Substring(0, n);
                }
                return valuePrefix + valueSuffix;
            }
            if (valueSuffix != null) throw new ArgumentException();
            return null;
        }

        internal static void IndexedVarChar_Split(string value, int prefixLength, out string valuePrefix, out string valueSuffix)
        {
            if (prefixLength <= 0) throw new ArgumentOutOfRangeException();
            if (value != null)
            {
                if (value.Length <= prefixLength)
                {
                    if (value[value.Length - 1] == Char_FixedStringTypePadding)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    valuePrefix = value;
                    valueSuffix = null;
                    return;
                }
                valuePrefix = value.Substring(0, prefixLength);
                valueSuffix = value.Substring(prefixLength);
                return;
            }
            valuePrefix = null;
            valueSuffix = null;
        }
    }
}
