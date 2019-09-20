using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest;
using XenaExchange.Client.Rest.Requests;
using XenaExchange.Client.Rest.Trading;

namespace XenaExchange.Client.Tests.Integration
{
    public class TradingRestClientTests
    {
        private const LogLevel TestsLogLevel = LogLevel.Debug;

        private const string ApiKey = "b0kzGvpw1dYm2ksQ3Lsvu0MywjtjYQkcHykVIa1hmuA=";
        private const string ApiSecret = "3077020101042007807522912f2f97bc0702820b12e08bb468672cd85095d1864edecc8b923741a00a06082a8648ce3d030107a14403420004f0193792525e957686882e7541b41f608264ed308c2591f61c6d747ac0292f202d582d1c0cc951aad392995671651f3fda8b22cba77e56f5dc1951dca20aa889";

        private const long SpotAccountId = 8263118;
        private const long MarginAccountId = 1012833459;

        private readonly CancellationToken _token = CancellationToken.None;

        private ITradingRestClient _restClient;

        [OneTimeSetUp]
        public void Setup()
        {
            var options = new TradingRestClientOptions
            {
                ApiKey = ApiKey,
                ApiSecret = ApiSecret,
            };

            _restClient = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.SetMinimumLevel(TestsLogLevel);
                    loggingBuilder.AddConsole();
                })
                .AddXenaTradingRestClient(options)
                .BuildServiceProvider()
                .GetService<ITradingRestClient>();
        }

        [Test]
        public async Task Test_NewOrder()
        {
            var command = OrderExtensions.NewLimitOrder(NewClOrdId("limit"), "XBTUSD", Side.Sell, 1M,
                MarginAccountId, 10000M);

            var er = await _restClient.NewOrderAsync(command, _token).ConfigureAwait(false);

            er.Should().NotBeNull();
            er.Should().NotBeNull();
            er.ExecType.Should().Be(ExecType.PendingNewExec);
        }

        [Test]
        public async Task Test_NewLimitOrder()
        {
            var er = await _restClient.NewLimitOrderAsync(
                NewClOrdId("limit"),
                "XBTUSD",
                Side.Sell, 1M,
                MarginAccountId,
                10000M,
                cancellationToken: _token).ConfigureAwait(false);

            er.Should().NotBeNull();
            er.Should().NotBeNull();
            er.ExecType.Should().Be(ExecType.PendingNewExec);
        }

        [Test]
        public async Task Test_NewStopOrder()
        {
            var er = await _restClient.NewStopOrderAsync(
                NewClOrdId("stop"),
                "BTC/USDT",
                Side.Sell,
                0.01M,
                SpotAccountId,
                9500,
                cancellationToken: _token).ConfigureAwait(false);

            er.Should().NotBeNull();
            er.Should().NotBeNull();
            er.ExecType.Should().Be(ExecType.NewExec);
        }

        [Test]
        public async Task Test_CancelOrder()
        {
            var newOrderCmd = OrderExtensions.NewLimitOrder(NewClOrdId("limit"), "XBTUSD", Side.Sell, 1M,
                MarginAccountId, 10000M);

            var er = await _restClient.NewOrderAsync(newOrderCmd, _token).ConfigureAwait(false);
            er.Should().NotBeNull();
            er.Should().NotBeNull();
            er.ExecType.Should().Be(ExecType.PendingNewExec);

            var cancelOrderCmd = er.ToOrderCancelRequest(NewClOrdId("cancel"));
            var cancelOrderResponse = await _restClient.CancelOrderAsync(cancelOrderCmd, _token).ConfigureAwait(false);

            cancelOrderResponse.Should().NotBeNull();
            cancelOrderResponse.Should().NotBeNull();
            cancelOrderResponse.ExecType.Should().Be(ExecType.CanceledExec);
        }

        [Test]
        public async Task Test_CancelOrderByClOrdId()
        {
            var newOrderCmd = OrderExtensions.NewLimitOrder(NewClOrdId("limit"), "XBTUSD", Side.Sell, 1M,
                MarginAccountId, 10000M);

            var er = await _restClient.NewOrderAsync(newOrderCmd, _token).ConfigureAwait(false);
            er.Should().NotBeNull();
            er.Should().NotBeNull();
            er.ExecType.Should().Be(ExecType.PendingNewExec);

            var cancelOrderResponse = await _restClient.CancelOrderByClOrdIdAsync(
                NewClOrdId("cancel-2"),
                er.ClOrdId,
                "XBTUSD",
                Side.Sell,
                MarginAccountId,
                cancellationToken: _token).ConfigureAwait(false);

            cancelOrderResponse.Should().NotBeNull();
            cancelOrderResponse.Should().NotBeNull();
            cancelOrderResponse.ExecType.Should().Be(ExecType.CanceledExec);
        }

        [Test]
        public async Task Test_CancelOrderByOrderId()
        {
            var newOrderCmd = OrderExtensions.NewLimitOrder(NewClOrdId("limit"), "XBTUSD", Side.Sell, 1M,
                MarginAccountId, 10000M);

            var er = await _restClient.NewOrderAsync(newOrderCmd, _token).ConfigureAwait(false);
            er.Should().NotBeNull();
            er.Should().NotBeNull();
            er.ExecType.Should().Be(ExecType.PendingNewExec);

            var cancelOrderResponse = await _restClient.CancelOrderByOrderIdAsync(
                NewClOrdId("cancel-2"),
                er.OrderId,
                "XBTUSD",
                Side.Sell,
                MarginAccountId,
                cancellationToken: _token).ConfigureAwait(false);

            cancelOrderResponse.Should().NotBeNull();
            cancelOrderResponse.Should().NotBeNull();
            cancelOrderResponse.ExecType.Should().Be(ExecType.CanceledExec);
        }

        [Test]
        public async Task Test_ReplaceOrderSingle()
        {
            var newOrderCmd = OrderExtensions.NewLimitOrder(NewClOrdId("limit"), "XBTUSD", Side.Sell, 1M,
                MarginAccountId, 10000M);

            var newOrderEr = await _restClient.NewOrderAsync(newOrderCmd, _token).ConfigureAwait(false);
            newOrderEr.Should().NotBeNull();
            newOrderEr.Should().NotBeNull();
            newOrderEr.ExecType.Should().Be(ExecType.PendingNewExec);

            var replaceOrderCmd = newOrderEr.ToOrderCancelReplaceRequest(NewClOrdId("cancel"));
            replaceOrderCmd.Price = 10500M.ToFixString();
            var replaceOrderEr = await _restClient.ReplaceOrderAsync(replaceOrderCmd, _token).ConfigureAwait(false);

            replaceOrderEr.Should().NotBeNull();
            replaceOrderEr.Should().NotBeNull();
            replaceOrderEr.ExecType.Should().Be(ExecType.PendingReplaceExec);
        }

        [Test]
        public async Task Test_CollapsePositions()
        {
            var report = await _restClient.CollapsePositionsAsync(
                NewClOrdId("collapse"),
                MarginAccountId,
                "XBTUSD",
                _token).ConfigureAwait(false);

            report.Should().NotBeNull();
            report.Should().NotBeNull();
        }

        [Test]
        public async Task Test_ListAccounts()
        {
            var accounts = await _restClient.ListAccountsAsync(_token).ConfigureAwait(false);
            accounts.Should().NotBeNull();

            var expected = new[]
            {
                new AccountInfo{Id = SpotAccountId, Kind = AccountKind.Spot},
                new AccountInfo{Id = MarginAccountId, Kind = AccountKind.Margin, Currency = "BTC"},
            };
            accounts.Should().BeEquivalentTo<AccountInfo>(expected);
        }

        [Test]
        public async Task Test_GetBalances()
        {
            var balanceSnapshot = await _restClient.GetBalancesAsync(SpotAccountId, _token).ConfigureAwait(false);
            balanceSnapshot.Should().NotBeNull();
            balanceSnapshot.Should().NotBeNull();
            balanceSnapshot.Balances.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Test_GetMarginRequirements()
        {
            var report = await _restClient.GetMarginRequirementsAsync(MarginAccountId, _token).ConfigureAwait(false);
            report.Should().NotBeNull();
            report.Should().NotBeNull();
            report.Account.Should().Be(MarginAccountId);
            report.MarginAmounts.Count.Should().BeGreaterThan(0);
            report.RejectReason.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task Test_ListOpenPositions()
        {
            var positions = await _restClient.ListOpenPositionsAsync(MarginAccountId, _token).ConfigureAwait(false);
            positions.Should().NotBeNull();
            positions.Should().NotBeNull();
            positions.Length.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Test_PositionsHistory()
        {
            var request = new PositionsHistoryRequest(MarginAccountId)
            {
//                PositionId =130722413,
//                Symbol = "XBTUSD",
//                OpenFrom = DateTime.Parse("2019-08-02T08:04:40Z").ToUniversalTime(),
//                OpenTo = DateTime.Parse("2019-08-02T08:04:50Z").ToUniversalTime(),
//                CloseFrom = DateTime.Parse("2019-07-26T13:51:04Z").ToUniversalTime(),
//                CloseTo = DateTime.Parse("2019-07-26T13:51:06Z").ToUniversalTime(),
//                PageNumber = 2,
//                Limit = 2,
            };
            var positions = await _restClient.PositionsHistoryAsync(request, _token).ConfigureAwait(false);
            positions.Should().NotBeNull();
            positions.Should().NotBeNull();
            positions.Length.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Test_ListActiveOrders()
        {
            var orders = await _restClient.ListActiveOrdersAsync(MarginAccountId, _token).ConfigureAwait(false);
            orders.Should().NotBeNull();
            orders.Length.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Test_TradeHistory()
        {
            var request = new TradeHistoryRequest(MarginAccountId)
            {
//                TradeId = "177ed192-03e8-4f7c-9f69-bbfc32101cab",
//                ClOrdId = "KwdVbYloR",
//                Symbol = "XBTUSD",
//                From = DateTime.Parse("2019-08-02T08:04:42Z").ToUniversalTime(),
//                To = DateTime.Parse("2019-08-02T08:04:44Z").ToUniversalTime(),
//                PageNumber = 2,
//                Limit = 2,
            };
            var trades = await _restClient.TradeHistoryAsync(request, _token).ConfigureAwait(false);
            trades.Should().NotBeNull();
            trades.Should().NotBeNull();
            trades.Length.Should().BeGreaterThan(0);
        }

        private static string NewClOrdId(string prefix) => $"{prefix}-{DateTime.UtcNow.Ticks.ToString()}";
    }
}