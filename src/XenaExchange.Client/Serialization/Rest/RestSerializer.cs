using XenaExchange.Client.Messages;

namespace XenaExchange.Client.Serialization.Rest
{
    /// <summary>
    /// Uses json_name from protobuf options instead of regular ones.
    /// </summary>
    public class RestSerializer : SerializerBase<RestMessageWrapper>, IRestSerializer
    {
        public RestSerializer() : base(MessagesReflection.JsonNames())
        {
        }
    }
}