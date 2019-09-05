using Microsoft.Extensions.DependencyInjection;
using XenaExchange.Client.Serialization;
using XenaExchange.Client.WsClient.Interfaces;
using XenaExchange.Client.WsClient.MarketData;
using XenaExchange.Client.WsClient.TradingApi;

namespace XenaExchange.Client.WsClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXenaMarketDataWebsocketClient(this IServiceCollection serviceCollection, string uri)
        {
            // TODO: use Microsoft.Extensions.Options.
            var mdWsOptions = new MarketDataWsClientOptions { Uri = uri };

            return serviceCollection
                .AddSingleton(mdWsOptions)
                .AddSingleton<ISerializer, FixSerializer>()
                .AddTransient<IMarketDataWsClient, MarketDataWsClient>();
        }

        public static IServiceCollection AddXenaTradingWebsocketClient(this IServiceCollection serviceCollection, TradingWsClientOptions options)
        {
            return serviceCollection
                .AddSingleton(options)
                .AddSingleton<ISerializer, FixSerializer>()
                .AddTransient<ITradingWsClient, TradingWsClient>();
        }
    }
}