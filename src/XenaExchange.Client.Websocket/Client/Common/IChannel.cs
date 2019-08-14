using System;
using System.Threading.Tasks;

namespace XenaExchange.Client.Websocket.Client.Common
{
    /// <summary>
    /// Channel interface which allows to transfer messages between different threads in async way.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    internal interface IChannel<T>
    {
        /// <summary>
        /// Sends message to channel.
        /// </summary>
        /// <param name="message">Message to send.</param>
         Task SendAsync(T message);

        /// <summary>
        /// Awaits and receives message from channel.
        /// </summary>
        /// <param name="timeout">Receiving timeout.</param>
        /// <returns>Message.</returns>         
        /// <exception cref="TimeoutException">Receive await timed out.</exception>
         Task<T> ReceiveAsync(TimeSpan timeout);
    }
}