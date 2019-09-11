using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest.Exceptions;
using XenaExchange.Client.Rest.Requests;
using XenaExchange.Client.Serialization;
using XenaExchange.Client.Serialization.Rest;
using XenaExchange.Client.Signature;
using XenaExchange.Client.Ws.Common;

namespace XenaExchange.Client.Rest
{
    /// <summary>
    /// An implementation of Xena trading rest client.
    /// </summary>
    public class TradingRestClient : ITradingRestClient
    {
        public const string HttpClientName = "xena.exchange";

        private const string TradingPrefix = "trading/";
        private const string NewOrderPath = TradingPrefix + "order/new";
        private const string CancelOrderPath = TradingPrefix + "order/cancel";
        private const string ReplaceOrderPath = TradingPrefix + "order/replace";
        private const string PositionMaintenancePath = TradingPrefix + "/position/maintenance";
        private const string AccountsPath = TradingPrefix + "accounts";

        private const string BalancesPathTemplate = TradingPrefix + "accounts/{0}/balance";
        private const string MarginRequirementsPathTemplate = TradingPrefix + "accounts/{0}/margin-requirements";
        private const string OpenPositionsPathTemplate = TradingPrefix + "accounts/{0}/positions";
        private const string PositionsHistoryPathTemplate = TradingPrefix + "accounts/{0}/positions-history";
        private const string ActiveOrdersPathTemplate = TradingPrefix + "accounts/{0}/orders";
        private const string TradeHistoryPathTemplate = TradingPrefix + "accounts/{0}/trade-history";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TradingRestClientOptions _options;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        private HttpClient HttpClient => _httpClientFactory.CreateClient(HttpClientName);

