using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DIContainer.Exceptions
{
    [Serializable]
    public class DiException : Exception
    {
        public DiException()
        {
        }

        public DiException(string message) : base(message)
        {
        }

        public DiException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
