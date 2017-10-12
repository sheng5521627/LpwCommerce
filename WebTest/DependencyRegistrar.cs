using Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Core.Configuration;
using Core.Infrastructure;
using Autofac.Integration.Mvc;

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
        }
    }
}