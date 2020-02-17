using System;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Microsoft.Extensions.Logging;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Ws.Interfaces;

namespace XenaExchange.Client.Examples.Ws
{
    public class MarketDataWsExample
    {
        private readonly IMarketDataWsClient _wsClient;
        private readonly ILogger _logger;
        public MarketDataWsExample(IMarketDataWsClient wsClient, ILogger<MarketDataWsExample> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _wsClient = wsClient ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await TestMarketDataAsync().ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _wsClient.CloseAsync().ConfigureAwait(false);
        }

        private async Task TestMarketDataAsync()
        {
            _wsClient.OnDisconnect.Subscribe(async info =>
            {
                // Don't reconnect here in a loop
                // OnDisconnect will fire on each WsClient.ConnectAsync() failure
                var reconnectInterval = TimeSpan.FromSeconds(5);
                try
                {
                    await Task.Delay(reconnectInterval).ConfigureAwait(false);
                    await info.WsClient.ConnectAsync().ConfigureAwait(false);
                    _logger.LogInformation("Reconnected");

                    // Reubscribe on all streams after reconnect
                    await SubscribeDOMAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Reconnect attempt failed, trying again after {reconnectInterval.ToString()}");
                }
            });

            await _wsClient.ConnectAsync().ConfigureAwait(false);

            // Subscribe on candles stream.
            // Async handler, throttling specified.
            // await SubscribeCandlesAsync().ConfigureAwait(false);

            // Subscribe on DOM:aggregated stream.
            // Sync handler, default throttling = 0 ms (every single update is sent).
            await SubscribeDOMAsync().ConfigureAwait(false);

            // Subscibe on trades
            // await SubscribeTradesAsync().ConfigureAwait(false);

            // Subscribe on market-watch
            // var streamId = await SubscribeMarketWatchAsync().ConfigureAwait(false);

            // Unsubscribe from market-watch stream
            // await Task.Delay(5000).ConfigureAwait(false);
            // await _wsClient.Unsubscribe(streamId).ConfigureAwait(false);
            // _logger.LogInformation($"Unsubscribed from {streamId}");
        }

        private async Task SubscribeCandlesAsync()
        {
            var symbol = "BTC/USDT";
            var timeframe = CandlesTimeframe.Timeframe1m;
            await _wsClient.SubscribeCandlesAsync(symbol, timeframe, async (client, message) =>
            {
                switch (message)
                {
                    case MarketDataRequestReject reject:
                        _logger.LogWarning($"Market data candles request for {symbol}:{timeframe} was rejected: {reject.RejectText}");
                        break;
                    case MarketDataRefresh refresh:
                        var updateType = refresh.IsSnapshot() ? "snapshot" : "update";
                        _logger.LogInformation($"Received candles {updateType}: {refresh}");
                        break;
                    default:
                        _logger.LogWarning($"Message of type {message.GetType().Name} not supported for candles stream");
                        break;
                }
                await Task.Delay(10).ConfigureAwait(false); // Any async action.
            }, throttlingMs: ThrottlingMs.Candles.Throttling1s).ConfigureAwait(false);
        }

        private async Task SubscribeDOMAsync()
        {
            var symbol = "BTC/USDT";
            await _wsClient.SubscribeDOMAggregatedAsync(symbol, (client, message) =>
            {
                switch (message)
                {
                    case MarketDataRequestReject reject:
                        _logger.LogWarning($"Market data DOM request for {symbol} was rejected: {reject.RejectText}");
                        break;
                    case MarketDataRefresh refresh:
                        var updateType = refresh.IsSnapshot() ? "snapshot" : "update";
                        _logger.LogInformation($"Received DOM {updateType}: {refresh}");
                        break;
                    default:
                        _logger.LogWarning($"Message of type {message.GetType().Name} not supported for DOM stream");
                        break;
                }
                return Task.CompletedTask;
            }, throttlingMs: ThrottlingMs.DOM.Throttling5s, aggregation: DOMAggregation.Aggregation5, depth: MDMarketDepth.Depth10).ConfigureAwait(false);
        }

        private async Task SubscribeTradesAsync()
        {
            var symbol = "XBTUSD";
            await _wsClient.SubscribeTradeReportsAsync(symbol, (client, message) =>
            {
                switch (message)
                {
                    case MarketDataRequestReject reject:
                        _logger.LogWarning($"Market data trades request for {symbol} was rejected: {reject.RejectText}");
                        break;
                    case MarketDataRefresh refresh:
                        var updateType = refresh.IsSnapshot() ? "snapshot" : "update";
                        _logger.LogInformation($"Received trades {updateType}: {refresh}");
                        break;
                    default:
                        _logger.LogWarning($"Message of type {message.GetType().Name} not supported for DOM stream");
                        break;
                }
                return Task.CompletedTask;
            }).ConfigureAwait(false);
        }

        private async Task<string> SubscribeMarketWatchAsync()
        {
            return await _wsClient.SubscribeMarketWatchAsync((client, message) =>
            {
                switch (message)
                {
                    case MarketDataRequestReject reject:
                        _logger.LogWarning($"Market data market-watch request was rejected: {reject.RejectText}");
                        break;
                    case MarketDataRefresh refresh:
                        // MarketDataRefresh for market-watch stream is always a snapshot
                        _logger.LogInformation($"Received market-watch snapshot: {refresh}");
                        break;
                    default:
                        _logger.LogWarning($"Message of type {message.GetType().Name} not supported for market-watch stream");
                        break;
                }
                return Task.CompletedTask;
            }).ConfigureAwait(false);
        }
    }
}