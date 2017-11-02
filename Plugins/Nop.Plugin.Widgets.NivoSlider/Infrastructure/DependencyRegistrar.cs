using Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Configuration;
using Core.Infrastructure;
using Autofac;
using Lpw.Plugin.Widgets.NivoSlider.Controllers;

namespace Lpw.Plugin.Widgets.NivoSlider.Infrastructure
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
            builder.RegisterType<WidgetsNivoSliderController>();
        }
    }
}