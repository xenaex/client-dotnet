using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Websocket.Client;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Signature;
using XenaExchange.Client.Ws.Common;
using XenaExchange.Client.Ws.Interfaces;
using XenaExchange.Client.Ws.Interfaces.Exceptions;

namespace XenaExchange.Client.Ws.TradingApi
{
    /// <summary>
    /// Provides methods to Xena trading websocket API.
    /// </summary>
    public class TradingWsClient : XenaWsClientBase, ITradingWsClient
    {
        private readonly TradingWsClientOptions _tradingOptions;

        private IChannel<Logon> _logonChannel;

        private XenaTradingWsHandler _generalHandler;

        private readonly ConcurrentDictionary<Type, Delegate> _handlers;

        private readonly Subject<DisconnectInfo<ITradingWsClient>> _disconnectedSubject = new Subject<DisconnectInfo<ITradingWsClient>>();

        public IObservable<DisconnectInfo<ITradingWsClient>> OnDisconnect => _disconnectedSubject.AsObservable();

        public TradingWsClient(TradingWsClientOptions options, IFixSerializer serializer, ILogger<TradingWsClient> logger)
            : base(options, serializer, logger)
        {
            _tradingOptions = options ?? throw new ArgumentNullException(nameof(options));
            _handlers = new ConcurrentDictionary<Type, Delegate>();
        }

        /// <inheritdoc />
        public async Task<Logon> ConnectAndLogonAsync()
        {
            await base.ConnectBaseAsync().ConfigureAwait(false);
            return await LogonAsync().ConfigureAwait(false);
        }

