using Google.Protobuf;
using Newtonsoft.Json;
using XenaExchange.Client.Websocket.Messages;

namespace XenaExchange.Client.Websocket.Serialization
{
    /// <summary>
    /// Implementation of serializer for Xena messages fix-protocol.
    /// </summary>
    public class FixSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public FixSerializer()
        {
            var fixDictionary = Fix.InitDictionary();
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new FixContractResolver(fixDictionary),
            };
        }

        public IMessage Deserialize(string messageJson)
        {
            // Determine the type of incoming message.
            var wrapper = JsonConvert.DeserializeObject<MessageWrapper>(messageJson);
            var known = Fix.DeserializeMap.TryGetValue(wrapper.MessageType, out var messageType);
            if (!known)
                throw new MsgNotSupportedException($"Message {messageJson} not supported.");

            // Deserialize message into a concrete type.
            var message = JsonConvert.DeserializeObject(messageJson, messageType, _jsonSerializerSettings);
            return (IMessage)message;
        }

        public string Serialize<T>(T message) where T : IMessage
        {
            return JsonConvert.SerializeObject(message, _jsonSerializerSettings);
        }
    }
}