using FluentValidation;
using FluentValidation.Attributes;
using Services.Stores;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Framework.Mvc;

namespace WebSite.Controllers
{
    public class HomeController : BasePublicController
    {
        public HomeController(IStoreService storeService)
        {

        }
        
        public ActionResult Index()
        {
            return View();
        }
    }
}