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

        public Student(NopConfig config)
        {

        }
    }

    public class Teacher : IPerson
    {

    }
}
