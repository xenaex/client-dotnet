using System.Threading.Tasks;
using Api;
using Google.Protobuf;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Ws.Interfaces.Exceptions;

namespace XenaExchange.Client.Ws.Interfaces
{
    /// <summary>
    /// Xena Trading websocket client interface.
    ///
    /// All details about trading websocket API could be found here:
    /// https://support.xena.exchange/support/solutions/articles/44001795000-ws-trading-api
    /// </summary>
    public interface ITradingWsClient : IWsClient, IOnDisconnect<ITradingWsClient>
    {
        /// <summary>
        /// Connects to websocket and if connection was successful sends Logon message with provided authorization data.
        /// </summary>
        /// <returns>Logon response. If logon is rejected Logon.RejectText will contain the reject reason.</returns>
        /// <exception cref="WsNotConnectedException">Failed to connect to websocket.</exception>
        Task<Logon> ConnectAndLogonAsync();

        /// <summary>
        /// Starts to route all received from websocket messages to provided handler.
        ///
        /// Is is possible to listen all messages and any concrete message simultaneously.
        /// This way each message will be duplicated in a general handler and in a concrete handler.
        ///
        /// If an attempt to subscribe twice on all messages is made <see cref="DuplicateSubscriptionException"/> will be thrown.
        /// </summary>
        /// <param name="handler">Messages handler.</param>
        /// <exception cref="DuplicateSubscriptionException">Already subscribed on all messages.</exception>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        void Listen(XenaTradingWsHandler handler);

        /// <summary>
        /// Starts to route messages with specified type to provided handler.
        ///
        /// Is is possible to listen all messages and any concrete message simultaneously.
        /// This way each message will be duplicated in a general handler and in a concrete handler.
        ///
        /// If an attempt to subscribe twice on all messages is made <see cref="DuplicateSubscriptionException"/> will be thrown.
        /// </summary>
        /// <param name="handler">Messages handler.</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="DuplicateSubscriptionException">Already subscribed on specified message type.</exception>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        void Listen<T>(XenaTradingWsHandler<T> handler) where T : IMessage;

        /// <summary>
        /// Stops route messages with specified type to registered handler.
        /// </summary>
        /// <typeparam name="T">Message type to stop to listen.</typeparam>
        void RemoveListener<T>() where T : IMessage;

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
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task NewMarketOrderAsync(
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
            string text = null);

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
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task NewLimitOrderAsync(
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
            string text = null);

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
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task NewStopOrderAsync(
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
            string text = null);

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
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task NewMarketIfTouchOrderAsync(
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
            string text = null);

        /// <summary>
        /// Cancels an existing order by provided original client order id.
        /// </summary>
        /// <param name="clOrdId">Client id of canceling order.</param>
        /// <param name="origClOrdId">Client id of order to be canceled.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side of order to be canceled.</param>
        /// <param name="account">Account id.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task CancelOrderByClOrdIdAsync(string clOrdId, string origClOrdId, string symbol, string side, ulong account);

        /// <summary>
        /// Cancels an existing order by provided order id.
        /// </summary>
        /// <param name="clOrdId">Client id of canceling order.</param>
        /// <param name="orderId">Id of order to be canceled. Could be obtained from <see cref="ExecutionReport"/>.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="side">Side of order to be canceled.</param>
        /// <param name="account">Account id.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task CancelOrderByOrderIdAsync(string clOrdId, string orderId, string symbol, string side, ulong account);

        /// <summary>
        /// Cancels an existing order and replaces it with <paramref name="request"/>.
        /// </summary>
        /// <param name="request">Request containing all filled fields for replace.
        /// Could be obtained via <see cref="OrderExtensions.ToOrderCancelReplaceRequest"/>.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task CancelReplaceOrderAsync(OrderCancelReplaceRequest request);

        /// <summary>
        /// Collapses all existing positions for specified margin account and symbol.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="posReqId">Optional request id. If set, reply will contain the same id.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task CollapsePositionsAsync(ulong account, string symbol, string posReqId = null);

        /// <summary>
        /// Requests account status report.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="requestId">Request id to be returned in <see cref="BalanceSnapshotRefresh"/>.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task AccountStatusReportAsync(ulong account, string requestId = null);

        /// <summary>
        /// Requests all orders and fills for specified account.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="requestId">Request id to be returned in </param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task GetOrdersAndFillsAsync(ulong account, string requestId = null);

        /// <summary>
        /// Requests all positions for specified account.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="requestId">Request id to be returned in <see cref="MassPositionReport"/>.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task GetPositionsAsync(ulong account, string requestId = null);

        /// <summary>
        /// Creates OrderMassCancelRequest and send request.
        /// To receive response, client has to listen <see cref="OrderMassCancelReport"/>.
        /// </summary>
        /// <param name="account">Account id.</param>
        /// <param name="clOrdId">Client order id for current cancelling order.</param>
        /// <param name="symbol">Orders' symbol to cancel (optional).</param>
        /// <param name="side">Orders' side to cancel (optional).</param>
        /// <param name="positionEffect">Orders' position effect to cancel (optional).</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task OrderMassCancelAsync(
            ulong account,
            string clOrdId,
            string symbol = null,
            string side = null,
            string positionEffect = PositionEffect.Default);
    }
}