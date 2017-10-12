using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class NopException : Exception
    {
        public NopException()
        {

        }

        public NopException(string message)
            : base(message)
        {

        }

        public NopException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {

        }

        public NopException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public NopException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
