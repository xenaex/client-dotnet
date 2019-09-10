using Microsoft.Extensions.DependencyInjection;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Ws.Interfaces;
using XenaExchange.Client.Ws.MarketData;
using XenaExchange.Client.Ws.TradingApi;

namespace XenaExchange.Client.Ws
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXenaMarketDataWebsocketClient(this IServiceCollection serviceCollection, string uri)
        {
            // TODO: use Microsoft.Extensions.Options.
            var mdWsOptions = new MarketDataWsClientOptions { Uri = uri };

            return serviceCollection
                .AddSingleton(mdWsOptions)
                .AddSingleton<IFixSerializer, FixSerializer>()
                .AddTransient<IMarketDataWsClient, MarketDataWsClient>();
        }

        public static IServiceCollection AddXenaTradingWebsocketClient(this IServiceCollection serviceCollection, TradingWsClientOptions options)
        {
            return serviceCollection
                .AddSingleton(options)
                .AddSingleton<IFixSerializer, FixSerializer>()
                .AddTransient<ITradingWsClient, TradingWsClient>();
        }
    }
}