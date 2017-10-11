using Core.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var nopConfi = ConfigurationManager.GetSection("NopConfig") as NopConfig; 

            Console.WriteLine(nopConfi.IgnoreStartupTasks);
            Console.ReadLine();
        }
    }
}
