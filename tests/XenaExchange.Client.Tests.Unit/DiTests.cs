using System;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using XenaExchange.Client.Rest.Trading;
using XenaExchange.Client.Serialization.Rest;

namespace XenaExchange.Client.Tests.Unit
{
    public class DiTests
    {
        [Test]
        public void NewTradingRestClient_UsingFactory()
        {
            var factory = new ServiceCollection()
                .AddHttpClient("123", client => client.BaseAddress = new Uri("http://123"))
                .Services
                .BuildServiceProvider()
                .GetService<IHttpClientFactory>();

            Action action = () => new TradingRestClient(
                new TradingRestClientOptions(),
                new RestSerializer(),
                httpClientFactory: factory);

            action.Should().NotThrow();
        }

        [Test]
        public void NewTradingRestClient_UsingClient()
        {
            Action action = () => new TradingRestClient(
                new TradingRestClientOptions(),
                new RestSerializer(),
                httpClient: new HttpClient());

            action.Should().NotThrow();
        }

        [Test]
        public void TradingRestClient_NoFactory_NoClient()
        {
            Action action = () => new TradingRestClient(new TradingRestClientOptions(), new RestSerializer());
            action.Should().Throw<ArgumentNullException>();
        }
    }
}