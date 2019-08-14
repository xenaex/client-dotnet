using System;
using System.Runtime.Serialization;

namespace XenaExchange.Client.Websocket.Client.Interfaces.Exceptions
{
    public class DuplicateSubscriptionException : Exception
    {
        public DuplicateSubscriptionException()
        {
        }

        public DuplicateSubscriptionException(string message) : base(message)
        {
        }

        public DuplicateSubscriptionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}