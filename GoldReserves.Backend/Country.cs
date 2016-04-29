using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldReserves.Backend
{
    public class Country
    {
        public string Id_IsoTwoLetterCode { get; set; }
        public string Id_IsoThreeLetterCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name_English { get; set; }
    }
}
