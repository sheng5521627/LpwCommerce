using Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Core.Configuration;
using Core.Infrastructure;
using Core.Data;
using Data;
using System.Web;
using Core.Fakes;
using Core;
using Services.Helpers;
using Autofac.Integration.Mvc;
using Core.Caching;
using Services.Events;

namespace Web.Framework
{
    public class DependendencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 0;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            #region httpcontext 依赖注入
            builder.Register(c => HttpContext.Current != null ? (new HttpContextWrapper(HttpContext.Current)) as HttpContextBase : (new FakeHttpContext("~/")) as HttpContextBase)
                .As<HttpContextBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().InstancePerLifetimeScope();

            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            builder.RegisterType<UserAgentHelper>().As<IUserAgentHelper>().InstancePerLifetimeScope();

            #endregion

            #region 数据层的依赖注入

            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataProviderSettings).As<DataSettings>();
            builder.Register(c => new EfDataProviderManager(c.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataProviderSettings);
                var dataProvider = efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IDbContext>(c => new NopObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            }
            else
            {
                builder.Register<IDbContext>(c => new NopObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerLifetimeScope();
            }
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            #endregion

            //controller
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            #region 缓存依赖注入

            if (config.RedisCachingEnabled)
            {
                builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_static").SingleInstance();
            }
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("nop_cache_per_request").InstancePerLifetimeScope();

            #endregion

            #region 事件依赖注入

            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>));
            foreach(var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) => {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>))).InstancePerLifetimeScope();
            }
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
            #endregion
        }
    }
}
