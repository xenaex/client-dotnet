using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Microsoft.Extensions.Logging;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest.Exceptions;
using XenaExchange.Client.Rest.Requests;
using XenaExchange.Client.Rest.Trading;

namespace XenaExchange.Client.Examples.Rest
{
    public class TradingRestExample
    {
        private const long SpotAccountId = 1;
        private const long MarginAccountId = 2;

        private readonly ITradingRestClient _restClient;
        private readonly ILogger _logger;

        public TradingRestExample(ITradingRestClient restClient, ILogger<TradingRestExample> logger)
        {
            _restClient = restClient;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await TestTradingAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (RestClientException e)
            {
                _logger.LogError(e, $"Rest client exception: {e.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task TestTradingAsync(CancellationToken cancellationToken)
        {
            // await MarketOrderAsync(cancellationToken).ConfigureAwait(false);
            await LimitOrderAsync(cancellationToken).ConfigureAwait(false);
            // await LimitOrderPostOnlyAsync(cancellationToken).ConfigureAwait(false);
            // await StopOrderAsync(cancellationToken).ConfigureAwait(false);
            // await SltpGroupAsync(cancellationToken).ConfigureAwait(false);
            // await StopLossForExistingPositionAsync(cancellationToken).ConfigureAwait(false);
            // await TakeProfitForExistingPositionAsync(cancellationToken).ConfigureAwait(false);
            // await CancelOrdersAsync(cancellationToken).ConfigureAwait(false);
            // await ReplaceAsync(cancellationToken).ConfigureAwait(false);
            // await CollapsePositionsAsync(cancellationToken).ConfigureAwait(false);
            // await GetOpenPositionsAsync(cancellationToken).ConfigureAwait(false);
            // await PositionsHistoryAsync(cancellationToken).ConfigureAwait(false);
            // await ListActiveOrdersAsync(cancellationToken).ConfigureAwait(false);
            // await ListActiveOrdersAsync(cancellationToken).ConfigureAwait(false);
            // await ListOrderAsync(cancellationToken).ConfigureAwait(false);
            // await ListLastOrderStatusesAsync(cancellationToken).ConfigureAwait(false);
            // await ListOrderHistoryAsync(cancellationToken).ConfigureAwait(false);
            // await TradeHistoryAsync(cancellationToken).ConfigureAwait(false);
            // await GetBalancesAsync(cancellationToken).ConfigureAwait(false);
            // await GetMarginRequirementsAsync(cancellationToken).ConfigureAwait(false);
            // await ListAccountsAsync(cancellationToken).ConfigureAwait(false);
            // await MassCancelAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task MarketOrderAsync(CancellationToken cancellationToken)
        {
            var executionReport = await _restClient.NewMarketOrderAsync(
                CommonFuncs.NewClOrdId("market-order"),
                "BTC/USDT",
                Side.Sell,
                0.01M,
                SpotAccountId,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            HandleOrderReport(executionReport);
        }

        private async Task LimitOrderAsync(CancellationToken cancellationToken)
        {
            var executionReport = await _restClient.NewLimitOrderAsync(
                CommonFuncs.NewClOrdId("limit-order"),
                "BTC/USDT",
                Side.Sell,
                0.01M,
                SpotAccountId,
                10500,
                text: "order comment 1",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            HandleOrderReport(executionReport);
        }

        private async Task LimitOrderPostOnlyAsync(CancellationToken cancellationToken)
        {
            var executionReport = await _restClient.NewLimitOrderAsync(
                    CommonFuncs.NewClOrdId("limit-order"),
                    "BTC/USDT",
                    Side.Buy,
                    0.01M,
                    SpotAccountId,
                    10000,
                    execInst: new[] { ExecInst.StayOnOfferSide },
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            HandleOrderReport(executionReport);

            executionReport = await _restClient.NewLimitOrderAsync(
                    CommonFuncs.NewClOrdId("limit-order"),
                    "BTC/USDT",
                    Side.Buy,
                    0.01M,
                    SpotAccountId,
                    10000,
                    execInst: new[] { ExecInst.PegToOfferSide },
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            HandleOrderReport(executionReport);
        }

        private async Task StopOrderAsync(CancellationToken cancellationToken)
        {
            var executionReport = await _restClient.NewStopOrderAsync(
                CommonFuncs.NewClOrdId("stop-order"),
                "BTC/USDT",
                Side.Sell,
                0.01M,
                SpotAccountId,
                9500,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            HandleOrderReport(executionReport);
        }

        private async Task SltpGroupAsync(CancellationToken cancellationToken)
        {
            var id = CommonFuncs.NewClOrdId("limit-sltp-1");
            var command = OrderExtensions.NewLimitOrder(id, "XBTUSD", Side.Sell, 1M, MarginAccountId, 10600);
            command.AddTrailingStopLoss(500);
            command.AddTakeProfit(10000);

            var executionReport = await _restClient.NewOrderAsync(command, cancellationToken).ConfigureAwait(false);
            HandleOrderReport(executionReport);

            // OR
            executionReport = await _restClient.NewLimitOrderAsync(
                CommonFuncs.NewClOrdId("limit-sltp-2"),
                "XBTUSD",
                Side.Sell,
                1M,
                MarginAccountId,
                10600,
                trailingOffset: 500,
                takeProfitPrice: 10000,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            HandleOrderReport(executionReport);
        }

        private async Task StopLossForExistingPositionAsync(CancellationToken cancellationToken)
        {
            var id = CommonFuncs.NewClOrdId("stop-order-1");
            var command = OrderExtensions.NewStopOrder(id, "XBTUSD", Side.Sell, 1M, MarginAccountId, 9500);
            command.ForPosition(12345);

            var executionReport = await _restClient.NewOrderAsync(command, cancellationToken).ConfigureAwait(false);
            HandleOrderReport(executionReport);

            // OR
            executionReport = await _restClient.NewStopOrderAsync(
                    CommonFuncs.NewClOrdId("stop-order-2"),
                    "XBTUSD",
                    Side.Sell, 1M,
                    MarginAccountId,
                    9500,
                    positionId: 12345,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            HandleOrderReport(executionReport);
        }

        private async Task TakeProfitForExistingPositionAsync(CancellationToken cancellationToken)
        {
            var id = CommonFuncs.NewClOrdId("stop-order");
            var command = OrderExtensions.NewLimitOrder(id, "XBTUSD", Side.Sell, 1M, MarginAccountId, 10000);
            command.ForPosition(12345);

            var executionReport = await _restClient.NewOrderAsync(command, cancellationToken).ConfigureAwait(false);
            HandleOrderReport(executionReport);
        }

        private async Task CancelOrdersAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 3; i++)
            {
                var executionReport = await _restClient.NewLimitOrderAsync(
                        CommonFuncs.NewClOrdId($"limit-{i}"),
                        "BTC/USDT",
                        Side.Buy,
                        0.01M,
                        SpotAccountId,
                        10000,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (executionReport.ExecType != ExecType.NewExec)
                    continue;

                switch (i)
                {
                    case 0:
                        var cancelResponse1 = await _restClient.CancelOrderByOrderIdAsync(
                            CommonFuncs.NewClOrdId("cancel-1"),
                            executionReport.OrderId,
                            "BTC/USDT",
                            Side.Buy,
                            SpotAccountId,
                            cancellationToken: cancellationToken).ConfigureAwait(false);

                        HandleOrderReport(cancelResponse1);
                        break;
                    case 1:
                        // Actually it is possible to cancel order by client id at any time. No need to wait for ExecutionReport confirmation.
                        var cancelResponse2 = await _restClient.CancelOrderByClOrdIdAsync(
                            CommonFuncs.NewClOrdId("cancel-2"),
                            executionReport.ClOrdId,
                            "BTC/USDT",
                            Side.Buy,
                            SpotAccountId,
                            cancellationToken: cancellationToken).ConfigureAwait(false);

                        HandleOrderReport(cancelResponse2);
                        break;
                    case 2:
                        var cancelCommand = executionReport.ToOrderCancelRequest(CommonFuncs.NewClOrdId("cancel-3"));
                        var cancelResponse3 = await _restClient.CancelOrderAsync(cancelCommand, cancellationToken)
                            .ConfigureAwait(false);

                        HandleOrderReport(cancelResponse3);
                        break;
                }
            }
        }

        private async Task ReplaceAsync(CancellationToken cancellationToken)
        {
            var executionReport = await _restClient.NewLimitOrderAsync(
                CommonFuncs.NewClOrdId("limit"),
                    "XBTUSD",
                    Side.Buy,
                    1M,
                    MarginAccountId,
                    10000,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            var replaceCommand = executionReport.ToOrderCancelReplaceRequest(CommonFuncs.NewClOrdId("replace"));
            replaceCommand.OrderQty = 2M.ToFixString();

            var replaceResponse = await _restClient.ReplaceOrderAsync(replaceCommand, cancellationToken)
                .ConfigureAwait(false);

            HandleOrderReport(replaceResponse);
        }

        private async Task CollapsePositionsAsync(CancellationToken cancellationToken)
        {
            var report = await _restClient.CollapsePositionsAsync(
                CommonFuncs.NewClOrdId("collapse"),
                MarginAccountId,
                "XBTUSD",
                cancellationToken).ConfigureAwait(false);

            _logger.LogInformation($"Position maintenance report: {report}");
        }

        private async Task GetOpenPositionsAsync(CancellationToken cancellationToken)
        {
            var positions = await _restClient.ListOpenPositionsAsync(MarginAccountId, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", positions.Select(p => p.ToString()));
            _logger.LogInformation($"Open positions: {toPrint}");
        }

        private async Task PositionsHistoryAsync(CancellationToken cancellationToken)
        {
            // look up documentation to get all available filters
            var request = new PositionsHistoryRequest(MarginAccountId, symbol: "XBTUSD");
            var positions = await _restClient.PositionsHistoryAsync(request, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", positions.Select(p => p.ToString()));
            _logger.LogInformation($"Positions history: {toPrint}");
        }

        private async Task ListActiveOrdersAsync(CancellationToken cancellationToken)
        {
            var orders = await _restClient.ListActiveOrdersAsync(MarginAccountId, string.Empty, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", orders.Select(p => p.ToString()));
            _logger.LogInformation($"Active orders: {toPrint}");
        }

        private async Task ListOrderAsync(CancellationToken cancellationToken)
        {
            var order = await _restClient.GetOrderAsync(MarginAccountId, string.Empty, "o83821gee", cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation($"Active orders: {order.ToString()}");
        }

        private async Task ListLastOrderStatusesAsync(CancellationToken cancellationToken)
        {
            var orders = await _restClient.GetLastOrderStatusesAsync(MarginAccountId, "XBTUSD", null, null, null, null, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", orders.Select(p => p.ToString()));
            _logger.LogInformation($"Active orders: {toPrint}");
        }
        private async Task ListOrderHistoryAsync(CancellationToken cancellationToken)
        {
            var orders = await _restClient.GetOrderHistoryAsync(MarginAccountId, "XBTUSD", string.Empty, string.Empty, null, null, null, null, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", orders.Select(p => p.ToString()));
            _logger.LogInformation($"Active orders: {toPrint}");
        }

        private async Task TradeHistoryAsync(CancellationToken cancellationToken)
        {
            // look up documentation to get all available filters
            var request = new TradeHistoryRequest(MarginAccountId, symbol: "XBTUSD");
            var trades = await _restClient.TradeHistoryAsync(request, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", trades.Select(p => p.ToString()));
            _logger.LogInformation($"Trades history: {toPrint}");
        }

        private async Task GetBalancesAsync(CancellationToken cancellationToken)
        {
            var balanceSnapshot = await _restClient.GetBalancesAsync(MarginAccountId, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", balanceSnapshot.Balances.Select(p => p.ToString()));
            _logger.LogInformation($"Balances: {toPrint}");
        }

        private async Task GetMarginRequirementsAsync(CancellationToken cancellationToken)
        {
            var report = await _restClient.GetMarginRequirementsAsync(MarginAccountId, cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", report.MarginAmounts.Select(p => p.ToString()));
            _logger.LogInformation($"Margin amounts: {toPrint}");
        }

        private async Task ListAccountsAsync(CancellationToken cancellationToken)
        {
            var accounts = await _restClient.ListAccountsAsync(cancellationToken)
                .ConfigureAwait(false);

            var toPrint = string.Join("\n", accounts.Select(p => p.ToString()));
            _logger.LogInformation($"Accounts: {toPrint}");
        }

        private async Task MassCancelAsync(CancellationToken cancellationToken)
        {
            // cancel orders that will open positions for specific symbol and side
            var report = await _restClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-1"),
                "XBTUSD", Side.Buy, PositionEffect.Open).ConfigureAwait(false);

            _logger.LogInformation($"Order mass cancel report 1: {report}");

            // cancel all orders for specific symbol and side
            report = await _restClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-2"),
                "XBTUSD", Side.Buy).ConfigureAwait(false);

            _logger.LogInformation($"Order mass cancel report 2: {report}");

            // cancel all orders for specific symbol
            report = await _restClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-3"),
                    "XBTUSD").ConfigureAwait(false);

            _logger.LogInformation($"Order mass cancel report 3: {report}");

            // cancel all order for account
            report = await _restClient.OrderMassCancelAsync(MarginAccountId, CommonFuncs.NewClOrdId("mass-cancel-4"))
                .ConfigureAwait(false);

            _logger.LogInformation($"Order mass cancel report 4: {report}");
        }

        private void HandleOrderReport(ExecutionReport er)
        {
            _logger.LogInformation($"Execution report: {er}");
        }
    }
}