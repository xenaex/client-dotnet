using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Websocket.Client;
using XenaExchange.Client.Ws.Interfaces.Exceptions;

namespace XenaExchange.Client.Ws
{
    public abstract class WsClientBase
    {
        protected readonly WsClientOptionsBase Options;

        protected readonly ILogger Logger;

        private readonly SemaphoreSlim _wsLock = new SemaphoreSlim(1, 1);

        private CancellationTokenSource _pingCts;

        protected WebsocketClient WsClient;

        private long _lastSentTs;
        private long _lastReceivedTs;

        protected WsClientBase(WsClientOptionsBase options, ILogger logger)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract string PingMessage { get; }
        protected abstract Task OnTextMessage(string message);
        protected abstract void OnDisconnectBase(DisconnectionType type);

        protected async Task ConnectBaseAsync()
        {
            if (WsClient == null)
            {
                await _wsLock.WaitAsync().ConfigureAwait(false);
                if (WsClient == null)
                    InitWsClient();

                _wsLock.Release();
            }

            await WsClient.Start().ConfigureAwait(false);
            EnsureConnected();

            _pingCts = new CancellationTokenSource();
            HeartbeatsAsync(_pingCts.Token);
        }

        protected async Task SendAsync(string message)
        {
            EnsureConnected();
            await _wsLock.WaitAsync().ConfigureAwait(false);
            try
            {
                EnsureConnected();
                await WsClient.SendInstant(message).ConfigureAwait(false);
                MarkLastSentTs();
            }
            finally
            {
                _wsLock.Release();
            }
        }

        public async Task CloseAsync()
        {
            var client = WsClient;
            if (client?.IsRunning != true)
                return;

            // Close is kind of Send so Stop message should be sent under lock.
            await _wsLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!WsClient?.IsRunning != true)
                    return;

                await WsClient.Stop(WebSocketCloseStatus.NormalClosure, "").ConfigureAwait(false);
            }
            finally
            {
                _wsLock.Release();
            }
        }

        private void InitWsClient()
        {
            WsClient = new WebsocketClient(new Uri(Options.Uri))
            {
                IsReconnectionEnabled = false,
                ErrorReconnectTimeoutMs = 0,
                ReconnectTimeoutMs = 0
            };

            WsClient.MessageReceived
                .Select(m => Observable.FromAsync(async () => await HandleMessage(m).ConfigureAwait(false)))
                .Concat()
                .Subscribe();

            WsClient.DisconnectionHappened.Subscribe(HandleDisconnect);
        }

        private void EnsureConnected()
        {
            var client = WsClient;
            if (client?.IsRunning != true)
                throw new WsNotConnectedException();
        }

        private async Task HeartbeatsAsync(CancellationToken cancellationToken)
        {
            _lastSentTs = DateTime.UtcNow.Ticks;
            _lastReceivedTs = _lastSentTs;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(Options.CheckHeartbeatsInterval, cancellationToken).ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested)
                    return;

                var isActive = await CheckServerInactivityAsync().ConfigureAwait(false);
                if (isActive)
                    await SendPingIfNeededAsync().ConfigureAwait(false);
            }
        }

        private async Task<bool> CheckServerInactivityAsync()
        {
            var now = DateTime.UtcNow;
            var lastReceived = new DateTime(_lastReceivedTs, DateTimeKind.Utc);
            var sinceLastReceived = now - lastReceived;

            var disconnectTimeout = TimeSpan.FromTicks((long)(1.5 * Options.PingInterval.Ticks));
            if (sinceLastReceived <= disconnectTimeout)
                return true;

            Logger.LogError($"No new messages from server for {sinceLastReceived}, disconnecting");
            try
            {
                await ResetWsClientAsync().ConfigureAwait(false);
                Logger.LogDebug("WsClient Stopped.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to stop {Options.Uri} gracefully");
            }

            return false;
        }

        private async Task SendPingIfNeededAsync()
        {
            var now = DateTime.UtcNow;
            var lastSent = new DateTime(_lastSentTs, DateTimeKind.Utc);
            var sinceLastSent = now - lastSent;

            // Check if ping needed
            if (sinceLastSent < Options.PingInterval)
                return;

            // Send ping
            try
            {
                await SendAsync(PingMessage).ConfigureAwait(false);
                MarkLastSentTs();
            }
            catch (TaskCanceledException)
            {
                // Do nothing, task was canceled.
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to send ping message");
            }
        }

        private async Task HandleMessage(ResponseMessage message)
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
                    MarkLastReceivedTs();
                    await OnTextMessage(message.Text).ConfigureAwait(false);
                    return;
                default:
                    throw new NotSupportedException($"WebSocketMessageType {message.MessageType} not supported");
            }
        }

        private void HandleDisconnect(DisconnectionType type)
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

            OnDisconnectBase(type);
        }

        /// <summary>
        /// Null and dispose WsClient
        /// Dispose is needed cause in some cases WsClient is unable to detect that network is unavailable
        /// and Stop() doesn't really stop the client.
        /// </summary>
        private async Task ResetWsClientAsync()
        {
            if (WsClient == null)
                return;

            await _wsLock.WaitAsync().ConfigureAwait(false);
            var client = WsClient;
            if (client == null)
            {
                _wsLock.Release();
                return;
            }

            WsClient = null;
            _wsLock.Release();

            client.Dispose();
        }

        private void MarkLastSentTs()
        {
            _lastSentTs = DateTime.UtcNow.Ticks;
        }

        private void MarkLastReceivedTs()
        {
            _lastReceivedTs = DateTime.UtcNow.Ticks;
        }

        public void Dispose()
        {
            // TODO: isn't thread safe, could crush on app terminating.
            WsClient?.Dispose();
        }
    }
}