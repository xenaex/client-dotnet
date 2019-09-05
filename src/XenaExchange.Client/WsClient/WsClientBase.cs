using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Websocket.Client;
using XenaExchange.Client.Serialization;
using XenaExchange.Client.WsClient.Common;
using XenaExchange.Client.WsClient.Interfaces;
using XenaExchange.Client.WsClient.Interfaces.Exceptions;

namespace XenaExchange.Client.WsClient
{
    public abstract class WsClientBase : IWsClient
    {
        protected const string PingMsg = "{\"35\":\"0\"}";

        protected readonly WsClientOptionsBase Options;

        protected readonly ILogger Logger;

        protected readonly ISerializer Serializer;

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        private CancellationTokenSource _pingCts;

        protected readonly WebsocketClient WsClient;

        protected WsClientBase(WsClientOptionsBase options, ISerializer serializer, ILogger logger)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            WsClient = new WebsocketClient(new Uri(Options.Uri));

            WsClient.IsReconnectionEnabled = false;
            WsClient.ReconnectTimeoutMs = 0;
            WsClient.ErrorReconnectTimeoutMs = 0;

            WsClient.MessageReceived
                .Select(m => Observable.FromAsync(async () => await OnRawMessage(m).ConfigureAwait(false)))
                .Concat()
                .Subscribe();

            WsClient.DisconnectionHappened.Subscribe(OnDisconnect);
        }

        protected abstract Task OnMessage(IMessage message);

        protected virtual async Task ConnectBaseAsync()
        {
            // Infinite if not connected
            await WsClient.Start().ConfigureAwait(false);
            EnsureConnected();

            _pingCts = new CancellationTokenSource();
            PingAsync(_pingCts.Token);
        }

        public async Task SendCommandAsync<T>(T message)
            where T : IMessage
        {
            Validator.NotNull(nameof(message), message);

            var msg = Serializer.Serialize(message);
            await SendAsync(msg).ConfigureAwait(false);
        }

        public async Task CloseAsync()
        {
            if (WsClient == null)
                return;
            if (!WsClient.IsRunning)
                return;

            // Close is kind of Send so Stop message should be sent under lock.
            await _sendLock.WaitAsync().ConfigureAwait(false);
            if (!WsClient.IsRunning)
                return;

            await WsClient.Stop(WebSocketCloseStatus.NormalClosure, "").ConfigureAwait(false);
        }

        private void EnsureConnected()
        {
            if (!WsClient.IsRunning)
                throw new WsNotConnectedException();
        }

        private async Task SendAsync(string message)
        {
            EnsureConnected();
            await _sendLock.WaitAsync().ConfigureAwait(false);
            try
            {
                EnsureConnected();
                await WsClient.SendInstant(message).ConfigureAwait(false);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task PingAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(Options.PingInteral).ConfigureAwait(false);
                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    await SendAsync(PingMsg).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Failed to send ping message");
                }
            }
        }

        private async Task OnRawMessage(ResponseMessage message)
        {
            switch (message.MessageType)
            {
                case WebSocketMessageType.Binary:
                    Logger.LogDebug($"Received binary message {Encoding.UTF8.GetString(message.Binary)}");
                    return;
                case WebSocketMessageType.Close:
                    Logger.LogInformation($"Websocket {Options.Uri} closed");
                    return;
                case WebSocketMessageType.Text:
                    IMessage msg;
                    try
                    {
                        msg = Serializer.Deserialize(message.Text);
                    }
                    catch (MsgNotSupportedException ex)
                    {
                        Logger.LogWarning(ex.Message);
                        return;
                    }

                    if (msg is Heartbeat)
                        return;

                    await OnMessage(msg).ConfigureAwait(false);

                    return;
                default:
                    throw new NotSupportedException($"WebSocketMessageType {message.MessageType} not supported");
            }
        }

        private void OnDisconnect(DisconnectionType type)
        {
            var msg = $"Disconnected with type {type}";
            switch (type)
            {
                case DisconnectionType.Exit:
                case DisconnectionType.ByUser:
                    Logger.LogInformation(msg);
                    break;
                default:
                    Logger.LogError(msg);
                    break;
            }

            _pingCts?.Cancel();
            _pingCts = null;
        }

        public void Dispose()
        {
            WsClient?.Dispose();
        }
    }
}