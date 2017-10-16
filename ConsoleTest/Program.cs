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

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineContext.Initialize(false);
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
            var list = _repository.GetById(1);

            Console.WriteLine(list.Name);
        }
    }
}
