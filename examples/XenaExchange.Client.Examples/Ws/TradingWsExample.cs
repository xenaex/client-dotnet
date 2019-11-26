using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Microsoft.Extensions.Logging;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Ws.Interfaces;
using XenaExchange.Client.Ws.Interfaces.Exceptions;
using XenaExchange.Client.Ws.TradingApi;

namespace XenaExchange.Client.Examples.Ws
{
    public class TradingWsExample
    {
        private readonly ITradingWsClient _wsClient;

        private readonly TradingWsClientOptions _options;

        private readonly ILogger<TradingWsExample> _logger;

        private ulong SpotAccountId => (ulong)_options.Accounts[0];
        private ulong MarginAccountId => (ulong)_options.Accounts[1];

        public TradingWsExample(ITradingWsClient wsClient, TradingWsClientOptions options, ILogger<TradingWsExample> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _wsClient = wsClient ?? throw new ArgumentNullException(nameof(wsClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await TestTradingAsync().ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _wsClient.CloseAsync().ConfigureAwait(false);
        }

        private async Task TestTradingAsync()
        {
            _wsClient.OnDisconnect.Subscribe(async info =>
            {
                // Don't reconnect here in a loop
                // OnDisconnect will fire on each _wsClient.ConnectAndLogonAsync() failure
                var reconnectInterval = TimeSpan.FromSeconds(5);
                try
                {
                    await Task.Delay(reconnectInterval).ConfigureAwait(false);
                    var response = await info.WsClient.ConnectAndLogonAsync().ConfigureAwait(false);
                    _logger.LogInformation($"Reconnected, logon response: {response}");

                    // Don't call again Listen methods after reconnect
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reconnect attempt failed, trying again after {reconnectInterval.ToString()}");
                }
            });

            var logonResponse = await _wsClient.ConnectAndLogonAsync().ConfigureAwait(false);
            _logger.LogInformation($"Connected, logon response: {logonResponse}");

            // Subscriptions on both general and any concrete event are allowed but messages will be delivered to both handlers
            // If subcribe twice on any event DuplicateSubscriptionException will be thrown

            // General listener
            // _wsClient.Listen((client, msg) =>
            // {
            //     _logger.LogInformation($"General listener, received {msg.GetType().Name}: {msg}");
            //     return Task.CompletedTask;
            // });

            // Concrete listeners
            _wsClient.Listen<ExecutionReport>((client, er) =>
            {
                _logger.LogInformation($"Received execution report: {er}");
                return Task.CompletedTask;
            });
            _wsClient.Listen<BalanceSnapshotRefresh>((client, report) =>
            {
                _logger.LogInformation($"Received account status report: {report}");
                return Task.CompletedTask;
            });
            _wsClient.Listen<BalanceIncrementalRefresh>((client, report) =>
            {
                _logger.LogInformation($"Received account status update report: {report}");
                return Task.CompletedTask;
            });
            _wsClient.Listen<MassPositionReport>((client, report) =>
            {
                _logger.LogInformation($"Received mass position report: {report}");
                return Task.CompletedTask;
            });
            _wsClient.Listen<OrderMassStatusResponse>((client, response) =>
            {
                _logger.LogInformation($"Received order mass status report: {response}");
                return Task.CompletedTask;
            });
            // Subsribe twice and handle an exception.
            try
            {
                _wsClient.Listen<OrderMassStatusResponse>((client, response) =>
                {
                    _logger.LogInformation($"Received order mass status report: {response}");
                    return Task.CompletedTask;
                });
            }
            catch (DuplicateSubscriptionException)
            {
                // Handle duplicate.
            }

//            await _wsClient.AccountStatusReportAsync(MarginAccountId).ConfigureAwait(false);
//            await _wsClient.GetPositionsAsync(MarginAccountId).ConfigureAwait(false);
//            await _wsClient.CollapsePositionsAsync(MarginAccountId, "XBTUSD", Guid.NewGuid().ToString())
//                .ConfigureAwait(false);
//
//            await InfinitePlaceCancelAsync().ConfigureAwait(false);
//            await GetAllOrdersAndCancelAsync().ConfigureAwait(false);
//            await MarketOrderAsync().ConfigureAwait(false);
//            await LimitOrderAsync().ConfigureAwait(false);
//            await StopOrderAsync().ConfigureAwait(false);
//            await SltpGroupAsync().ConfigureAwait(false);
//            await StopLossForExistingPositionAsync().ConfigureAwait(false);
//            await TakeProfitForExistingPositionAsync().ConfigureAwait(false);
//            await CancelOrdersAsync().ConfigureAwait(false);
//            await ReplaceAsync().ConfigureAwait(false);
//            await SyncLimitOrderAsync().ConfigureAwait(false);
//            await GetOpenPositionsAsync().ConfigureAwait(false);
//            await GetBalancesAsync().ConfigureAwait(false);
            await OrderMassCancelAsync().ConfigureAwait(false);
        }

        private async Task InfinitePlaceCancelAsync()
        {
            _wsClient.RemoveListener<ExecutionReport>();
            _wsClient.RemoveListener<BalanceIncrementalRefresh>();
            var placeInterval = TimeSpan.FromMilliseconds(500);

            _wsClient.Listen<ExecutionReport>(async (client, er) =>
            {
                _logger.LogInformation($"ER {er.ClOrdId} ExecType {er.ExecType}");
                if (er.ExecType != ExecType.NewExec)
                    return;

                var cancelCommand = er.ToOrderCancelRequest(CommonFuncs.NewClOrdId("cancel-order"));
                try
                {
                    await client.SendCommandAsync(cancelCommand).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to cancel order {er.ClOrdId}: {ex.Message}");
                }
            });

            while (true)
            {
                var limitCommand = OrderExtensions.NewLimitOrder(CommonFuncs.NewClOrdId("limit-order"), "BTC/USDT", Side.Sell, 0.01M, SpotAccountId, 10500);
                try
                {
                    await _wsClient.SendCommandAsync(limitCommand).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to place limit order: {ex.Message}");
                }
                await Task.Delay(placeInterval).ConfigureAwait(false);
            }
        }

        private async Task MarketOrderAsync()
        {
            await _wsClient.NewMarketOrderAsync(CommonFuncs.NewClOrdId("market-order"), "BTC/USDT", Side.Sell, 0.01M, SpotAccountId).ConfigureAwait(false);

            var command = OrderExtensions.NewMarketOrder(CommonFuncs.NewClOrdId("market-order"), "BTC/USDT", Side.Sell, 0.01M, SpotAccountId);
            await _wsClient.SendCommandAsync(command).ConfigureAwait(false);
        }

        private async Task LimitOrderAsync()
        {
            await _wsClient.NewLimitOrderAsync(CommonFuncs.NewClOrdId("limit-order"), "BTC/USDT", Side.Sell, 0.01M, SpotAccountId, 10500).ConfigureAwait(false);

            var command = OrderExtensions.NewLimitOrder(CommonFuncs.NewClOrdId("limit-order"), "BTC/USDT", Side.Sell, 0.01M, SpotAccountId, 10500);
            await _wsClient.SendCommandAsync(command).ConfigureAwait(false);
        }

        private async Task StopOrderAsync()
        {
            await _wsClient.NewStopOrderAsync(CommonFuncs.NewClOrdId("stop-order"), "BTC/USDT", Side.Sell, 0.01M, SpotAccountId, 9500).ConfigureAwait(false);

            var command = OrderExtensions.NewStopOrder(CommonFuncs.NewClOrdId("stop-order"), "BTC/USDT", Side.Sell, 0.01M, SpotAccountId, 9500);
            await _wsClient.SendCommandAsync(command).ConfigureAwait(false);
        }

        private async Task SltpGroupAsync()
        {
            var command = OrderExtensions.NewLimitOrder(CommonFuncs.NewClOrdId("limit-sltp"), "XBTUSD", Side.Sell, 1M, MarginAccountId, 10600);
            command.AddTrailingStopLoss(500);
            command.AddTakeProfit(10000);
            await _wsClient.SendCommandAsync(command).ConfigureAwait(false);

            await _wsClient.NewLimitOrderAsync(
                CommonFuncs.NewClOrdId("limit-sltp"),
                "XBTUSD",
                Side.Sell,
                1M,
                MarginAccountId,
                10600,
                trailingOffset: 500,
                takeProfitPrice: 10000).ConfigureAwait(false);
        }

        private async Task StopLossForExistingPositionAsync()
        {
            var command = OrderExtensions.NewStopOrder(CommonFuncs.NewClOrdId("stop-order"), "XBTUSD", Side.Sell, 1M, MarginAccountId, 9500);
            command.ForPosition(12345);
            await _wsClient.SendCommandAsync(command).ConfigureAwait(false);

            command = OrderExtensions.NewStopOrder(CommonFuncs.NewClOrdId("stop-order"), "XBTUSD", Side.Sell, 1M, MarginAccountId, 9500, positionId: 12345);
            await _wsClient.SendCommandAsync(command).ConfigureAwait(false);
        }

        private async Task TakeProfitForExistingPositionAsync()
        {
            var command = OrderExtensions.NewLimitOrder(CommonFuncs.NewClOrdId("stop-order"), "XBTUSD", Side.Sell, 1M, MarginAccountId, 10000);
            command.ForPosition(12345);
            await _wsClient.SendCommandAsync(command).ConfigureAwait(false);
        }

        private async Task CancelOrdersAsync()
        {
            var ids = Enumerable.Range(0, 3).Select(i => CommonFuncs.NewClOrdId($"limit-{i}")).ToArray();

            // RemoveListener is here only to override previous ExecutionReport handler. There is no need to use it every time before Listen().
            _wsClient.RemoveListener<ExecutionReport>();
            _wsClient.Listen<ExecutionReport>(async (client, er) =>
            {
                _logger.LogInformation($"Received execution report: {er}");
                if (er.ExecType != ExecType.NewExec)
                    return;

                if (er.ClOrdId == ids[0])
                    await client.CancelOrderByOrderIdAsync(CommonFuncs.NewClOrdId("cancel-1"), er.OrderId, "BTC/USDT", Side.Buy, SpotAccountId).ConfigureAwait(false);
                if (er.ClOrdId == ids[1])
                    // Actually it is possible to cancel order by client id at any time. No need to wait for ExecutionReport confirmation.
                    await client.CancelOrderByClOrdIdAsync(CommonFuncs.NewClOrdId("cancel-2"), er.ClOrdId, "BTC/USDT", Side.Buy, SpotAccountId).ConfigureAwait(false);
                if (er.ClOrdId == ids[2])
                {
                    var cancelCmd = er.ToOrderCancelRequest(CommonFuncs.NewClOrdId("cancel-3"));
                    await client.SendCommandAsync(cancelCmd).ConfigureAwait(false);
                }
            });

            await _wsClient.NewLimitOrderAsync(ids[0], "BTC/USDT", Side.Buy, 0.01M, SpotAccountId, 10000).ConfigureAwait(false);
            await _wsClient.NewLimitOrderAsync(ids[1], "BTC/USDT", Side.Buy, 0.01M, SpotAccountId, 10000).ConfigureAwait(false);
            await _wsClient.NewLimitOrderAsync(ids[2], "BTC/USDT", Side.Buy, 0.01M, SpotAccountId, 10000).ConfigureAwait(false);
        }

        private async Task GetAllOrdersAndCancelAsync()
        {
            // RemoveListener is here only to override previous ExecutionReport handler. There is no need to use it every time before Listen().
            _wsClient.RemoveListener<ExecutionReport>();

            ExecutionReport[] orders = null;
            var ordersReceivedLock = new SemaphoreSlim(0, 1);

            _wsClient.RemoveListener<OrderMassStatusResponse>();
            _wsClient.Listen<OrderMassStatusResponse>((client, response) =>
            {
                _logger.LogInformation($"Received order mass status response {response.MassStatusReqId} " +
                                       $"with {response.Orders.Count.ToString()} orders");

                orders = response.Orders.ToArray();
                ordersReceivedLock.Release();
                return Task.CompletedTask;
            });

            await _wsClient.GetOrdersAndFillsAsync(SpotAccountId, Guid.NewGuid().ToString()).ConfigureAwait(false);
            await ordersReceivedLock.WaitAsync().ConfigureAwait(false);

            if (orders.Length == 0)
            {
                _logger.LogWarning("No orders to cancel");
                return;
            }

            var ordersCanceledLock = new SemaphoreSlim(0, 1);
            var canceledCount = 0;
            _wsClient.Listen<ExecutionReport>((client, er) =>
            {
                _logger.LogInformation($"Received ER for {er.ClOrdId}: {er.ExecType}");
                if (er.ExecType == ExecType.CanceledExec && orders.Any(o => o.OrderId == er.OrderId))
                {
                    if (++canceledCount == orders.Length)
                        ordersCanceledLock.Release();
                }
                return Task.CompletedTask;
            });

            foreach (var order in orders)
                await _wsClient.SendCommandAsync(order.ToOrderCancelRequest(CommonFuncs.NewClOrdId("cancel"))).ConfigureAwait(false);

            await ordersCanceledLock.WaitAsync().ConfigureAwait(false);
        }

        private async Task ReplaceAsync()
        {
            // RemoveListener is here only to override previous ExecutionReport handler. There is no need to use it every time before Listen().
            _wsClient.RemoveListener<ExecutionReport>();

            var clOrdId = CommonFuncs.NewClOrdId("limit");
            _wsClient.Listen<ExecutionReport>(async (client, er) =>
            {
                _logger.LogInformation($"Received ER for {er.ClOrdId}: {er.ExecType}");
                if (er.ExecType == ExecType.NewExec && er.ClOrdId == clOrdId)
                {
                    var command = er.ToOrderCancelReplaceRequest(CommonFuncs.NewClOrdId("replace"));
                    command.OrderQty = 2M.ToFixString();

                    await client.CancelReplaceOrderAsync(command).ConfigureAwait(false);
                }
            });

            await _wsClient.NewLimitOrderAsync(clOrdId, "XBTUSD", Side.Buy, 1M, MarginAccountId, 10000).ConfigureAwait(false);
        }

        private async Task SyncLimitOrderAsync()
        {
            // RemoveListener is here only to override previous ExecutionReport handler. There is no need to use it every time before Listen().
            _wsClient.RemoveListener<ExecutionReport>();

            var orderLock = new SemaphoreSlim(0, 1);
            var clOrdId = CommonFuncs.NewClOrdId("limit-1");

            var filledQty = "0";
            _wsClient.Listen<ExecutionReport>((client, er) =>
            {
                _logger.LogInformation($"Received execution report1: {er}");
                if (er.ExecType == ExecType.Trade && er.ClOrdId == clOrdId)
                {
                    filledQty = er.LastQty;
                    orderLock.Release();
                }

                return Task.CompletedTask;
            });

            await _wsClient.NewLimitOrderAsync(clOrdId, "BTC/USDT", Side.Buy, 0.01M, SpotAccountId, 10000).ConfigureAwait(false);

            // Wait until order is filled
            await orderLock.WaitAsync().ConfigureAwait(false);
            _logger.LogInformation($"Order filled for {filledQty}");
        }

        private async Task GetOpenPositionsAsync()
        {
            // RemoveListener is here only to override previous ExecutionReport handler. There is no need to use it every time before Listen().
            _wsClient.RemoveListener<MassPositionReport>();

            MassPositionReport massPositionReport = null;
            var positionsLock = new SemaphoreSlim(0, 1);

            _wsClient.Listen<MassPositionReport>((client, report) =>
            {
                massPositionReport = report;
                positionsLock.Release();
                return Task.CompletedTask;
            });

            await _wsClient.GetPositionsAsync(MarginAccountId, Guid.NewGuid().ToString()).ConfigureAwait(false);
            await positionsLock.WaitAsync().ConfigureAwait(false);

            _logger.LogInformation($"Mass position report: {massPositionReport}");
        }

        private async Task GetBalancesAsync()
        {
            // RemoveListener is here only to override previous ExecutionReport handler. There is no need to use it every time before Listen().
            _wsClient.RemoveListener<BalanceSnapshotRefresh>();

            BalanceSnapshotRefresh balanceSnapshot = null;
            var balancesLock = new SemaphoreSlim(0, 1);

            _wsClient.Listen<BalanceSnapshotRefresh>((client, report) =>
            {
                balanceSnapshot = report;
                balancesLock.Release();
                return Task.CompletedTask;
            });

            await _wsClient.AccountStatusReportAsync(MarginAccountId, Guid.NewGuid().ToString()).ConfigureAwait(false);
            await balancesLock.WaitAsync().ConfigureAwait(false);

            _logger.LogInformation($"Balances snapshot: {balanceSnapshot}");
        }

        private async Task OrderMassCancelAsync()
        {
            _wsClient.Listen<OrderMassCancelReport>((client, report) =>
            {
                _logger.LogInformation($"Order mass cancel report: {report}");
                return Task.CompletedTask;
            });

            // cancel orders that will open positions for specific symbol and side
            await _wsClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-1"), "XBTUSD",
                Side.Buy, PositionEffect.Open).ConfigureAwait(false);

            // cancel all orders for specific symbol and side
            await _wsClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-2"), "XBTUSD",
                Side.Buy).ConfigureAwait(false);

            // cancel all orders for specific symbol
            await _wsClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-3"), "XBTUSD")
                .ConfigureAwait(false);

            // cancel all order for account
            await _wsClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-4"))
                .ConfigureAwait(false);

            await Task.Delay(100).ConfigureAwait(false);
        }
    }
}