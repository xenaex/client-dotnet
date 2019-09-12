﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XenaExchange.Client.Examples.Rest;
using XenaExchange.Client.Examples.Ws;
using XenaExchange.Client.Rest;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Serialization.Rest;
using XenaExchange.Client.Ws;
using XenaExchange.Client.Ws.MarketData;
using XenaExchange.Client.Ws.TradingApi;

namespace XenaExchange.Client.Examples
{
    internal static class Program
    {
        private const string MdWsUri = "wss://api.xena.exchange/ws/market-data";
        private const string TradingWsUri = "wss://api.xena.exchange/ws/trading";
        private const string ApiKey = "TO_FILL";
        private const string ApiSecret = "TO_FILL";

        private const long SpotAccountId = 1;
        private const long MarginAccountId = 2;

        private static readonly CancellationToken _token = CancellationToken.None;

        private static readonly TradingWsClientOptions _tradingWsOptions = new TradingWsClientOptions
        {
            Uri = TradingWsUri,
            Accounts = new List<long> { SpotAccountId, MarginAccountId },
            ApiKey = ApiKey,
            ApiSecret = ApiSecret,
        };

        private static readonly TradingRestClientOptions _tradingRestOptions = new TradingRestClientOptions
        {
            ApiKey = ApiKey,
            ApiSecret = ApiSecret,
        };

        public static async Task Main(string[] args)
        {
            await DiExampleAsync().ConfigureAwait(false);
//            await NoDiExampleAsync().ConfigureAwait(false);

            // TODO: handle CTRL+C, IHostBuilder.RunConsoleAsync() in Ubuntu doesn't work.
            await Task.Delay(int.MaxValue).ConfigureAwait(false);
        }

        private static async Task NoDiExampleAsync()
        {
            var logLevel = LogLevel.Debug;

            var fixSerializer = new FixSerializer();
            var restSerializer = new RestSerializer();

            // Market data websocket example
            var mdWsOptions = new MarketDataWsClientOptions{ Uri = MdWsUri };
            var mdLogger = Dependencies.ConsoleLogger<MarketDataWsClient>(logLevel);
            var mdWsClient = new MarketDataWsClient(mdWsOptions, fixSerializer, mdLogger);
            var mdExample = new MarketDataWsExample(mdWsClient, Dependencies.ConsoleLogger<MarketDataWsExample>(logLevel));
            await mdExample.StartAsync(_token).ConfigureAwait(false);

            // Trading websocket example
            var tradingLogger = Dependencies.ConsoleLogger<TradingWsClient>(logLevel);
            var tradingWsClient = new TradingWsClient(_tradingWsOptions, fixSerializer, tradingLogger);
            var tradingExample = new TradingWsExample(tradingWsClient, _tradingWsOptions, Dependencies.ConsoleLogger<TradingWsExample>(logLevel));
            await tradingExample.StartAsync(_token).ConfigureAwait(false);

            // Trading rest example
            var httpClientFactory = Dependencies.CreateHttpClientFactory(
                TradingRestClient.HttpClientName,
                "https://api.xena.exchange");

            var tradingRestClient = new TradingRestClient(
                httpClientFactory,
                _tradingRestOptions,
                restSerializer,
                Dependencies.ConsoleLogger<TradingRestClient>(logLevel));

            var tradingRestExample = new TradingRestExample(
                tradingRestClient,
                Dependencies.ConsoleLogger<TradingRestExample>(logLevel));

            await tradingRestExample.StartAsync(_token).ConfigureAwait(false);
        }

        private static async Task DiExampleAsync()
        {
            var services = new ServiceCollection()
                            .AddExamplesLogging(LogLevel.Debug)
                            .AddXenaMarketDataWebsocketClient(MdWsUri)
                            .AddXenaTradingWebsocketClient(_tradingWsOptions)
                            .AddXenaTradingRestClient(_tradingRestOptions)
                            .AddSingleton<TradingWsExample>()
                            .AddSingleton<MarketDataWsExample>()
                            .AddSingleton<TradingRestExample>()
                            .BuildServiceProvider();

            var tradingWsExample = services.GetService<TradingWsExample>();
            var mdWsExample = services.GetService<MarketDataWsExample>();
            var tradingRestExample = services.GetService<TradingRestExample>();

//            await tradingWsExample.StartAsync(_token).ConfigureAwait(false);
//            await mdWsExample.StartAsync(_token).ConfigureAwait(false);
            await tradingRestExample.StartAsync(_token).ConfigureAwait(false);
        }
    }
}