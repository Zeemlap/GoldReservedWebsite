using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GoldReserves.Backend
{
    public class Bla
    {
        private class CountryGeoCsvLatAndLngConverter : ITypeConverter
        {
            public bool CanConvertFrom(Type type)
            {
                return type == typeof(string);
            }

            public bool CanConvertTo(Type type)
            {
                throw new NotImplementedException();
            }

            public object ConvertFromString(TypeConverterOptions options, string text)
            {
                double v;
                var numStyle = options.NumberStyle ?? (
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
                if (!double.TryParse(text, numStyle, options.CultureInfo, out v))
                {
                    if (text.Length == 0) return double.NaN;
                    throw new CsvTypeConverterException();
                }
                return v;
            }

            public string ConvertToString(TypeConverterOptions options, object value)
            {
                throw new NotImplementedException();
            }
        }

        private class CountryGeoCsvClassMap : CsvClassMap<Country>
        {
            public CountryGeoCsvClassMap()
            {
                Map(c => c.Id_IsoTwoLetterCode).Name("country");
                Map(c => c.Latitude).Name("latitude").TypeConverter<CountryGeoCsvLatAndLngConverter>();
                Map(c => c.Longitude).Name("longitude").TypeConverter<CountryGeoCsvLatAndLngConverter>();
            }
        }
        
        private class CountryIso
        {
            public int Id { get; set; }
            public string Id_IsoTwoLetterCode { get; set; }
            public string Id_IsoThreeLetterCode { get; set; }
            public string EnglishShortName { get; set; }
            public string FrenchShortName { get; set; }
        }

        private class CountryIsoCsvClassMap : CsvClassMap<CountryIso>
        {
            public CountryIsoCsvClassMap()
            {
                Map(ci => ci.EnglishShortName).Name("English short name");
                Map(ci => ci.FrenchShortName).Name("French short name");
                Map(ci => ci.Id_IsoTwoLetterCode).Name("Alpha-2 code");
                Map(ci => ci.Id_IsoThreeLetterCode).Name("Alpha-3 code");
                Map(ci => ci.Id).Name("Numeric");
            }
        }


        public List<Country> GetCountries()
        {
            var assem = typeof(Bla).Assembly;
            var names = assem.GetManifestResourceNames();
            List<Country> countryList;
            using (var stream = typeof(Bla).Assembly.GetManifestResourceStream(names.Single(n => n.EndsWith("countries_geo.csv"))))
            {
                var csvConfig = new CsvConfiguration();
                csvConfig.CultureInfo = CultureInfo.InvariantCulture;
                csvConfig.Delimiter = "\t";
                csvConfig.RegisterClassMap<CountryGeoCsvClassMap>();
                var csvReader = new CsvReader(new StreamReader(stream, Encoding.UTF8, false, 1024, true), csvConfig);
                var countryEnum = csvReader.GetRecords<Country>();
                countryList = countryEnum.ToList();
            }
            Dictionary<string, CountryIso> countryIsoFromRegionNameTwoLetter;
            // all officially assigned codes on https://www.iso.org/obp/ui/#search
            using (var stream = typeof(Bla).Assembly.GetManifestResourceStream(names.Single(n => n.EndsWith("countries_iso.csv"))))
            {
                var csvConfig = new CsvConfiguration();
                csvConfig.CultureInfo = CultureInfo.InvariantCulture;
                csvConfig.Delimiter = ",";
                csvConfig.RegisterClassMap<CountryIsoCsvClassMap>();
                var csvReader = new CsvReader(new StreamReader(stream, Encoding.UTF8, false, 1024, true), csvConfig);
                var countryIsoEnum = csvReader.GetRecords<CountryIso>();
                countryIsoFromRegionNameTwoLetter = countryIsoEnum.ToDictionary(ci => ci.Id_IsoTwoLetterCode);
            }
            countryIsoFromRegionNameTwoLetter["AN"] = new CountryIso()
            {
                EnglishShortName = "Netherlands Antilles",
                Id_IsoTwoLetterCode = "AN",
                Id_IsoThreeLetterCode = "ANT",
            };
            countryIsoFromRegionNameTwoLetter["GZ"] = new CountryIso()
            {
                EnglishShortName = "Gaza Strip",
                Id_IsoTwoLetterCode = "GZ",
                Id_IsoThreeLetterCode = "AAA",
            };
            countryIsoFromRegionNameTwoLetter["XK"] = new CountryIso()
            {
                EnglishShortName = "Kosovo",
                Id_IsoTwoLetterCode = "XK",
                Id_IsoThreeLetterCode = "XKX",
            };
            foreach (var country in countryList)
            {
                var countryIso = countryIsoFromRegionNameTwoLetter[country.Id_IsoTwoLetterCode];
                country.Id_IsoThreeLetterCode = countryIso.Id_IsoThreeLetterCode;
                country.Name_English = countryIso.EnglishShortName;
            }
            return countryList;
        }

    }
}
