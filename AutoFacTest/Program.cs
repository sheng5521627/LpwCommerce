using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFacTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Type type = typeof(A);
            Type type1 = typeof(D);
            var builder = new ContainerBuilder();
            builder.RegisterType(type).As(type.FindInterfaces((t, c) => 
            {
                return t.IsGenericType && ((Type)c).IsAssignableFrom(t.GetGenericTypeDefinition());
            },typeof(IA<>))).InstancePerDependency();

            builder.RegisterType(type1).As(type1.FindInterfaces((t, c) =>
            {
                return t.IsGenericType && ((Type)c).IsAssignableFrom(t.GetGenericTypeDefinition());
            }, typeof(IA<>))).InstancePerDependency();
            var container = builder.Build();
            var list = container.Resolve<IEnumerable<IA<C>>>().ToArray();

            Console.ReadLine();
        }
    }

    public interface IA<T>
    {

    }
    public interface IB<T>: IA<T>
    {

    }
    public class A : IA<B>,IA<C>
    {

    }
    public class B
    {

    }

    public class C
    {

    }

    public class D : IA<C>
    {

    }
}
