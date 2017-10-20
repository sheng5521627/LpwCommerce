using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFacTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new SettingsSource());

            var container = builder.Build();
            container.Resolve<MySettings>();
            Console.ReadLine();
        }
    }

    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod("BuildRegistration", BindingFlags.NonPublic | BindingFlags.Static);
        public bool IsAdapterForIndividualComponents
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        /// <summary>
        /// 创建注册组件
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <returns></returns>
        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder.ForDelegate((c, p) =>
            {
                return new TSettings();
            })
            .InstancePerLifetimeScope()
            .CreateRegistration();
        }
    }

    public class MySettings :ISettings
    {

    }
}
