using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTest1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            string virtualPath = HttpContext.Request.AppRelativeCurrentExecutionFilePath;
            string applicationPath = HttpContext.Request.ApplicationPath;
            return Content(virtualPath + " + " + applicationPath);
        }
    }
}