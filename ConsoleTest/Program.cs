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
           var list =   typeof(B).GetCustomAttributes(true);

            foreach(var item in list)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }

    [AttributeUsage(AttributeTargets.Class,Inherited =true)]
    public class MyAttribute : Attribute
    {

    }

    [My]
    public class A
    {

    }

    public class B : A
    {

    }
}
