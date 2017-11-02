using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Framework.Localization;
using Web.Framework.Mvc.Routes;

namespace WebSite.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            //We reordered our routes so the most used ones are on top. It can improve performance.

            //home page
            routes.MapLocalizedRoute("HomePage",
                            "",
                            new { controller = "Home", action = "Index" },
                            new[] { "WebSite.Controllers.Controllers" });

            routes.MapRoute("WidgetsByZone",
                            "widgetsbyzone",
                            new { controller = "Widget", action = "WidgetsByZone" },
                            new[] { "WebSite.Controllers.Controllers" });

            //page not found
            routes.MapLocalizedRoute("PageNotFound",
                            "page-not-found",
                            new { controller = "Common", action = "PageNotFound" },
                            new[] { "WebSite.Controllers.Controllers" });
        }
    }
}