using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest.Events
{
    public class EntityAdd : IConsumer<Student>
    {
        public void HandleEvent(Student eventMessage)
        {
            Console.WriteLine("事件发生");
        }
    }
}
