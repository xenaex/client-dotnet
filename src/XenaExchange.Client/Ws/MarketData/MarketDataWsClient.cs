using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Api;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Ws.Common;
using XenaExchange.Client.Ws.Interfaces;
using XenaExchange.Client.Ws.Interfaces.Exceptions;

namespace XenaExchange.Client.Ws.MarketData
{
    /// <summary>
    /// Provides methods to Xena market data websocket API.
    /// </summary>
    public class MarketDataWsClient : WsClientBase, IMarketDataWsClient
    {
        private readonly MarketDataWsClientOptions _mdOptions;

        private readonly ConcurrentDictionary<string, Subscription> _subscriptions;

        private readonly Subject<DisconnectInfo<IMarketDataWsClient>> _disconnectedSubject = new Subject<DisconnectInfo<IMarketDataWsClient>>();

        public IObservable<DisconnectInfo<IMarketDataWsClient>> OnDisconnect => _disconnectedSubject.AsObservable();

        public MarketDataWsClient(MarketDataWsClientOptions options, IFixSerializer serializer, ILogger<MarketDataWsClient> logger)
            : base(options, serializer, logger)
        {
            _mdOptions = options ?? throw new ArgumentNullException(nameof(options));
            _subscriptions = new ConcurrentDictionary<string, Subscription>();
            WsClient.DisconnectionHappened.Subscribe(type =>
            {
                _subscriptions.Clear();
                _disconnectedSubject.OnNext(new DisconnectInfo<IMarketDataWsClient>(this, type));
            });
        }

        protected override async Task OnMessage(IMessage message)
        {
            switch (message)
            {
                case Logon logon:
                    Logger.LogInformation($"Logon response: {logon}");
                    break;
                case MarketDataRefresh mdRefresh:
                    await OnMdEventAsync(mdRefresh, mdRefresh.MDStreamId, false).ConfigureAwait(false);
                    break;
                case MarketDataRequestReject mdReject:
                    await OnMdEventAsync(mdReject, mdReject.MDStreamId, true).ConfigureAwait(false);
                    _subscriptions.TryRemove(mdReject.MDStreamId, out _);
                    break;
                default:
                    Logger.LogWarning($"Received unexpected md message of type {message.GetType().Name}: {message}");
                    break;
            }
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            await ConnectBaseAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<string> SubscribeCandlesAsync(
            string symbol,
            string timeframe,
            XenaMdWsHandler handler,
            long throttlingMs = 0)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.NotNullOrEmpty(nameof(timeframe), timeframe);
            Validator.OneOf(nameof(timeframe), timeframe, CandlesTimeframe.All);
            Validator.NotNull(nameof(handler), handler);
            Validator.GrThanOrEq(nameof(throttlingMs), throttlingMs, 0);
            
            return await SubscribeAsync("candles", handler, symbol, timeframe, throttlingMs).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<string> SubscribeDOMAggregatedAsync(
            string symbol,
            XenaMdWsHandler handler,
            long throttlingMs = 0,
            long aggregation = 0)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.NotNull(nameof(handler), handler);
            Validator.GrThanOrEq(nameof(throttlingMs), throttlingMs, 0);
            Validator.GrThanOrEq(nameof(aggregation), aggregation, 0);

            return await SubscribeAsync("DOM", handler, symbol, "aggregated", throttlingMs: throttlingMs, aggregation: aggregation).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<string> SubscribeMarketWatchAsync(XenaMdWsHandler handler)
        {
            Validator.NotNull(nameof(handler), handler);

            return await SubscribeAsync("market-watch", handler).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<string> SubscribeTradeReportsAsync(string symbol, XenaMdWsHandler handler, long throttlingMs = 0)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.NotNull(nameof(handler), handler);
            Validator.GrThanOrEq(nameof(throttlingMs), throttlingMs, 0);

            return await SubscribeAsync("trades", handler, symbol, throttlingMs: throttlingMs).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task Unsubscribe(string subscriptionId)
        {
            if (!_subscriptions.TryGetValue(subscriptionId, out var sub))
                throw new SubscriptionNotFoundException($"Subscription {subscriptionId} not found");

            var request = sub.Request.Clone();
            request.SubscriptionRequestType = SubscriptionRequestType.DisablePreviousSnapshot;
            await SendCommandAsync(request).ConfigureAwait(false);

            _subscriptions.TryRemove(subscriptionId, out var _);
        }

        private async Task<string> SubscribeAsync(
            string streamName,
            XenaMdWsHandler handler,
            string symbol = null,
            string streamPostfix = null,
            long throttlingMs = 0,
            long aggregation = 0)
        {
            var streamId = streamName;
            if (symbol != null)
                streamId += $":{symbol}";
            if (streamPostfix != null)
                streamId += $":{streamPostfix}";

            var request = new MarketDataRequest
            {
                MsgType = MsgTypes.MarketDataRequest,
                MDStreamId = streamId,
                SubscriptionRequestType = SubscriptionRequestType.SnapshotAndUpdates,
                ThrottleType = ThrottleType.OutstandingRequests,
                ThrottleTimeInterval = throttlingMs,
                ThrottleTimeUnit = ThrottleTimeUnit.Milliseconds,
                AggregatedBook = aggregation,
            };

            var sub = new Subscription(request, handler);
            if (!_subscriptions.TryAdd(streamId, sub))
                throw new DuplicateSubscriptionException($"Subscription {request.MDStreamId} done or in progress");

            await SendCommandAsync(request).ConfigureAwait(false);
            return streamId;
        }

        private async Task OnMdEventAsync(IMessage message, string mdStreamId, bool isReject)
        {
            Subscription sub;
            if (isReject)
                _subscriptions.TryRemove(mdStreamId, out sub);
            else
                _subscriptions.TryGetValue(mdStreamId, out sub);

            if (sub != null)
            {
                await sub.Handler(this, message).ConfigureAwait(false);
                return;
            }

            Logger.LogWarning($"Received {message.GetType().Name} from {mdStreamId}, but no handlers were found");
        }
    }
}