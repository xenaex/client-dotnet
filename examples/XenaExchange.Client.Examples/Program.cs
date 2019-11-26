using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using XenaExchange.Client.Examples.Rest;
using XenaExchange.Client.Examples.Ws;
using XenaExchange.Client.Rest;
using XenaExchange.Client.Rest.MarketData;
using XenaExchange.Client.Rest.Trading;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Serialization.Rest;
using XenaExchange.Client.Ws;
using XenaExchange.Client.Ws.MarketData;
using XenaExchange.Client.Ws.TradingApi;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

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

        private static readonly CancellationToken Token = CancellationToken.None;

        private static readonly TradingWsClientOptions TradingWsOptions = new TradingWsClientOptions
        {
            Uri = TradingWsUri,
            Accounts = new List<long> { SpotAccountId, MarginAccountId },
            ApiKey = ApiKey,
            ApiSecret = ApiSecret,
        };

        private static readonly TradingRestClientOptions TradingRestOptions = new TradingRestClientOptions
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
            await mdExample.StartAsync(Token).ConfigureAwait(false);

            // Trading websocket example
            var tradingLogger = Dependencies.ConsoleLogger<TradingWsClient>(logLevel);
            var tradingWsClient = new TradingWsClient(TradingWsOptions, fixSerializer, tradingLogger);
            var tradingExample = new TradingWsExample(tradingWsClient, TradingWsOptions, Dependencies.ConsoleLogger<TradingWsExample>(logLevel));

            var httpClient = new HttpClient {BaseAddress = new Uri("https://api.xena.exchange")};

            var tradingRestClient = new TradingRestClient(TradingRestOptions, restSerializer, httpClient: httpClient);
            var tradingRestExample = new TradingRestExample(
                tradingRestClient,
                Dependencies.ConsoleLogger<TradingRestExample>(logLevel));

            await tradingRestExample.StartAsync(Token).ConfigureAwait(false);

            // Market data rest example
            var marketDataRestClient = new MarketDataRestClient(fixSerializer, restSerializer, httpClient: httpClient);
            var marketDataRestExample = new MarketDataRestExample(
                marketDataRestClient,
                Dependencies.ConsoleLogger<MarketDataRestExample>(logLevel));

            await marketDataRestExample.StartAsync(Token).ConfigureAwait(false);
        }

        private static async Task DiExampleAsync()
        {
            var services = new ServiceCollection()
                            .AddExamplesLogging(LogLevel.Debug)

                            .AddXenaMarketDataWebsocketClient(MdWsUri)
                            .AddXenaTradingWebsocketClient(TradingWsOptions)
                            .AddXenaTradingRestClient(TradingRestOptions)
                            .AddXenaMarketDataRestClient()

                            .AddSingleton<TradingWsExample>()
                            .AddSingleton<MarketDataWsExample>()
                            .AddSingleton<TradingRestExample>()
                            .AddSingleton<MarketDataRestExample>()

                            .BuildServiceProvider();

            var tradingWsExample = services.GetService<TradingWsExample>();
            var mdWsExample = services.GetService<MarketDataWsExample>();
            var tradingRestExample = services.GetService<TradingRestExample>();
            var mdRestExample = services.GetService<MarketDataRestExample>();

//            await tradingWsExample.StartAsync(Token).ConfigureAwait(false);
//            await mdWsExample.StartAsync(Token).ConfigureAwait(false);
            await tradingRestExample.StartAsync(Token).ConfigureAwait(false);
//            await mdRestExample.StartAsync(Token).ConfigureAwait(false);
        }
    }
}