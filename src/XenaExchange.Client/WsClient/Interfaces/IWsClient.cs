using System;
using System.Threading.Tasks;
using Google.Protobuf;

namespace XenaExchange.Client.WsClient.Interfaces
{
    /// <summary>
    /// Base websocket interface.
    /// </summary>
    public interface IWsClient : IDisposable
    {
        /// <summary>
        /// Sends Xena protobuf message to websocket.
        /// It acts under SemaphoreSlim lock.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <typeparam name="T">Message type.</typeparam>
        Task SendCommandAsync<T>(T message) where T : IMessage;

        /// <summary>
        /// Closes websocket connection with NormalClosure type and empty reason.
        /// </summary>
        Task CloseAsync();
    }
}