using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoldReserves.Backend;
using GoldReserves.Web.Models;
using GoldReserves.Data;

namespace GoldReserves.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var countryList = new Bla().GetCountries();
            var report = new Bla().GetWorldGoldReservesReportAsync().Result;
            var rowFromName = report.Rows.ToDictionary(e => e.Name);
            var countryViewModelList = new List<CountryViewModel>();
            foreach (var country in countryList)
            {
                WorldOfficialGoldHoldingReportRow row;
                rowFromName.TryGetValue(country.Name_English, out row);
                if (row != null)
                {
                    countryViewModelList.Add(new CountryViewModel()
                    {
                        Id_IsoTwoLetterCode = country.Id_IsoTwoLetterCode,
                        ResourceQuantity = (double)row.Tons,
                    });
                }
            }
            return View(new HomeIndexViewModel() { Countries = countryList, CountryViewModels = countryViewModelList, });
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