using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFacTest
{
    public abstract class BaseHandler
    {
        public virtual string Handler(string message)
        {
            return "Handled: " + message;
        }
    }

    public class HandlerA : BaseHandler
    {
        public override string Handler(string message)
        {
            return "[A]" + base.Handler(message);
        }
    }

    public class HandlerB : BaseHandler
    {
        public override string Handler(string message)
        {
            return "[B]" + base.Handler(message);
        }
    }

    public interface IHandlerFactory
    {
        T GetHandler<T>() where T : BaseHandler;
    }

    public class HandlerFactory : IHandlerFactory
    {
        public T GetHandler<T>() where T : BaseHandler
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }

    public class ConsumerA
    {
        private HandlerA _handlerA;
        public ConsumerA(HandlerA handlerA)
        {
            this._handlerA = handlerA;
        }

        public void DoWork()
        {
            Console.WriteLine(this._handlerA.Handler("ConsumerA"));
        }
    }

    public class ConsumerB
    {
        private HandlerB _handlerB;
        public ConsumerB(HandlerB handlerB)
        {
            this._handlerB = handlerB;
        }

        public void DoWork()
        {
            Console.WriteLine(this._handlerB.Handler("ConsumerB"));
        }
    }
}
