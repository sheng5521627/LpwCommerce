using Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Core.Configuration;
using Core.Infrastructure;
using Autofac.Core;
using Core.Caching;

namespace ConsoleTest
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
            builder.RegisterType<Student>().As<IPerson>().AsSelf().WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
            builder.RegisterType<Teacher>().As<IPerson>();
        }
    }
}
