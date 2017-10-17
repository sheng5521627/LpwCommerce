using Core.Caching;
using Core.Configuration;
using Core.Infrastructure;
using Core.Plugins;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserAgentHelper _userAgetnHelper;
        public HomeController(NopConfig config, IUserAgentHelper userAgetnHelper)
        {
            _userAgetnHelper = userAgetnHelper;
        }

        public ActionResult Index()
        {
            bool flag = _userAgetnHelper.IsSearchEngine();
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