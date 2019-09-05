using Newtonsoft.Json;

namespace XenaExchange.Client.Messages
{
    /// <summary>
    /// Is used for the first step of deserialization in order to determine the type of message.
    /// </summary>
    public class MessageWrapper
    {
        [JsonProperty("35")]
        public string MessageType { get; set; }
    }
}