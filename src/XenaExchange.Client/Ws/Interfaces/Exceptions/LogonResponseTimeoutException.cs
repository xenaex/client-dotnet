using System;
using System.Runtime.Serialization;

namespace XenaExchange.Client.Ws.Interfaces.Exceptions
{
    public class LogonResponseTimeoutException : Exception
    {
        public LogonResponseTimeoutException()
        {
        }

        public LogonResponseTimeoutException(string message) : base(message)
        {
        }

        public LogonResponseTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogonResponseTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}