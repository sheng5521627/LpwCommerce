using Core.Data;
using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Framework;
using Web.Framework.Mvc;
using Web.Framework.Mvc.Routes;
using Web.Framework.Themes;
using FluentValidation.Mvc;
using Services.Tasks;
using StackExchange.Profiling;
using StackExchange.Profiling.Mvc;

namespace WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("favicon.ico");
            routes.Ignore("{resource}.axd/{*pathInfo}");

            //注册所有路由
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "WebSite.Controllers" });
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = false;

            EngineContext.Initialize(false);

            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();

            if (databaseInstalled)
            {
                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            }

            ModelMetadataProviders.Current = new NopMetadataProvider();

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

            if (databaseInstalled)
            {
                TaskManager.Instance.Initialze();
                TaskManager.Instance.Start();
            }

            GlobalFilters.Filters.Add(new ProfilingActionFilter());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            MiniProfiler.Start();
            HttpContext.Current.Items["nop.MiniProfilerStarted"] = true;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var miniProfilerStarted = HttpContext.Current.Items.Contains("Application_EndRequest")
                                      && Convert.ToBoolean(HttpContext.Current.Items["Application_EndRequest"]);
            if (miniProfilerStarted)
            {
                MiniProfiler.Stop();
            }
        }
    }
}