        public TradingRestClient(
            IHttpClientFactory httpClientFactory,
            TradingRestClientOptions options,
            IRestSerializer serializer,
            ILogger<TradingRestClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
            _serializer = serializer;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> NewOrderAsync(
            NewOrderSingle command,
            CancellationToken cancellationToken = default)
        {
            return await PostAsync<ExecutionReport>(NewOrderPath, command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> NewMarketOrderAsync(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            CancellationToken cancellationToken = default)
        {
            var command = OrderExtensions.NewMarketOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice);

            return await NewOrderAsync(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> NewLimitOrderAsync(string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal price,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0,
            CancellationToken cancellationToken = default)
        {
            var command = OrderExtensions.NewLimitOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                price,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice,
                trailingOffset,
                capPrice);

            return await NewOrderAsync(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> NewStopOrderAsync(string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal stopPx,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0,
            CancellationToken cancellationToken = default)
        {
            var command = OrderExtensions.NewStopOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                stopPx,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice,
                trailingOffset,
                capPrice);

            return await NewOrderAsync(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> NewMarketIfTouchOrderAsync(string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal stopPx,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0,
            CancellationToken cancellationToken = default)
        {
            var command = OrderExtensions.NewMarketIfTouchOrder(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                stopPx,
                timeInForce,
                execInst,
                positionId,
                stopLossPrice,
                takeProfitPrice,
                trailingOffset,
                capPrice);

            return await NewOrderAsync(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> CancelOrderAsync(
            OrderCancelRequest command,
            CancellationToken cancellationToken = default)
        {
            return await PostAsync<ExecutionReport>(CancelOrderPath, command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> CancelOrderByClOrdIdAsync(
            string clOrdId,
            string origClOrdId,
            string symbol,
            string side,
            ulong account,
            CancellationToken cancellationToken = default)
        {
            Validator.NotNullOrEmpty(nameof(clOrdId), clOrdId);
            Validator.NotNullOrEmpty(nameof(origClOrdId), origClOrdId);
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.OneOf(nameof(side), side, Side.All);

            var command = new OrderCancelRequest
            {
                MsgType = MsgTypes.OrderCancelRequest,
                ClOrdId = clOrdId,
                OrigClOrdId = origClOrdId,
                Symbol = symbol,
                Side = side,
                Account = account,
                TransactTime = Functions.NowUnixNano(),
            };

            return await CancelOrderAsync(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> CancelOrderByOrderIdAsync(
            string clOrdId,
            string orderId,
            string symbol,
            string side,
            ulong account,
            CancellationToken cancellationToken = default)
        {
            Validator.NotNullOrEmpty(nameof(clOrdId), clOrdId);
            Validator.NotNullOrEmpty(nameof(orderId), orderId);
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.OneOf(nameof(side), side, Side.All);

            var command = new OrderCancelRequest
            {
                MsgType = MsgTypes.OrderCancelRequest,
                ClOrdId = clOrdId,
                OrderId = orderId,
                Symbol = symbol,
                Side = side,
                Account = account,
                TransactTime = Functions.NowUnixNano(),
            };

            return await CancelOrderAsync(command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> ReplaceOrderAsync(
            OrderCancelReplaceRequest command,
            CancellationToken cancellationToken = default)
        {
            return await PostAsync<ExecutionReport>(ReplaceOrderPath, command, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<PositionMaintenanceReport> CollapsePositionsAsync(
            string requestId,
            ulong account,
            string symbol,
            CancellationToken cancellationToken = default)
        {
            Validator.NotNullOrEmpty(nameof(symbol), symbol);
            Validator.NotNullOrEmpty(nameof(requestId), requestId);

            var command = new PositionMaintenanceRequest(account, symbol, requestId);
            return await PostAsync<PositionMaintenanceReport>(
                PositionMaintenancePath,
                command,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<AccountInfo[]> ListAccountsAsync(CancellationToken cancellationToken = default)
        {
            var responseString = await SendAsync(AccountsPath, HttpMethod.Get, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var response = JsonConvert.DeserializeObject<ListAccountsResponse>(responseString);
            return response.Accounts;
        }

        /// <inheritdoc />
        public async Task<BalanceSnapshotRefresh> GetBalancesAsync(
            ulong account,
            CancellationToken cancellationToken = default)
        {
            return await GetAsync<BalanceSnapshotRefresh>(
                string.Format(BalancesPathTemplate, account),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<MarginRequirementReport> GetMarginRequirementsAsync(
            ulong account,
            CancellationToken cancellationToken = default)
        {
            return await GetAsync<MarginRequirementReport>(
                string.Format(MarginRequirementsPathTemplate, account),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<PositionReport[]> ListOpenPositionsAsync(
            ulong account,
            CancellationToken cancellationToken = default)
        {
            var path = string.Format(OpenPositionsPathTemplate, account);
            return await ListPositionsInternalAsync(path, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PositionReport[]> PositionsHistoryAsync(
            PositionsHistoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var path = string.Format(PositionsHistoryPathTemplate, request.Account);
            var parameters = new List<string>();
            if (request.PositionId.HasValue)
                parameters.Add("id="+request.PositionId);
            if (request.ParentPositionId.HasValue)
                parameters.Add("parentid="+request.ParentPositionId);
            if (!string.IsNullOrWhiteSpace(request.Symbol))
                parameters.Add("symbol="+request.Symbol);
            if (request.OpenFrom.HasValue)
                parameters.Add("openfrom="+request.OpenFrom.Value.ToUnixNano());
            if (request.OpenTo.HasValue)
                parameters.Add("opento="+request.OpenTo.Value.ToUnixNano());
            if (request.CloseFrom.HasValue)
                parameters.Add("closefrom="+request.CloseFrom.Value.ToUnixNano());
            if (request.CloseTo.HasValue)
                parameters.Add("closeto="+request.CloseTo.Value.ToUnixNano());
            if (request.PageNumber.HasValue)
                parameters.Add("page="+request.PageNumber);
            if (request.Limit.HasValue)
                parameters.Add("limit="+request.Limit);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);
            return await ListPositionsInternalAsync(path, query, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport[]> ListActiveOrdersAsync(
            ulong account,
            CancellationToken cancellationToken = default)
        {
            var path = string.Format(ActiveOrdersPathTemplate, account);
            var responseString = await SendAsync(path, HttpMethod.Get, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _serializer.Deserialize<ExecutionReport[]>(responseString);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport[]> TradeHistoryAsync(
            TradeHistoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var path = string.Format(TradeHistoryPathTemplate, request.Account);
            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.TradeId))
                parameters.Add("trade_id="+request.TradeId);
            if (!string.IsNullOrWhiteSpace(request.ClOrdId))
                parameters.Add("client_order_id="+request.ClOrdId);
            if (!string.IsNullOrWhiteSpace(request.Symbol))
                parameters.Add("symbol="+request.Symbol);
            if (request.From.HasValue)
                parameters.Add("from="+request.From.Value.ToUnixNano());
            if (request.To.HasValue)
                parameters.Add("to="+request.To.Value.ToUnixNano());
            if (request.PageNumber.HasValue)
                parameters.Add("page="+request.PageNumber);
            if (request.Limit.HasValue)
                parameters.Add("limit="+request.Limit);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);
            var responseString = await SendAsync(path, HttpMethod.Get, query: query, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _serializer.Deserialize<ExecutionReport[]>(responseString);
        }

        private async Task<PositionReport[]> ListPositionsInternalAsync(
            string path,
            string query = null,
            CancellationToken cancellationToken = default)
        {
            var responseString = await SendAsync(path, HttpMethod.Get, query: query, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _serializer.Deserialize<PositionReport[]>(responseString);
        }

        private async Task<TResult> GetAsync<TResult>(
            string path,
            string query = null,
            CancellationToken cancellationToken = default)
            where TResult : IMessage
        {
            var responseStr = await SendAsync(path, HttpMethod.Get, query: query, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _serializer.Deserialize<TResult>(responseStr);
        }

        private async Task<TResult> PostAsync<TResult>(
            string path,
            IMessage command,
            CancellationToken cancellationToken = default)
            where TResult: IMessage
        {
            var responseStr = await SendAsync(path, HttpMethod.Post, command: command, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _serializer.Deserialize<TResult>(responseStr);
        }

        private async Task<string> SendAsync(
            string path,
            HttpMethod method,
            string query = null,
            IMessage command = null,
            CancellationToken cancellationToken = default)
        {
            var httpClient = HttpClient;
            var uriBuilder = new UriBuilder(httpClient.BaseAddress) { Path = path };
            if (!string.IsNullOrWhiteSpace(query))
                uriBuilder.Query = query;

            var request = new HttpRequestMessage(method, uriBuilder.Uri);

            var nonce = (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() * 1000000).ToString();
            var authPayload = $"AUTH{nonce}";
            var signature = XenaSignature.Sign(_options.ApiSecret, authPayload);

            request.Headers.Add("X-AUTH-API-KEY", _options.ApiKey);
            request.Headers.Add("X-AUTH-API-PAYLOAD", authPayload);
            request.Headers.Add("X-AUTH-API-SIGNATURE", signature);
            request.Headers.Add("X-AUTH-API-NONCE", nonce);

            if (command != null)
            {
                var payload = _serializer.Serialize(command);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                case var _ when response.StatusCode < HttpStatusCode.BadRequest:
                    throw new RestClientException(
                        $"Only {HttpStatusCode.OK} successful code is supported",
                        response.StatusCode);

                default:
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var error = content;

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            error = JsonConvert.DeserializeObject<ErrorResponse>(content)?.Error;
                        }
                        catch (Exception) {} // If it's not a json, ok, let's return content as is
                    }

                    throw new RestClientException(error, response.StatusCode);
            }
        }
    }
}