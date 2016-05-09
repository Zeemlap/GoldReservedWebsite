using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoldReserves.Backend;
using GoldReserves.Web.Models;
using GoldReserves.Data;
using System.Configuration;
using Newtonsoft.Json;
using System.Globalization;

namespace GoldReserves.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = AppUtil.DbContext;
            
            var geoRegionList = db.GeoRegions.AsNoTracking().ToList();
            var politicalEntityList = db.PoliticalEntities.Include("Names").AsNoTracking().ToList();
            return View(new HomeIndexViewModel()
            {
                GeoRegions = geoRegionList,
                PoliticalEntities = politicalEntityList,
            });
        }

        [HttpGet]
        public ActionResult ResourceQuantityPerPoliticalEntity(string dataTimePoint) // todo add resource type in future...
        {
            const string iso8601Pattern = "yyyy-MM-ddTHH\\:mm\\:ss.fff\\Z";
            DateTime dt_dataTimePoint;
            if (dataTimePoint == null || !DateTime.TryParseExact(dataTimePoint, iso8601Pattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out dt_dataTimePoint))
            {
                throw new ArgumentException();
            }
            var db = AppUtil.DbContext;
            var bla = db.GetWorldOfficialGoldHoldings(dt_dataTimePoint);
            var jsonSer = new JsonSerializer();
            jsonSer.Converters.Add(new TonsPerPoliticalEntityJsonConverter());
            return new NewtonsoftJsonResult(jsonSer, bla);
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