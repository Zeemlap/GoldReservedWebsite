using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoldReserves.Web.Models
{
    public class CountryViewModel
    {
        public string Id_IsoTwoLetterCode { get; set; }
        public double ResourceQuantity { get; set; }
    }
}