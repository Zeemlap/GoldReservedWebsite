using GoldReserves.Backend;
using System.Collections.Generic;

namespace GoldReserves.Web.Models
{
    public class HomeIndexViewModel
    {
        public List<Country> Countries { get; set; }
        public List<CountryViewModel> CountryViewModels { get; set; }
    }
}