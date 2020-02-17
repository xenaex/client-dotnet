using System.Threading.Tasks;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Ws.Interfaces.Exceptions;

namespace XenaExchange.Client.Ws.Interfaces
{
    /// <summary>
    /// Xena Market Data websocket client interface.
    ///
    /// All details about market data websocket API could be found here:
    /// https://support.xena.exchange/support/solutions/articles/44001794992-market-data-api
    /// </summary>
    public interface IMarketDataWsClient : IWsClient, IOnDisconnect<IMarketDataWsClient>
    {
        /// <summary>
        /// Connects to websocket.
        /// </summary>
        /// <exception cref="WsNotConnectedException">Failed to connect to websocket.</exception>
        Task ConnectAsync();

        /// <summary>
        /// Subscribes to candles stream.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="timeframe">Candles timeframe. Available are listed in <see cref="CandlesTimeframe"/> constants.</param>
        /// <param name="handler">Messages handler. Possible types to handle are:
        ///     - MarketDataRefresh with MsgType=MsgTypes.MarketDataSnapshotFullRefresh ("W") or MarketDataIncrementalRefresh ("X");
        ///     - MarketDataRequestReject.</param>
        /// <param name="throttlingMs">Throttling interval. Default is 0 - every single update is sent to stream.
        /// Available values are listed in ThrottlingMs.Candles constants.</param>
        /// <returns>Stream id which can be used later to unsubscribe.</returns>
        /// <exception cref="DuplicateSubscriptionException">Already subscribed on candles stream with provided symbol.</exception>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task<string> SubscribeCandlesAsync(
            string symbol,
            string timeframe,
            XenaMdWsHandler handler,
            long throttlingMs = 0);

        /// <summary>
        /// Subscribes to depth of market stream.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="handler">Messages handler. Possible types to handle are:
        ///     - MarketDataRefresh with MsgType=MsgTypes.MarketDataSnapshotFullRefresh ("W") or MarketDataIncrementalRefresh ("X");
        ///     - MarketDataRequestReject.</param>
        /// <param name="throttlingMs">Throttling interval. Default is 0 - every single update is sent to stream.
        /// Available values are listed in ThrottlingMs.DOM constants.</param>
        /// <param name="aggregation">DOM prices are rounded to TickSize*aggregation and than aggregated.
        /// Symbol TickSize could be obtained from https://trading.xena.exchange/en/platform-specification/instruments
        /// Available aggregation values are listed in DOMAggregation constants.</param>
        /// <returns>Stream id which can be used later to unsubscribe.</returns>
        /// <exception cref="DuplicateSubscriptionException">Already subsribed on DOM stream with provided symbol.</exception>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task<string> SubscribeDOMAggregatedAsync(string symbol, XenaMdWsHandler handler, long throttlingMs = 0, long aggregation = 0, long depth = 0);

        /// <summary>
        /// Subscribes to market-watch stream.
        /// </summary>
        /// <param name="handler">Messages handler. Possible types to handle are:
        ///     - MarketDataRefresh with MsgType=MsgTypes.MarketDataSnapshotFullRefresh ("W");
        ///     - MarketDataRequestReject.</param>
        /// <returns>Stream id which can be used later to unsubscribe.</returns>
        /// <exception cref="DuplicateSubscriptionException">Already subsribed on market-watch stream.</exception>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task<string> SubscribeMarketWatchAsync(XenaMdWsHandler handler);

        /// <summary>
        /// Subscribes on trade reports stream.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="handler">Messages handler. Possible types to handle are:
        ///     - MarketDataRefresh with MsgType=MsgTypes.MarketDataSnapshotFullRefresh ("W") or MarketDataIncrementalRefresh ("X");
        ///     - MarketDataRequestReject.</param>
        /// <param name="throttlingMs">Throttling interval. Default is 0 - every single update is sent to stream.
        /// Available values are listed in ThrottlingMs.DOM constants.</param>
        /// <returns>Stream id which can be used later to unsubscribe.</returns>
        /// <exception cref="DuplicateSubscriptionException">Already subsribed on trade reports stream with provided symbol.</exception>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task<string> SubscribeTradeReportsAsync(string symbol, XenaMdWsHandler handler, long throttlingMs = 0);

        /// <summary>
        /// Unsubscribe from stream by provided stream id.
        /// </summary>
        /// <param name="subscriptionId">Stream id to unsubscribe.</param>
        /// <exception cref="WsNotConnectedException">No websocket connection with server.</exception>
        Task Unsubscribe(string subscriptionId);
    }
}