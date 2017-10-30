using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Framework
{
    public static class UrlHelperExtensions
    {
        public static string LogOn(this UrlHelper urlHelper,string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Login", "Customer", new { ReturnUrl = returnUrl });

            return urlHelper.Action("Login", "Customer");
        }

        public static string LogOff(this UrlHelper urlHelper,string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("LogOut", "Customer", new { ReturnUrl = returnUrl });

            return urlHelper.Action("LogOut", "Customer");
        }
    }
}
