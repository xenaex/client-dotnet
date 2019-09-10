using Google.Protobuf;

namespace XenaExchange.Client.Serialization
{
    /// <summary>
    /// Xena messages serializer interface.
    /// </summary>
    public interface ISerializer
    {
        IMessage Deserialize(string message);

        T Deserialize<T>(string message);
        
        string Serialize<T>(T message) where T : IMessage;
    }
}