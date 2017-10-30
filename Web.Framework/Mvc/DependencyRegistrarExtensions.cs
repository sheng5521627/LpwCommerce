using Autofac;
using Core.Data;
using Core.Infrastructure.DependencyManagement;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Framework.Mvc
{
    public static class DependencyRegistrarExtensions
    {
        /// <summary>
        /// Register custom DataContext for a plugin
        /// </summary>
        /// <typeparam name="T">Class implementing IDbContext</typeparam>
        /// <param name="dependencyRegistrar">Dependency registrar</param>
        /// <param name="builder">Builder</param>
        /// <param name="contextName">Context name</param>
        public static void RegisterPluginDataContext<T>(this IDependencyRegistrar dependencyRegistrar,
            ContainerBuilder builder, string contextName)
             where T : IDbContext
        {
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                builder.Register(c => (IDbContext)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerLifetimeScope();

                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { dataProviderSettings.DataConnectionString }))
                    .InstancePerLifetimeScope();
            }
            else
            {
                builder.Register(c => (IDbContext)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataSettings>().DataConnectionString }))
                    .Named<IDbContext>(contextName)
                    .InstancePerLifetimeScope();

                builder.Register(c => (T)Activator.CreateInstance(typeof(T), new object[] { c.Resolve<DataSettings>().DataConnectionString }))
                    .InstancePerLifetimeScope();
            }
        }
    }
}