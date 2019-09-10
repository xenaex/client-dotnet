using Newtonsoft.Json;

namespace XenaExchange.Client.Serialization.Rest
 {
     /// <summary>
     /// Is used for the first step of deserialization in order to determine the type of message.
     /// </summary>
     public class RestMessageWrapper : IMessageWrapper

     {
         [JsonProperty("msgType")]
         public string MessageType { get; set; }
     }
 }