using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace XenaExchange.Client.Examples
{
    public static class Dependencies
    {
        public static IServiceCollection AddExamplesLogging(this IServiceCollection serviceCollection, LogLevel logLevel)
        {
            return serviceCollection.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.SetMinimumLevel(logLevel);
                    loggingBuilder.AddConsole();
                });
        }

        public static ILogger<T> ConsoleLogger<T>(LogLevel logLevel)
        {
            return new ServiceCollection()
                .AddExamplesLogging(logLevel)
                .BuildServiceProvider()
                .GetService<ILogger<T>>();
        }

        public static IHttpClientFactory CreateHttpClientFactory(string httpClientName, string uri)
        {
            return new ServiceCollection()
                .AddHttpClient(httpClientName, client => client.BaseAddress = new Uri(uri))
                .Services
                .BuildServiceProvider()
                .GetService<IHttpClientFactory>();
        }
    }
}