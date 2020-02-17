using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest.Requests;
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
        private const string ServerTimePath = MdPrefix + "server-time";
        private const string TradesBasePath = MdPrefix + "trades";

        private readonly IFixSerializer _fixSerializer;
        private readonly IRestSerializer _restSerializer;

        /// <summary>
        /// Creates an instance of market data rest client.
        /// Either <paramref name="httpClientFactory"/> or <paramref name="httpClient"/> should be specified.
        /// If both are present, <paramref name="httpClientFactory"/> will be used.
        /// </summary>
        /// <param name="fixSerializer">Fix serializer.</param>
        /// <param name="restSerializer">Rest serializer.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="httpClient">Http client.</param>
        public MarketDataRestClient(
            IFixSerializer fixSerializer,
            IRestSerializer restSerializer,
            IHttpClientFactory httpClientFactory = null,
            HttpClient httpClient = null) : base(httpClientFactory, httpClient)
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
        public async Task<MarketDataRefresh> GetDomAsync(string symbol, long depth = 0, long aggregation = 0, CancellationToken cancellationToken = default)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);

            var parameters = new List<string>();
            parameters.Add($"depth={depth}");
            parameters.Add($"aggr={aggregation}");

            string query = null;
            query = string.Join("&", parameters);

            var path = $"{DomBasePath}/{symbol}";
            return await GetAsync<MarketDataRefresh>(path, _fixSerializer, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Instrument[]> ListInstrumentsAsync(CancellationToken cancellationToken = default)
        {
            return await GetAsync<Instrument[]>(InstrumentPath, _restSerializer, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<DateTime> GetServerTimeAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetAsync<Heartbeat>(ServerTimePath, _fixSerializer, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return Functions.FromUnixNano(response.TransactTime);
        }

        public async Task<MarketDataRefresh> TradeHistoryAsync(TradeHistoryMdRequest request, CancellationToken cancellationToken = default)
        {
            Validator.NotNull(nameof(request), request);
            Validator.NotNullOrEmpty(nameof(request.Symbol), request.Symbol);

            var path = $"{TradesBasePath}/{request.Symbol}";
            var parameters = new List<string>();
            if (request.From.HasValue)
                parameters.Add($"from={request.From.Value.ToUnixNano()}");
            if (request.To.HasValue)
                parameters.Add($"to={request.To.Value.ToUnixNano()}");
            if (request.PageNumber.HasValue)
                parameters.Add($"page={request.PageNumber}");
            if (request.Limit.HasValue)
                parameters.Add($"limit={request.Limit}");

            string query = null;
            if (parameters.Count > 0)
                query = string.Join("&", parameters);

            return await GetAsync<MarketDataRefresh>(
                path,
                _fixSerializer,
                query: query,
                cancellationToken: cancellationToken).ConfigureAwait(false);
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