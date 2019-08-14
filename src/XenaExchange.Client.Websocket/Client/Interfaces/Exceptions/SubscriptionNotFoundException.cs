using System;
using System.Runtime.Serialization;

namespace XenaExchange.Client.Websocket.Client.Interfaces.Exceptions
{
    public class SubscriptionNotFoundException : Exception
    {
        public SubscriptionNotFoundException()
        {
        }

        public SubscriptionNotFoundException(string message) : base(message)
        {
        }

        public SubscriptionNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SubscriptionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}