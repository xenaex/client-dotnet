using XenaExchange.Client.Websocket.Client.Interfaces;
using XenaExchange.Client.Websocket.Client.MarketData;
using XenaExchange.Client.Websocket.Client.TradingApi;
using XenaExchange.Client.Websocket.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace XenaExchange.Client.Websocket.Client
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