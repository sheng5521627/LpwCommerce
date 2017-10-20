using Core.Caching;
using Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public interface IPerson
    {

    }

    public class Student : IPerson
    {
        public string Name { get; set; }

        public Student()
        {

        }

        public Student(ICacheManager cacheManager, IPerson student)
        {
            
        }
    }

    public class Teacher : IPerson
    {
        public string Name { get; set; }

        public Teacher(string s)
        {
            Name = "a";
        }

        public Teacher (int i)
        {
            Name = "2";
        }
    }
}