        protected override async Task OnMessage(IMessage message)
        {
            if (message is Logon logon)
            {
                var logonChannel = _logonChannel;
                if (logonChannel != null)
                    await logonChannel.SendAsync(logon).ConfigureAwait(false);

                return;
            }

            var tasks = new List<Task>();
            var generalHandler = _generalHandler;

            if (generalHandler != null)
                tasks.Add(generalHandler(this, message));
            if (_handlers.TryGetValue(message.GetType(), out var handler))
                tasks.Add(RouteToHandlerAsync(message, handler));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        protected override void OnDisconnectBase(DisconnectionType type)
        {
            _disconnectedSubject.OnNext(new DisconnectInfo<ITradingWsClient>(this, type));
        }

        /// <inheritdoc />
        public void Listen(XenaTradingWsHandler handler)
        {
            Validator.NotNull(nameof(handler), handler);

            if (Interlocked.CompareExchange(ref _generalHandler, handler, null) != null)
                throw new DuplicateSubscriptionException("Already subscribed on all messages");
        }

        /// <inheritdoc />
        public void Listen<T>(XenaTradingWsHandler<T> handler)
            where T : IMessage
        {
            Validator.NotNull(nameof(handler), handler);

            if (!_handlers.TryAdd(typeof(T), handler))
                throw new DuplicateSubscriptionException($"Already subscribed on {typeof(T).Name} events");
        }

        /// <inheritdoc />
        public void RemoveListener<T>()
            where T : IMessage
        {
            _handlers.TryRemove(typeof(T), out var _);
        }

        /// <inheritdoc />
        public async Task CollapsePositionsAsync(ulong accountId, string symbol, string requestId = null)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            if (requestId != null)
                Validator.NotNullOrEmpty(nameof(requestId), requestId);

            var request = new PositionMaintenanceRequest(accountId, symbol, requestId);
            await SendCommandAsync(request).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task AccountStatusReportAsync(ulong accountId, string requestId = null)
        {
            var request = new AccountStatusReportRequest
            {
                MsgType = MsgTypes.AccountStatusReportRequest,
                Account = accountId,
                AccountStatusRequestId = requestId.Proto(),
            };
            await SendCommandAsync(request).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task GetOrdersAndFillsAsync(ulong accountId, string requestId = null)
        {
            var request = new OrderStatusRequest
            {
                MsgType = MsgTypes.OrderMassStatusRequest,
                Account = accountId,
                MassStatusReqId = requestId.Proto(),
            };
            await SendCommandAsync(request).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task GetPositionsAsync(ulong accountId, string requestId = null)
        {
            var request = new PositionsRequest
            {
                MsgType = MsgTypes.RequestForPositions,
                Account = accountId,
                PosReqId = requestId.Proto(),
            };
            await SendCommandAsync(request).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task OrderMassCancelAsync(
            ulong account,
            string clOrdId,
            string symbol = null,
            string side = null,
            string positionEffect = PositionEffect.Default)
        {
            var request = new OrderMassCancelRequest(account, clOrdId, symbol, side, positionEffect);
            request.Validate();

            await SendCommandAsync(request).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task NewMarketOrderAsync(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0)
        {
            var command = OrderExtensions.NewMarketOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice);

            await SendCommandAsync(command).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task NewLimitOrderAsync(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal price,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0)
        {
            var command = OrderExtensions.NewLimitOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                price,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice,
                trailingOffset,
                capPrice);

            await SendCommandAsync(command).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task NewStopOrderAsync(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal stopPx,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0)
        {
            var command = OrderExtensions.NewStopOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                stopPx,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice,
                trailingOffset,
                capPrice);

            await SendCommandAsync(command).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task NewMarketIfTouchOrderAsync(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal stopPx,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0)
        {
            var command = OrderExtensions.NewMarketIfTouchOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                stopPx,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice,
                trailingOffset,
                capPrice);

            await SendCommandAsync(command).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task CancelOrderByClOrdIdAsync(string clOrdId, string origClOrdId, string symbol, string side, ulong account)
        {
            Validator.NotNullOrEmpty(nameof(clOrdId), clOrdId);
            Validator.NotNullOrEmpty(nameof(origClOrdId), origClOrdId);
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.OneOf(nameof(side), side, Side.All);

            var command = new OrderCancelRequest
            {
                MsgType = MsgTypes.OrderCancelRequest,
                ClOrdId = clOrdId,
                OrigClOrdId = origClOrdId,
                Symbol = symbol,
                Side = side,
                Account = account,
                TransactTime = Functions.NowUnixNano(),
            };

            await SendCommandAsync(command).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task CancelOrderByOrderIdAsync(string clOrdId, string orderId, string symbol, string side, ulong account)
        {
            Validator.NotNullOrEmpty(nameof(clOrdId), clOrdId);
            Validator.NotNullOrEmpty(nameof(orderId), orderId);
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.OneOf(nameof(side), side, Side.All);

            var command = new OrderCancelRequest
            {
                MsgType = MsgTypes.OrderCancelRequest,
                ClOrdId = clOrdId,
                OrderId = orderId,
                Symbol = symbol,
                Side = side,
                Account = account,
                TransactTime = Functions.NowUnixNano(),
            };

            await SendCommandAsync(command).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task CancelReplaceOrderAsync(OrderCancelReplaceRequest request)
        {
            await SendCommandAsync(request).ConfigureAwait(false);
        }

        private async Task<Logon> LogonAsync()
        {
            var nonce = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() * 1000000;
            var authPayload = $"AUTH{nonce}";
            var signature = XenaSignature.Sign(_tradingOptions.ApiSecret, authPayload);
            var logon = new Logon
            {
                MsgType = MsgTypes.Logon,
                Username = _tradingOptions.ApiKey,
                SendingTime = nonce,
                RawData = authPayload,
                Password = signature,
            };
            logon.Account.AddRange(_tradingOptions.Accounts.Select(p => (ulong)p));

            _logonChannel = new Channel<Logon>();
            await SendCommandAsync(logon).ConfigureAwait(false);

            Logon logonResponse;
            try
            {
                logonResponse = await _logonChannel.ReceiveAsync(_tradingOptions.LogonResponseTimeout);
            }
            catch (TimeoutException ex)
            {
                const string msg = "Logon response timed out";
                Logger.LogError(msg);
                throw new LogonResponseTimeoutException(msg, ex);
            }
            if (!string.IsNullOrWhiteSpace(logonResponse.RejectText))
                throw new LogonRejectedException(logonResponse.RejectText);

            _logonChannel = null;
            Logger.LogInformation("Logon successful");

            return logonResponse;
        }

        private async Task RouteToHandlerAsync(IMessage msg, Delegate handler)
        {
            switch (msg)
            {
                case OrderMassStatusResponse orderMassStatusResponse:
                    await HandleConcreteAsync(orderMassStatusResponse, handler).ConfigureAwait(false);
                    break;

                case BalanceIncrementalRefresh balanceIncrementalRefresh:
                    await HandleConcreteAsync(balanceIncrementalRefresh, handler).ConfigureAwait(false);
                    break;

                case BalanceSnapshotRefresh balanceSnapshotRefresh:
                    await HandleConcreteAsync(balanceSnapshotRefresh, handler).ConfigureAwait(false);
                    break;

                case MarginRequirementReport marginRequirementReport:
                    await HandleConcreteAsync(marginRequirementReport, handler).ConfigureAwait(false);
                    break;

                case PositionReport positionReport:
                    await HandleConcreteAsync(positionReport, handler).ConfigureAwait(false);
                    break;

                case MassPositionReport massPositionReport:
                    await HandleConcreteAsync(massPositionReport, handler).ConfigureAwait(false);
                    break;

                case PositionMaintenanceReport positionMaintenanceReport:
                    await HandleConcreteAsync(positionMaintenanceReport, handler).ConfigureAwait(false);
                    break;

                case ExecutionReport executionReport:
                    await HandleConcreteAsync(executionReport, handler).ConfigureAwait(false);
                    break;

                case Reject reject:
                    await HandleConcreteAsync(reject, handler).ConfigureAwait(false);
                    break;

                case OrderCancelReject orderCancelReject:
                    await HandleConcreteAsync(orderCancelReject, handler).ConfigureAwait(false);
                    break;

                case OrderMassCancelReport orderMassCancelReport:
                    await HandleConcreteAsync(orderMassCancelReport, handler).ConfigureAwait(false);
                    break;

                default:
                    Logger.LogError($"RouteToHandlerAsync not implemented for {msg.GetType().Name}");
                    break;
            }
        }

        private async Task HandleConcreteAsync<T>(T message, Delegate handler)
            where T : IMessage
        {
            var concreteHandler = (XenaTradingWsHandler<T>)handler;
            if (concreteHandler != null)
                await concreteHandler(this, message).ConfigureAwait(false);
        }
    }
}