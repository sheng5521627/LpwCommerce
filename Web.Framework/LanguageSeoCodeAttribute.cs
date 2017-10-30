using Core;
using Core.Data;
using Core.Domain.Localization;
using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Framework.Localization;

namespace Web.Framework
{
    public class LanguageSeoCodeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
            if (!localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                return;

            //ensure that this route is registered and localizable (LocalizedRoute in RouteProvider.cs)
            if (filterContext.RouteData == null || filterContext.RouteData.Route == null || !(filterContext.RouteData.Route is LocalizedRoute))
                return;

            var pageUrl = filterContext.HttpContext.Request.RawUrl;
            string applicationPath = filterContext.HttpContext.Request.ApplicationPath;
            //already localized URL
            if (pageUrl.IsLocalizedUrl(applicationPath, true))
                return;
            //add language code to URL
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            pageUrl = pageUrl.AddLanguageSeoCodeToRawUrl(applicationPath, workContext.WorkingLanguage);
            filterContext.Result = new RedirectResult(pageUrl, true);
        }
    }
}
