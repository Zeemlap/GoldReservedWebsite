using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TonsOfGoldPerCountry.Web.Models
{
    public class CountryViewModel
    {

        public string Code { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
    }
}