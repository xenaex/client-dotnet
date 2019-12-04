using System;

namespace XenaExchange.Client.Ws
{
    /// <summary>
    /// Websocket base options.
    /// </summary>
    public class WsClientOptionsBase
    {
        public string Uri { get; set; }

        public TimeSpan PingInterval { get; set; } = TimeSpan.FromSeconds(5);

        public TimeSpan CheckHeartbeatsInterval { get; set; } = TimeSpan.FromMilliseconds(500);

        public TimeSpan HandleMessageWarnThreshold { get; set; } = TimeSpan.FromMinutes(2);

        public int BufferWarnThreshold { get; set; } = 400;
    }
}