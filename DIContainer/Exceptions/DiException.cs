using System;
using System.Runtime.Serialization;

namespace DIContainer.Exceptions
{
    [Serializable]
    // TODO: make it more specific
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
