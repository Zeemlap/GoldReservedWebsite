using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GoldReserves.Backend
{
    public class Bla
    {
        private class CountryCsvLatAndLngConverter : ITypeConverter
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

        private class CountryCsvClassMap : CsvClassMap<Country>
        {
            public CountryCsvClassMap()
            {
                Map(c => c.Code).Name("country");
                Map(c => c.Latitude).Name("latitude").TypeConverter<CountryCsvLatAndLngConverter>();
                Map(c => c.Longitude).Name("longitude").TypeConverter<CountryCsvLatAndLngConverter>();
                Map(c => c.Name).Name("name");
            }
        }

        public List<Country> GetCountries()
        {

            var names = typeof(Bla).Assembly.GetManifestResourceNames();

            List<Country> countryList;
            using (var stream = typeof(Bla).Assembly.GetManifestResourceStream(names[0]))
            {
                var csvConfig = new CsvConfiguration();
                csvConfig.CultureInfo = CultureInfo.InvariantCulture;
                csvConfig.Delimiter = "\t";
                csvConfig.RegisterClassMap<CountryCsvClassMap>();
                var csvReader = new CsvReader(new StreamReader(stream, Encoding.UTF8, false, 1024, true), csvConfig);
                var countryEnum = csvReader.GetRecords<Country>();
                countryList = countryEnum.ToList();
            }
            return countryList;
        }

    }
}
