using System;
using System.Threading;
using System.Threading.Tasks;
using Api;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest.Exceptions;
using XenaExchange.Client.Rest.Requests;

namespace XenaExchange.Client.Rest.Trading
{
    /// <summary>
    /// Xena trading rest client interface.
    /// </summary>
    public interface ITradingRestClient
    {
        /// <summary>
        /// Places new order.
        /// </summary>
        /// <param name="command">New order command.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>New order execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> NewOrderAsync(NewOrderSingle command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Places new market order.
        /// </summary>
        /// <param name="clOrdId">Client order id. 20 symbols max. Must be unique within a moving frame of 24 hours.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side (Side constants).</param>
        /// <param name="orderQty">Order quantity.</param>
        /// <param name="account">Account id.</param>
        /// <param name="timeInForce">Time in force (TimeInForce constants).</param>
        /// <param name="execInst">ExecInst list (ExecInst constants).</param>
        /// <param name="positionId">Position id to close.</param>
        /// <param name="stopLossPrice">Stop loss price.</param>
        /// <param name="takeProfitPrice">Take-profit price.</param>
        /// <param name="text">Free format text string.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>New order execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> NewMarketOrderAsync(
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Places new limit order.
        /// </summary>
        /// <param name="clOrdId">Client order id. 20 symbols max. Must be unique within a moving frame of 24 hours.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side (Side constants).</param>
        /// <param name="orderQty">Order quantity.</param>
        /// <param name="account">Account id.</param>
        /// <param name="price">Limit order price.</param>
        /// <param name="timeInForce">Time in force (TimeInForce constants).</param>
        /// <param name="execInst">ExecInst list (ExecInst constants).</param>
        /// <param name="positionId">Position id to close.</param>
        /// <param name="stopLossPrice">Stop loss price.</param>
        /// <param name="takeProfitPrice">Take-profit price.</param>
        /// <param name="trailingOffset">Trailing offset value. For trailing stop and attempt zero loss orders.</param>
        /// <param name="capPrice">For trailing stop orders — empty. For attempt zero loss orders — stop loss price limit.
        /// If CapPrice = 0, CapPrice = Open price.</param>
        /// <param name="text">Free format text string.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>New order execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> NewLimitOrderAsync(
            string clOrdId,
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Places new stop order.
        /// </summary>
        /// <param name="clOrdId">Client order id. 20 symbols max. Must be unique within a moving frame of 24 hours.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side (Side constants).</param>
        /// <param name="orderQty">Order quantity.</param>
        /// <param name="account">Account id.</param>
        /// <param name="stopPx">Stop order price.</param>
        /// <param name="timeInForce">Time in force (TimeInForce constants).</param>
        /// <param name="execInst">ExecInst list (ExecInst constants).</param>
        /// <param name="positionId">Position id to close.</param>
        /// <param name="stopLossPrice">Stop loss price.</param>
        /// <param name="takeProfitPrice">Take-profit price.</param>
        /// <param name="trailingOffset">Trailing offset value. For trailing stop and attempt zero loss orders.</param>
        /// <param name="capPrice">For trailing stop orders — empty. For attempt zero loss orders — stop loss price limit.
        /// If CapPrice = 0, CapPrice = Open price.</param>
        /// <param name="text">Free format text string.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>New order execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> NewStopOrderAsync(
            string clOrdId,
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Places new market-if-touch order.
        /// </summary>
        /// <param name="clOrdId">Client order id. 20 symbols max. Must be unique within a moving frame of 24 hours.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side (Side constants).</param>
        /// <param name="orderQty">Order quantity.</param>
        /// <param name="account">Account id.</param>
        /// <param name="stopPx">Stop order price.</param>
        /// <param name="timeInForce">Time in force (TimeInForce constants).</param>
        /// <param name="execInst">ExecInst list (ExecInst constants).</param>
        /// <param name="positionId">Position id to close.</param>
        /// <param name="stopLossPrice">Stop loss price.</param>
        /// <param name="takeProfitPrice">Take-profit price.</param>
        /// <param name="trailingOffset">Trailing offset value. For trailing stop and attempt zero loss orders.</param>
        /// <param name="capPrice">For trailing stop orders — empty. For attempt zero loss orders — stop loss price limit.
        /// If CapPrice = 0, CapPrice = Open price.</param>
        /// <param name="text">Free format text string.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>New order execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> NewMarketIfTouchOrderAsync(
            string clOrdId,
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels an existing order.
        /// </summary>
        /// <param name="command">Cancellation command.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Order cancel execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> CancelOrderAsync(
            OrderCancelRequest command,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels an existing order by provided original client order id.
        /// </summary>
        /// <param name="clOrdId">Client id of canceling order.</param>
        /// <param name="origClOrdId">Client id of order to be canceled.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side of order to be canceled.</param>
        /// <param name="account">Account id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Order cancel execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> CancelOrderByClOrdIdAsync(
            string clOrdId,
            string origClOrdId,
            string symbol,
            string side,
            ulong account,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels an existing order by provided order id.
        /// </summary>
        /// <param name="clOrdId">Client id of canceling order.</param>
        /// <param name="orderId">Id of order to be canceled. Could be obtained from <see cref="ExecutionReport"/>.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side of order to be canceled.</param>
        /// <param name="account">Account id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Order cancel execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> CancelOrderByOrderIdAsync(
            string clOrdId,
            string orderId,
            string symbol,
            string side,
            ulong account,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels an existing order and replaces it with <paramref name="command"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Order replace execution report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> ReplaceOrderAsync(
            OrderCancelReplaceRequest command,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Collapses all existing positions for specified margin account and symbol.
        /// </summary>
        /// <param name="posReqId">Request id. Reply will contain the same id.</param>
        /// <param name="account">Account id.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Position maintenance report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<PositionMaintenanceReport> CollapsePositionsAsync(
            string posReqId,
            ulong account,
            string symbol,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists authorized accounts.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="AccountInfo"/>  array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<AccountInfo[]> ListAccountsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns account balances.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Balance snapshot refresh.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<BalanceSnapshotRefresh> GetBalancesAsync(ulong account, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests margin requirements for an account.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>Get requirements report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<MarginRequirementReport> GetMarginRequirementsAsync(
            ulong account,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists account's open positions.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="PositionReport"/> array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<PositionReport[]> ListOpenPositionsAsync(ulong account, CancellationToken cancellationToken = default);

        /// <summary>
        /// Filters and lists positions history.
        /// </summary>
        /// <param name="request">Positions history request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="PositionReport"/> array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<PositionReport[]> PositionsHistoryAsync(
            PositionsHistoryRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists account's active orders.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="symbol">Symbol id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="ExecutionReport"/> array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport[]> ListActiveOrdersAsync(
            ulong account,
            string symbol = "",
            CancellationToken cancellationToken = default);


        /// <summary>
        /// returns last execution report for order or cancel/replace request.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="orderId">Order id.</param>
        /// <param name="clOrdId">Client order id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="ExecutionReport"/> array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport> GetOrderAsync(
            ulong account,
            string orderId = "",
            string clOrdId = "",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// return list of last execution reports for non-active orders.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="orderId">Order id.</param>
        /// <param name="clOrdId">Client order id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="ExecutionReport"/> array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport[]> GetLastOrderStatusesAsync(
            ulong account,
            string symbol = "",
            DateTime? from = null,
            DateTime? to = null,
            int? pageNumber = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// return list of historical execution reports.
        /// </summary>
        /// <param name="account">Account id.</param>
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
        Task<ExecutionReport[]> GetOrderHistoryAsync(
            ulong account,
            string symbol = "",
            string orderId = "",
            string clOrdId = "",
            DateTime? from = null,
            DateTime? to = null,
            int? pageNumber = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Filters and lists trade history.
        /// </summary>
        /// <param name="request">Trade history request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="ExecutionReport"/> array.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<ExecutionReport[]> TradeHistoryAsync(
            TradeHistoryRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates OrderMassCancelRequest and send request.
        /// To receive response, client has to listen <see cref="OrderMassCancelReport"/>.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="clOrdId">Client order id for current cancelling order.</param>
        /// <param name="symbol">Orders' symbol to cancel (optional).</param>
        /// <param name="side">Orders' side to cancel (optional).</param>
        /// <param name="positionEffect">Orders' position effect to cancel (optional).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="OrderMassCancelReport"/>Order mass cancel report.</returns>
        /// <exception cref="RestClientException">Any HTTP status code other than 200 OK.</exception>
        Task<OrderMassCancelReport> OrderMassCancelAsync(ulong account,
            string clOrdId,
            string symbol = null,
            string side = null,
            string positionEffect = PositionEffect.Default,
            CancellationToken cancellationToken = default);

        /// <summary>SendApplicationHeartbeat sends application heartbeat.</summary>
        /// <param name="groupId">Group id.</param>
        /// <param name="HeartBeatIntervalInSec">HeartBeat interval in seconds.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task SendApplicationHeartbeat(
            string groupId,
            int HeartBeatIntervalInSec,
            CancellationToken cancellationToken = default);
    }
}