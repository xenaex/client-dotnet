using System;

namespace XenaExchange.Client.Websocket.Client
{
    /// <summary>
    /// Websocket base options.
    /// </summary>
    public class WsClientOptionsBase
    {
        public string Uri { get; set; }

        public TimeSpan PingInteral { get; set; } = TimeSpan.FromSeconds(5);
    }
}