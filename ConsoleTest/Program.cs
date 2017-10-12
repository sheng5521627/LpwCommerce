using Core.Configuration;
using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineContext.Initialize(false);

            bool flag = EngineContext.Current.ContainnerManager.IsRegistered<IPerson>();

            var o = EngineContext.Current.ResolveAll<IPerson>();

            foreach(var item in o)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }

    public class A
    {
        public string Name { get; set; }
    }

    public class B
    {

    }
}
