using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace XenaExchange.Client.Websocket.Examples
{
    public static class Logging
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
    }
}