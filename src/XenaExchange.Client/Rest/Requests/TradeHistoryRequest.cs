using System;

namespace XenaExchange.Client.Rest.Requests
{
    /// <summary>
    /// Trades history request.
    /// </summary>
    public class TradeHistoryRequest
    {
        /// <summary>
        /// Account ID.
        /// </summary>
        public ulong Account { get; set; }

        /// <summary>
        /// ID of a concrete trade to get an only one.
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// Client order id.
        /// </summary>
        public string ClOrdId { get; set; }

        /// <summary>
        /// Symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Trade date "From".
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Trade date "To".
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// Page number (pagination).
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Number of trades to get (pagination).
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="account">Account ID.</param>
        /// <param name="tradeId">ID of a concrete trade to get an only one.</param>
        /// <param name="clOrdId">Client order id.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="from">Trade date "From".</param>
        /// <param name="to">Trade date "To".</param>
        /// <param name="pageNumber">Page number (pagination).</param>
        /// <param name="limit">Number of positions to get (pagination).</param>
        public TradeHistoryRequest(
            ulong account,
            string tradeId = null,
            string clOrdId = null,
            string symbol = null,
            DateTime? from = null,
            DateTime? to = null,
            int? pageNumber = null,
            int? limit = null)
        {
            Account = account;
            TradeId = tradeId;
            ClOrdId = clOrdId;
            Symbol = symbol;
            From = from;
            To = to;
            PageNumber = pageNumber;
            Limit = limit;
        }
    }
}