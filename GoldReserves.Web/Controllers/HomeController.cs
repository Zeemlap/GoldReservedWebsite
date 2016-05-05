using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoldReserves.Backend;
using GoldReserves.Web.Models;
using GoldReserves.Data;
using System.Configuration;

namespace GoldReserves.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = AppUtil.DbContext;
            var geoRegionList = db.GeoRegions.AsNoTracking().ToList();
            var politicalEntityList = db.PoliticalEntities.Include("Names").AsNoTracking().ToList();
            var politicalEntityFromName = politicalEntityList.SelectMany(p => p.Names).ToDictionary(pen => pen.Name, pen => pen.PoliticalEntity);
            var report = new Bla().GetWorldGoldReservesReportAsync().Result;
            var politicalEntityViewModelList = new List<PoliticalEntityViewModel>();
            foreach (var reportRow in report.Rows)
            {
                PoliticalEntity politicalEntity;
                politicalEntityFromName.TryGetValue(reportRow.Name, out politicalEntity);
                if (politicalEntity != null)
                {
                    politicalEntityViewModelList.Add(new PoliticalEntityViewModel()
                    {
                        PoliticalEntityId = politicalEntity.Id,
                        ResourceQuantity = (double)reportRow.Tons,
                    });
                }
                else
                {

                }
            }
            return View(new HomeIndexViewModel()
            {
                GeoRegions = geoRegionList,
                PoliticalEntities = politicalEntityList,
                PoliticalEntityViewModels = politicalEntityViewModelList,
            });
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