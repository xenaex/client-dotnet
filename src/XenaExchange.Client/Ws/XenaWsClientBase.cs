using System;
using System.Threading.Tasks;
using Api;
using Microsoft.Extensions.Logging;
using Google.Protobuf;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Serialization;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Ws.Interfaces;

namespace XenaExchange.Client.Ws
{
    public abstract class XenaWsClientBase : WsClientBase, IWsClient
    {
        protected const string HeartbeatMsg = "{\"35\":\"0\"}";

        protected readonly ISerializer Serializer;

        protected XenaWsClientBase(WsClientOptionsBase options, IFixSerializer serializer, ILogger logger)
            : base(options, logger)
        {
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        protected abstract Task OnMessageAsync(IMessage message);

        protected override string PingMessage => HeartbeatMsg;

        protected override async Task OnTextMessageAsync(string message)
        {
            IMessage msg;
            try
            {
                msg = Serializer.Deserialize(message);
            }
            catch (MsgNotSupportedException ex)
            {
                Logger.LogWarning(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to deserialize message {message}: {ex.Message}");
                throw;
            }

            if (msg is Heartbeat)
                return;

            await OnMessageAsync(msg).ConfigureAwait(false);
        }

        public async Task SendCommandAsync<T>(T message)
            where T : IMessage
        {
            Validator.NotNull(nameof(message), message);

            var msg = Serializer.Serialize(message);
            await SendAsync(msg).ConfigureAwait(false);
        }
    }
}