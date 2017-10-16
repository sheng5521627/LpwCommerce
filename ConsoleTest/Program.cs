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
using Web.Framework;
using Data;
using Core.Domain.Stores;
using Core.Data;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineContext.Initialize(false);

            var flag = EngineContext.Current.ContainnerManager.IsRegistered<Store>();
            var a = EngineContext.Current.Resolve<A>();
            

            Console.ReadLine();
        }
    }

    public class A
    {
        private IRepository<Store> _repository;
        public A(IRepository<Store> repository)
        {
            _repository = repository;
            var list = _repository.GetById(11);
        }
    }
}
