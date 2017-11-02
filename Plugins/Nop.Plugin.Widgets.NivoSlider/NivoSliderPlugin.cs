using Core.Plugins;
using Services.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Lpw.Plugin.Widgets.NivoSlider
{
    public class NivoSliderPlugin : BasePlugin, IWidgetPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsNivoSlider";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Lpw.Plugin.Widgets.NivoSlider.Controllers" }, { "area", null } };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsNivoSlider";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Lpw.Plugin.Widgets.NivoSlider.Controllers" },
                { "area", null },
                { "widgetZone", widgetZone}
            };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { "home_page_top" };
        }
    }
}