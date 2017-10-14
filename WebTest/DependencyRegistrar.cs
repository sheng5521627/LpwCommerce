using Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Core.Configuration;
using Core.Infrastructure;
using Autofac.Integration.Mvc;
using Core.Caching;

namespace WebTest
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>();

            if (config.RedisCachingEnabled)
            {
                builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").SingleInstance();
            }

            //builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_per_request").InstancePerLifetimeScope();
        }
    }
}