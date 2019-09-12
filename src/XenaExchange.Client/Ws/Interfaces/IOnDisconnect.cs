using System;

namespace XenaExchange.Client.Ws.Interfaces
{
    /// <summary>
    /// Gives an opportunity to subscribe on Disconnection events with provided Websocket client inside callback. 
    /// </summary>
    /// <typeparam name="T">Websocket client type.</typeparam>
    public interface IOnDisconnect<T> where T : IWsClient
    {
        /// <summary>
        /// Stream for disconnection event (triggered after the connection was lost)
        /// </summary>
        IObservable<DisconnectInfo<T>> OnDisconnect { get; }
    }
}