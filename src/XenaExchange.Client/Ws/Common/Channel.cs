using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace XenaExchange.Client.Ws.Common
{
    /// <summary>
    /// Channel allows to transfer messages between different threads in async way.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    public class Channel<T> : IChannel<T>
    {
        private readonly BufferBlock<T> _bufferBlock = new BufferBlock<T>(new DataflowBlockOptions { BoundedCapacity = 100 });

        /// <inheritdoc />
        public async Task SendAsync(T message)
        {
            await _bufferBlock.SendAsync(message).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T> ReceiveAsync(TimeSpan timeout)
        {
            return await _bufferBlock.ReceiveAsync(timeout).ConfigureAwait(false);
        }
    }
}