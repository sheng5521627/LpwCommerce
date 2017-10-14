using Core.Caching;
using Core.Configuration;
using Core.Infrastructure;
using Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTest.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(NopConfig config)
        {
            
        }

        public ActionResult Index()
        {
            NopConfig config = EngineContext.Current.Resolve<NopConfig>();
            RedisCacheManager cache = new RedisCacheManager(config);
            cache.Set("name", "lpw", 1);

            string name = cache.Get<string>("name");


            return View();
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