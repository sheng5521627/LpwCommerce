using Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Core.Configuration;
using Core.Infrastructure;
using WebSite.Controllers;
using Autofac.Core;
using Core.Caching;
using WebSite.Infrastructure.Installation;

namespace WebSite.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 2;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();
            builder.RegisterType<WidgetController>().WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
        }
    }
}