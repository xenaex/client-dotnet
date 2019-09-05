using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XenaExchange.Client.Examples.Ws;
using XenaExchange.Client.Serialization;
using XenaExchange.Client.WsClient;
using XenaExchange.Client.WsClient.TradingApi;

namespace XenaExchange.Client.Examples
{
    class Program
    {
        private const string _mdWsUri = "wss://api.xena.exchange/ws/market-data";
        private const string _tradingWsUri = "wss://api.xena.exchange/ws/trading";
        private const string _apiKey = "TO_FILL";
        private const string _apiSecret = "TO_FILL";

        private const long _spotAccountId = 1;
        private const long _marginAccountId = 2;

        private static readonly CancellationToken _token = CancellationToken.None;

        private static readonly TradingWsClientOptions _tradingWsOptions = new TradingWsClientOptions()
        {
            Uri = _tradingWsUri,
            Accounts = new List<long> { _spotAccountId, _marginAccountId },
            ApiKey = _apiKey,
            ApiSecret = _apiSecret,
        };

        public static async Task Main(string[] args)
        {
            // await DIExampleAsync().ConfigureAwait(false);
            await NoDIExampleAsync().ConfigureAwait(false);

            // TODO: handle CTRL+C, IHostBuilder.RunConsoleAsync() in Ubuntu doesn't work.
            await Task.Delay(int.MaxValue).ConfigureAwait(false);
        }

        private static async Task NoDIExampleAsync()
        {
            var logLevel = LogLevel.Debug;
            var serializer = new FixSerializer();

            // var mdWsOptions = new MarketDataWsClientOptions{ Uri = _mdWsUri };
            // var mdLogger = Logging.ConsoleLogger<MarketDataWsClient>(logLevel);
            // var mdWsClient = new MarketDataWsClient(mdWsOptions, serializer, mdLogger);
            // var mdExample = new MarketDataExample(mdWsClient, Logging.ConsoleLogger<MarketDataExample>(logLevel));
            // await mdExample.StartAsync(_token).ConfigureAwait(false);

            var tradingLogger = Logging.ConsoleLogger<TradingWsClient>(logLevel);
            var tradingWsClient = new TradingWsClient(_tradingWsOptions, serializer, tradingLogger);
            var tradingExample = new WsTradingExample(tradingWsClient, _tradingWsOptions, Logging.ConsoleLogger<WsTradingExample>(logLevel));
            await tradingExample.StartAsync(_token).ConfigureAwait(false);
        }

        private static async Task DIExampleAsync()
        {
            var services = new ServiceCollection()
                            .AddExamplesLogging(LogLevel.Trace)
                            .AddXenaMarketDataWebsocketClient(_mdWsUri)
                            .AddXenaTradingWebsocketClient(_tradingWsOptions)
                            .AddSingleton<WsTradingExample>()
                            .AddSingleton<WsMarketDataExample>()
                            .BuildServiceProvider();

            var tradingExample = services.GetService<WsTradingExample>();
            var mdExample = services.GetService<WsMarketDataExample>();

            await tradingExample.StartAsync(_token).ConfigureAwait(false);
            // await mdExample.StartAsync(_token).ConfigureAwait(false);
        }
    }
}