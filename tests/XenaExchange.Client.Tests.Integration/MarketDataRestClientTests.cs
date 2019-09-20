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
                .GetDomAsync("XBTUSD", aggregation: DOMAggregation.Aggregation250, cancellationToken: _token)
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
    }
}