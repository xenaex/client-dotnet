using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Google.Protobuf;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest.Requests;
using XenaExchange.Client.Serialization.Rest;
using XenaExchange.Client.Signature;
using XenaExchange.Client.Ws.Common;

namespace XenaExchange.Client.Rest.Trading
{
    /// <summary>
    /// An implementation of Xena trading rest client.
    /// </summary>
    public class TradingRestClient : RestClientBase, ITradingRestClient
    {
        private const string TradingPrefix = "trading/";
        private const string NewOrderPath = TradingPrefix + "order/new";
        private const string CancelOrderPath = TradingPrefix + "order/cancel";
        private const string ReplaceOrderPath = TradingPrefix + "order/replace";
        private const string MassCancelPath = TradingPrefix + "order/mass-cancel";
        private const string HeartbeatPath = TradingPrefix + "order/heartbeat";
        private const string PositionMaintenancePath = TradingPrefix + "position/maintenance";
        private const string AccountsPath = TradingPrefix + "accounts";

        private const string BalancesPathTemplate = TradingPrefix + "accounts/{0}/balance";
        private const string MarginRequirementsPathTemplate = TradingPrefix + "accounts/{0}/margin-requirements";
        private const string OpenPositionsPathTemplate = TradingPrefix + "accounts/{0}/positions";
        private const string PositionsHistoryPathTemplate = TradingPrefix + "accounts/{0}/positions-history";
        private const string ActiveOrdersPathTemplate = TradingPrefix + "accounts/{0}/active-orders";
        private const string OrderPathTemplate = TradingPrefix + "accounts/{0}/order";
        private const string LastOrderStatusesPathTemplate = TradingPrefix + "accounts/{0}/last-order-statuses";
        private const string OrderHistoryPathTemplate = TradingPrefix + "accounts/{0}/order-history";
        private const string TradeHistoryPathTemplate = TradingPrefix + "accounts/{0}/trade-history";

        private readonly TradingRestClientOptions _options;
        private readonly IRestSerializer _serializer;

