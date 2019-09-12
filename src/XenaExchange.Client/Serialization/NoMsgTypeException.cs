using System;
using System.Runtime.Serialization;

namespace XenaExchange.Client.Serialization
{
    /// <summary>
    /// Received Xena message without msgType.
    /// </summary>
    public class NoMsgTypeException : Exception
    {
        public NoMsgTypeException()
        {
        }

        protected NoMsgTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NoMsgTypeException(string message) : base(message)
        {
        }

        public NoMsgTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}