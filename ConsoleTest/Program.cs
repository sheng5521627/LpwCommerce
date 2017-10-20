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
using Services.Events;
using ConsoleTest.Settings;
using ConsoleTest.Stores;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotePost q = new RemotePost();
            EngineContext.Initialize(false);

            List<A> list1 = new List<A>() { new A() { Id = 1, Name = "a" }, new ConsoleTest.A() { Id = 2, Name = "b" } };
            List<B> list2 = new List<ConsoleTest.B>() { new ConsoleTest.B() { EntityId = 1, EntityName = "a" }, new ConsoleTest.B() { EntityId = 2, EntityName = "b" } };

            var query = from a in list1
                        join b in list2
                        on new { id = a.Id, name = a.Name } equals new { id = b.EntityId, name = b.EntityName } into ab
                        from b in ab.DefaultIfEmpty()
                        select a;
            var list = query.ToList();
            foreach(var item in list)
            {

            }

            Console.ReadLine();
        }
    }
    

    public class A
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class B
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }
    }
}
