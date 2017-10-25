using ImageResizer;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<List<int>> list1 = new List<List<int>>();
            List<int> list2 = new List<int>() {1, 2, 3, 4};

            for (int counter = 0; counter < (1 << list2.Count); ++counter)
            {
                var combination = new List<int>();
                for (int i = 0; i < list2.Count; ++i)
                {
                    if ((counter & (1 << i)) == 0)
                    {
                        combination.Add(list2[i]);
                    }
                }
                list1.Add(combination);
            }
            foreach (var item in list1)
            {
                Console.WriteLine(string.Join(",", item));
            }

            Console.ReadLine();

        }
    }
}
