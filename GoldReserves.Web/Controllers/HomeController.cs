using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoldReserves.Backend;
using GoldReserves.Web.Models;

namespace GoldReserves.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var countryList1 = new Bla().GetCountries();
            var countryList2 = (from c in countryList1
                                where !double.IsNaN(c.Latitude) && !double.IsNaN(c.Longitude)
                                select new CountryViewModel()
                                {
                                    Code = c.Code,
                                    Latitude = c.Latitude,
                                    Longitude = c.Longitude,
                                    Name = c.Name,
                                }).ToList();
            return View(new HomeIndexViewModel() { Countries = countryList2, });
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}