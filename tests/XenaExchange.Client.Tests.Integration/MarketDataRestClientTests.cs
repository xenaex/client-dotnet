using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest;
using XenaExchange.Client.Rest.MarketData;
using XenaExchange.Client.Rest.Requests;
using XenaExchange.Client.Ws.Common;

namespace XenaExchange.Client.Tests.Integration
{
    public class MarketDataRestClientTests
    {
        private const LogLevel TestsLogLevel = LogLevel.Debug;

        private readonly CancellationToken _token = CancellationToken.None;

        private IMarketDataRestClient _restClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _restClient = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.SetMinimumLevel(TestsLogLevel);
                    loggingBuilder.AddConsole();
                })
                .AddXenaMarketDataRestClient()
                .BuildServiceProvider()
                .GetService<IMarketDataRestClient>();
        }

        [Test]
        public async Task Test_GetCandlesAsync()
        {
            var now = DateTime.UtcNow;
            now = now.Date.AddHours(now.Hour); // cut to hours

            var mdRefresh = await _restClient
                .GetCandlesAsync("BTC/USDT", CandlesTimeframe.Timeframe1h, cancellationToken: _token)
                .ConfigureAwait(false);

            mdRefresh.Should().NotBeNull();
            mdRefresh.MDEntry.Should().HaveCount(3);
            mdRefresh.MDEntry[0].TransactTime.Should().Be(now.AddHours(-2).ToUnixNano());
            mdRefresh.MDEntry[2].TransactTime.Should().Be(now.ToUnixNano());
        }

        [Test]
        public async Task Test_GetCandlesFromToAsync()
        {
            var now = DateTime.UtcNow;
            now = now.Date.AddHours(now.Hour); // cut to hours
            var from = now.AddHours(-5);
            var to = now.AddHours(-1);

            var mdRefresh = await _restClient
                .GetCandlesAsync("BTC/USDT", CandlesTimeframe.Timeframe1h, from, to, _token)
                .ConfigureAwait(false);

            mdRefresh.Should().NotBeNull();
            mdRefresh.MDEntry.Should().HaveCount(5);
            mdRefresh.MDEntry[0].TransactTime.Should().Be(now.AddHours(-5).ToUnixNano());
            mdRefresh.MDEntry[4].TransactTime.Should().Be(now.AddHours(-1).ToUnixNano());
        }

        [Test]
        public async Task Test_GetDomAsync()
        {
            var mdRefresh = await _restClient
                .GetDomAsync("XBTUSD", cancellationToken: _token)
                .ConfigureAwait(false);

            mdRefresh.Should().NotBeNull();
            mdRefresh.MDEntry.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task Test_ListInstrumentsAsync()
        {
            var instruments = await _restClient.ListInstrumentsAsync(_token).ConfigureAwait(false);
            instruments.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task Test_ServerTime()
        {
            var utcNow = DateTime.UtcNow;
            var serverTime = await _restClient.GetServerTimeAsync(_token).ConfigureAwait(false);
            serverTime.Should().BeAfter(utcNow.Subtract(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task Test_TradeHistoryAsync()
        {
            const string symbol = "XBTUSD";
            var request = new TradeHistoryMdRequest(symbol)
            {
//                From = Functions.FromUnixNano(1574162455620117000),
//                To = Functions.FromUnixNano(1574162518887000000),
                PageNumber = 2,
                Limit = 10,
            };
            var mdRefresh = await _restClient.TradeHistoryAsync(request, _token).ConfigureAwait(false);

            mdRefresh.Symbol.Should().Be(symbol);
            mdRefresh.MDEntry.Should().HaveCountGreaterThan(0);

            var entry = mdRefresh.MDEntry[0];
            entry.MDEntryType.Should().Be("2");
            entry.TradeId.Should().NotBeNullOrWhiteSpace();
            entry.TransactTime.Should().BeGreaterThan(0);
            entry.MDEntryPx.Should().NotBeNullOrWhiteSpace().And.NotBe("0");
            entry.MDEntrySize.Should().NotBeNullOrWhiteSpace().And.NotBe("0");

            Assert.Fail(mdRefresh.ToString());
        }
    }
}