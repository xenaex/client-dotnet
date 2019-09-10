using Websocket.Client;
using XenaExchange.Client.Ws.Interfaces;

namespace XenaExchange.Client.Ws
{
    /// <summary>
    /// Wrapper over DisconnectionType with WsClient object provided.
    /// Is returned on every disconnection event.
    /// </summary>
    /// <typeparam name="T">Websocket client type.</typeparam>
    public class DisconnectInfo<T> where T : IWsClient
    {
        /// <summary>
        /// Websocket client.
        /// </summary>
        public T WsClient { get; }

        /// <summary>
        /// Disconnection type.
        /// </summary>
        public DisconnectionType DisconnectType { get; }

        public DisconnectInfo(T wsClient, DisconnectionType disconnectType)
        {
            WsClient = wsClient;
            DisconnectType = disconnectType;
        }
    }
}