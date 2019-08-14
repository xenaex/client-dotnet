using System;
using System.Runtime.Serialization;

namespace XenaExchange.Client.Websocket.Client.Interfaces.Exceptions
{
    public class LogonRejectedException : Exception
    {
        public LogonRejectedException()
        {
        }

        public LogonRejectedException(string message) : base(message)
        {
        }

        public LogonRejectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogonRejectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}