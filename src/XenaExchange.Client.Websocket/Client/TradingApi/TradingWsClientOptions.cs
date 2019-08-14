using System;
using System.Collections.Generic;

namespace XenaExchange.Client.Websocket.Client.TradingApi
{
    /// <summary>
    /// Trading websocket client options.
    /// </summary>
    public class TradingWsClientOptions : WsClientOptionsBase
    {
        /// <summary>
        /// Account ids to logon with.
        /// </summary>
        /// <value></value>
        public List<long> Accounts { get; set; }

        /// <summary>
        /// Api key.
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
        /// Api key secret. 
        /// </summary>
        /// <value></value>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Timeout to wait between Logon message was sent and Logon response was received.
        /// </summary>
        /// <returns></returns>
        public TimeSpan LogonResponseTimeout { get;set; } = TimeSpan.FromSeconds(5);
    }
}