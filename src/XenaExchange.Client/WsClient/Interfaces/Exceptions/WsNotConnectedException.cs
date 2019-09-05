using System;
using System.Runtime.Serialization;

namespace XenaExchange.Client.WsClient.Interfaces.Exceptions
{
    public class WsNotConnectedException : Exception
    {
        public WsNotConnectedException()
        {
        }

        public WsNotConnectedException(string message) : base(message)
        {
        }

        public WsNotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WsNotConnectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}