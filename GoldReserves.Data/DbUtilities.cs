using Com.Jab.LibCore;
using System;

namespace GoldReserves.Data
{
    internal static class DbUtilities
    {
        public const char Char_FixedStringTypePadding = ' ';
        public const int CharLength_PoliticalEntityName_NamePrefix = 16;
        public const string IndexName_PoliticalEntityName_LanguageId_NamePrefix = "IX_NamePrefix_LanguageId";

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
