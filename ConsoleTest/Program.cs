using Core;
using Core.Configuration;
using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Core.Caching;
using System.Threading;
using Core.ComponentModel;
using Web.Framework;
using Data;
using Core.Domain.Stores;
using Core.Data;
using System.Data.Entity;
using Services.Helpers;
using Core.Domain.Blogs;
using Services.Configuration;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineContext.Initialize(false);

            A a = EngineContext.Current.ContainnerManager.ResolveUnregistered<A>();
            Console.Write(EngineContext.Current.ContainnerManager.IsRegistered<A>());

            Console.ReadLine();
        }
    }

    public class A
    {

    }
}
