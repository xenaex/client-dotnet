using System;

namespace XenaExchange.Client.Ws
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