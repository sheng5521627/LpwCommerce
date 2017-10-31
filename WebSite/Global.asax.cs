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
        }
    }
}
