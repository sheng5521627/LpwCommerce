using ImageResizer;
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
            string s1 = Guid.NewGuid().ToString();
            string s2 = Guid.NewGuid().ToString();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "1.jpg");
            using (var b = new Bitmap(path))
            {
                using (var destStream = new MemoryStream())
                {                    
                    ImageBuilder.Current.Build(b, destStream, new ResizeSettings
                    {
                        Width = 200,
                        Height = 200,
                        Scale = ScaleMode.Both,
                        Quality = 100
                    });
                    var destBinary = destStream.ToArray();
                    File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "2.jpg"), destBinary);
                }
            }
        }
    }
}
