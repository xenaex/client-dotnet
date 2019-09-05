using System;
using System.Runtime.Serialization;

namespace XenaExchange.Client.Serialization
{
    /// <summary>
    /// Is thrown when received message isn't tracked in Fix.DeserializeMap.
    /// </summary>
    public class MsgNotSupportedException : Exception
    {
        public MsgNotSupportedException()
        {
        }

        public MsgNotSupportedException(string message) : base(message)
        {
        }

        public MsgNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MsgNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}