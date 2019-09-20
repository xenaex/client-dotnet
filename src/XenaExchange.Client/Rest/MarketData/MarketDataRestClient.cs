using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Serialization;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Serialization.Rest;
using XenaExchange.Client.Ws.Common;

namespace XenaExchange.Client.Rest.MarketData
{
    /// <summary>
    /// An implementation of Xena trading rest client.
    /// </summary>
    public class MarketDataRestClient : RestClientBase, IMarketDataRestClient
    {
        private const string MdPrefix = "market-data/";
        private const string CandlesBasePath = MdPrefix + "candles";
        private const string DomBasePath = MdPrefix + "dom";
        private const string InstrumentPath = "public/instruments";

        private readonly IFixSerializer _fixSerializer;
        private readonly IRestSerializer _restSerializer;

        public MarketDataRestClient(
            IHttpClientFactory httpClientFactory,
            IFixSerializer fixSerializer,
            IRestSerializer restSerializer) : base(httpClientFactory)
        {
            _fixSerializer = fixSerializer;
            _restSerializer = restSerializer;
        }

        /// <inheritdoc />
        public async Task<MarketDataRefresh> GetCandlesAsync(
            string symbol,
            string timeFrame,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.OneOf(nameof(timeFrame), timeFrame, CandlesTimeframe.All);

            var path = $"{CandlesBasePath}/{symbol}/{timeFrame}";
            var parameters = new List<string>();
            if (from.HasValue)
                parameters.Add($"from={from.Value.ToUnixNano()}");
            if (to.HasValue)
                parameters.Add($"to={to.Value.ToUnixNano()}");

            string query = null;
            if (parameters.Count > 0)
                query = string.Join("&", parameters);

            return await GetAsync<MarketDataRefresh>(
                    path,
                    _fixSerializer,
                    query: query,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<MarketDataRefresh> GetDomAsync(
            string symbol,
            long aggregation = 0,
            CancellationToken cancellationToken = default)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.GrThanOrEq(nameof(aggregation), aggregation, 0);

            var path = $"{DomBasePath}/{symbol}";
            var query = aggregation == 0 ? null : $"aggr={aggregation}";

            return await GetAsync<MarketDataRefresh>(
                    path,
                    _fixSerializer,
                    query: query,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Instrument[]> ListInstrumentsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAsync<Instrument[]>(InstrumentPath, _restSerializer, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<TResult> GetAsync<TResult>(
            string path,
            ISerializer serializer,
            string query = null,
            CancellationToken cancellationToken = default)
        {
            var request = BuildRequestBase(path, HttpMethod.Get, query);
            return await SendAsyncBase<TResult>(request, serializer, cancellationToken).ConfigureAwait(false);
        }
    }
}