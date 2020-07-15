using System;
using Microsoft.Extensions.DependencyInjection;
using XenaExchange.Client.Rest.MarketData;
using XenaExchange.Client.Rest.Trading;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Serialization.Rest;

namespace XenaExchange.Client.Rest
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds to service collection:
        ///     - specified options as singleton;
        ///     - <see cref="RestSerializer"/> as <see cref="IRestSerializer"/> as singleton;
        ///     - HttpClient with name <see cref="TradingRestClient.HttpClientName"/>;
        ///     - <see cref="ITradingRestClient"/>.
        /// </summary>
        /// <param name="services">Service collection to add client to.</param>
        /// <param name="options">Trading rest client options.</param>
        /// <param name="uri">Xena trading rest uri.</param>
        /// <returns>Populated service collection.</returns>
        public static IServiceCollection AddXenaTradingRestClient(
            this IServiceCollection services,
            TradingRestClientOptions options,
            string uri = "https://api.xena.exchange")
        {
            return services
                .AddSingleton(options)
                .AddSingleton<IRestSerializer, RestSerializer>()
                .AddTransient<ITradingRestClient, TradingRestClient>()
                .AddHttpClient(RestClientBase.HttpClientName, client => client.BaseAddress = new Uri(uri))
                .Services;
        }

        /// <summary>
        /// Adds to service collection:
        ///     - specified options as singleton;
        ///     - <see cref="RestSerializer"/> as <see cref="IRestSerializer"/> as singleton;
        ///     - HttpClient with name <see cref="TradingRestClient.HttpClientName"/>;
        ///     - <see cref="IMarketDataRestClient"/>.
        /// </summary>
        /// <param name="services">Service collection to add client to.</param>
        /// <param name="uri">Xena trading rest uri.</param>
        /// <returns>Populated service collection.</returns>
        public static IServiceCollection AddXenaMarketDataRestClient(
            this IServiceCollection services,
            string uri = "https://api.xena.exchange")
        {
            return services
                .AddSingleton<IRestSerializer, RestSerializer>()
                .AddTransient<IMarketDataRestClient, MarketDataRestClient>()
                .AddHttpClient(RestClientBase.HttpClientName, client => client.BaseAddress = new Uri(uri))
                .Services;
        }
    }
}