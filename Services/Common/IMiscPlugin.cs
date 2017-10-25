﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Core.Plugins;

namespace Services.Common
{
    /// <summary>
    /// Misc plugin interface. 
    /// It's used by the plugins that have a configuration page but don't fit any other category (such as payment or tax plugins)
    /// </summary>
    public partial interface IMiscPlugin : IPlugin
    {
        /// <summary>
        /// Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);
    }
}