        /// <summary>
        /// Creates an instance of trading rest client.
        /// Either <paramref name="httpClientFactory"/> or <paramref name="httpClient"/> should be specified.
        /// If both are present, <paramref name="httpClientFactory"/> will be used.
        /// </summary>
        /// <param name="options">Trading rest client options.</param>
        /// <param name="serializer">Rest serializer.</param>
        /// <param name="httpClientFactory">Http client factory.</param>
        /// <param name="httpClient">Http client.</param>
        public TradingRestClient(
            TradingRestClientOptions options,
            IRestSerializer serializer,
            IHttpClientFactory httpClientFactory = null,
            HttpClient httpClient = null) : base(httpClientFactory, httpClient)
        {
            _options = options;
            _serializer = serializer;
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
            decimal stopLossPrice = 0,
            decimal takeProfitPrice = 0,
            string text = null,
            string groupId = null,
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
                takeProfitPrice,
                text,
                groupId);

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
            decimal stopLossPrice = 0,
            decimal takeProfitPrice = 0,
            decimal trailingOffset = 0,
            decimal capPrice = 0,
            string text = null,
            string groupId = null,
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
                capPrice,
                text,
                groupId);

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
            decimal stopLossPrice = 0,
            decimal takeProfitPrice = 0,
            decimal trailingOffset = 0,
            decimal capPrice = 0,
            string text = null,
            string groupId = null,
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
                capPrice,
                text,
                groupId);

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
            decimal stopLossPrice = 0,
            decimal takeProfitPrice = 0,
            decimal trailingOffset = 0,
            decimal capPrice = 0,
            string text = null,
            string groupId = null,
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
                capPrice,
                text,
                groupId);

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
            var response = await SendAsync<ListAccountsResponse>(AccountsPath, HttpMethod.Get, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

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
                parameters.Add("id=" + request.PositionId);
            if (request.ParentPositionId.HasValue)
                parameters.Add("parentid=" + request.ParentPositionId);
            if (!string.IsNullOrWhiteSpace(request.Symbol))
                parameters.Add("symbol=" + request.Symbol);
            if (request.OpenFrom.HasValue)
                parameters.Add("openfrom=" + request.OpenFrom.Value.ToUnixNano());
            if (request.OpenTo.HasValue)
                parameters.Add("opento=" + request.OpenTo.Value.ToUnixNano());
            if (request.CloseFrom.HasValue)
                parameters.Add("closefrom=" + request.CloseFrom.Value.ToUnixNano());
            if (request.CloseTo.HasValue)
                parameters.Add("closeto=" + request.CloseTo.Value.ToUnixNano());
            if (request.PageNumber.HasValue)
                parameters.Add("page=" + request.PageNumber);
            if (request.Limit.HasValue)
                parameters.Add("limit=" + request.Limit);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);
            return await ListPositionsInternalAsync(path, query, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport[]> ListActiveOrdersAsync(
            ulong account,
            string symbol = "",
            CancellationToken cancellationToken = default)
        {
            var path = string.Format(ActiveOrdersPathTemplate, account);

            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(symbol))
                parameters.Add("symbol=" + symbol);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);

            return await SendAsync<ExecutionReport[]>(
                path,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport> GetOrderAsync(
            ulong account,
            string orderId = "",
            string clOrdId = "",
            CancellationToken cancellationToken = default)
        {
            var path = string.Format(OrderPathTemplate, account);

            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(orderId))
                parameters.Add("order_id=" + orderId);
            if (!string.IsNullOrWhiteSpace(clOrdId))
                parameters.Add("client_order_id=" + clOrdId);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);

            return await SendAsync<ExecutionReport>(
                path,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport[]> GetLastOrderStatusesAsync(
             ulong accountId,
             string symbol = "",
             DateTime? from = null,
             DateTime? to = null,
             int? pageNumber = null,
             int? limit = null,
             CancellationToken cancellationToken = default)
        {
            var path = string.Format(LastOrderStatusesPathTemplate, accountId);

            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(symbol))
                parameters.Add("symbol=" + symbol);
            if (from.HasValue)
                parameters.Add("from=" + from);
            if (to.HasValue)
                parameters.Add("to=" + to);
            if (pageNumber.HasValue)
                parameters.Add("page=" + pageNumber);
            if (limit.HasValue)
                parameters.Add("limit=" + limit);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);

            return await SendAsync<ExecutionReport[]>(
                path,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// return list of historical execution reports.
        /// </summary>
        /// <param name="accountId">Account id.</param>
        /// <param name="symbol">Symbol id.</param>
        /// <param name="orderId">Order id.</param>
        /// <param name="clOrdId">Client order id.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="limit">Limit.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="ExecutionReport"/> array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>

        /// <inheritdoc />
        public async Task<ExecutionReport[]> GetOrderHistoryAsync(
            ulong accountId,
            string symbol = "",
            string orderId = "",
            string clOrdId = "",
            DateTime? from = null,
            DateTime? to = null,
            int? pageNumber = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {

            var path = string.Format(OrderHistoryPathTemplate, accountId);

            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(symbol))
                parameters.Add("symbol=" + symbol);
            if (from.HasValue)
                parameters.Add("from=" + from);
            if (to.HasValue)
                parameters.Add("to=" + to);
            if (pageNumber.HasValue)
                parameters.Add("page=" + pageNumber);
            if (limit.HasValue)
                parameters.Add("limit=" + limit);
            if (!string.IsNullOrWhiteSpace(orderId))
                parameters.Add("order_id=" + orderId);
            if (!string.IsNullOrWhiteSpace(clOrdId))
                parameters.Add("client_order_id=" + clOrdId);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);

            return await SendAsync<ExecutionReport[]>(
                path,
                HttpMethod.Get,
                query: query,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ExecutionReport[]> TradeHistoryAsync(
            TradeHistoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var path = string.Format(TradeHistoryPathTemplate, request.Account);
            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.TradeId))
                parameters.Add("trade_id=" + request.TradeId);
            if (!string.IsNullOrWhiteSpace(request.ClOrdId))
                parameters.Add("client_order_id=" + request.ClOrdId);
            if (!string.IsNullOrWhiteSpace(request.Symbol))
                parameters.Add("symbol=" + request.Symbol);
            if (request.From.HasValue)
                parameters.Add("from=" + request.From.Value.ToUnixNano());
            if (request.To.HasValue)
                parameters.Add("to=" + request.To.Value.ToUnixNano());
            if (request.PageNumber.HasValue)
                parameters.Add("page=" + request.PageNumber);
            if (request.Limit.HasValue)
                parameters.Add("limit=" + request.Limit);

            var query = parameters.Count == 0 ? null : string.Join("&", parameters);
            return await SendAsync<ExecutionReport[]>(
                    path,
                    HttpMethod.Get,
                    query: query,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<OrderMassCancelReport> OrderMassCancelAsync(
            ulong account,
            string clOrdId,
            string symbol = null,
            string side = null,
            string positionEffect = PositionEffect.Default,
            CancellationToken cancellationToken = default)
        {
            var command = new OrderMassCancelRequest(account, clOrdId, symbol, side, positionEffect);
            command.Validate();

            return await PostAsync<OrderMassCancelReport>(MassCancelPath, command, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SendApplicationHeartbeat(
            string groupId,
             int HeartBeatIntervalInSec,
              CancellationToken cancellationToken = default)
        {
            var command = new ApplicationHeartbeat
            {
                MsgType = MsgTypes.ApplicationHeartbeat,
                GrpID = groupId,
                HeartBtInt = HeartBeatIntervalInSec,
            };
            await SendWithoutResponceAsync(
                HeartbeatPath,
                HttpMethod.Post,
                command: command,
                cancellationToken: cancellationToken)
               .ConfigureAwait(false);
            return;
        }

        private async Task<PositionReport[]> ListPositionsInternalAsync(
            string path,
            string query = null,
            CancellationToken cancellationToken = default)
        {
            return await SendAsync<PositionReport[]>(
                    path,
                    HttpMethod.Get,
                    query: query,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<TResult> GetAsync<TResult>(
            string path,
            string query = null,
            CancellationToken cancellationToken = default)
            where TResult : IMessage
        {
            return await SendAsync<TResult>(path, HttpMethod.Get, query: query, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<TResult> PostAsync<TResult>(
            string path,
            IMessage command,
            CancellationToken cancellationToken = default)
            where TResult : IMessage
        {
            return await SendAsync<TResult>(
                    path,
                    HttpMethod.Post,
                    command: command,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<TResult> SendAsync<TResult>(
            string path,
            HttpMethod method,
            string query = null,
            IMessage command = null,
            CancellationToken cancellationToken = default)
        {
            var request = BuildRequestBase(path, method, query);

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

            return await SendAsyncBase<TResult>(request, _serializer, cancellationToken).ConfigureAwait(false);
        }

        private async Task SendWithoutResponceAsync(
            string path,
            HttpMethod method,
            string query = null,
            IMessage command = null,
            CancellationToken cancellationToken = default)
        {
            var request = BuildRequestBase(path, method, query);

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

            await SendAsyncBaseWithoutResponse(request, cancellationToken).ConfigureAwait(false);
            return;
        }
    }
}