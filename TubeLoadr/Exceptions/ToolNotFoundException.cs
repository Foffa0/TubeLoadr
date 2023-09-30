using System;
using System.Runtime.Serialization;

namespace TubeLoadr.Exceptions
{
    public class ToolNotFoundException : Exception
    {
        public ToolNotFoundException()
        {
        }

        public ToolNotFoundException(string? message) : base(message)
        {
        }

        public ToolNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ToolNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
