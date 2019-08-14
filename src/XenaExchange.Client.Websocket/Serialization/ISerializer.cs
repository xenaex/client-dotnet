using Google.Protobuf;

namespace XenaExchange.Client.Websocket.Serialization
{
    /// <summary>
    /// Xena messages serializer interface.
    /// </summary>
    public interface ISerializer
    {
        IMessage Deserialize(string message);
        
        string Serialize<T>(T message) where T : IMessage;
    }
}