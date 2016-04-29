using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoldReserves.Web.Models
{
    public class CountryViewModel
    {

        public string Id_IsoThreeLetterCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name_English { get; set; }
        public string Id_IsoTwoLetterCode { get; set; }
    }
}