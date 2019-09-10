using XenaExchange.Client.Messages;

namespace XenaExchange.Client.Serialization.Fix
{
    /// <summary>
    /// Uses fix tags (from provided fix dictionary) instead of regular json names.
    /// </summary>
    public class FixSerializer : SerializerBase<FixMessageWrapper>, IFixSerializer
    {
        public FixSerializer() : base(MessagesReflection.FixJsonNames())
        {
        }
    }
}