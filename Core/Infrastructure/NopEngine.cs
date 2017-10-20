using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Infrastructure.DependencyManagement;
using Autofac;
using System.Web.Mvc;
using Autofac.Integration.Mvc;

namespace Core.Infrastructure
{
    public class NopEngine : IEngine
    {
        private ContainerManager _containerManager;

        /// <summary>
        /// 
        /// </summary>
        public ContainerManager ContainnerManager
        {
            get
            {
                return this._containerManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(NopConfig config)
        {
            RegisterDependencies(config);
        }

        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="config"></param>
        protected virtual void RegisterDependencies(NopConfig config)
        {
            var builder = new ContainerBuilder();

            var typeFinder = new AppDomainTypeFinder();
            //var typeFinder = new WebAppTypeFinder();
            builder = new ContainerBuilder();
            builder.RegisterInstance(config).As<NopConfig>().SingleInstance();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
            {
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            }
            drInstances = drInstances.OrderBy(m => m.Order).ToList();
            foreach (var drInstance in drInstances)
            {
                drInstance.Register(builder, typeFinder, config);
            }

            var container = builder.Build();
            this._containerManager = new ContainerManager(container);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainnerManager.Resolve(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return ContainnerManager.Resolve<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainnerManager.ResolveAll<T>();
        }
    }
}
