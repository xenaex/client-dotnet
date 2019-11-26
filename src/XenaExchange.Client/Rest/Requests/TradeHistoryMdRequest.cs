using System;

namespace XenaExchange.Client.Rest.Requests
{
    /// <summary>
    /// List trades request
    /// </summary>
    public class TradeHistoryMdRequest
    {
        /// <summary>
        /// Symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Date "From".
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Date "To".
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
        /// Creates new instance of trade history market data request.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="from">Date from.</param>
        /// <param name="to">Date to.</param>
        /// <param name="pageNumber">Page number (pagination).</param>
        /// <param name="limit">Number of trades to get (pagination).</param>
        public TradeHistoryMdRequest(
            string symbol,
            DateTime? from = null,
            DateTime? to = null,
            int? pageNumber = null,
            int? limit = null)
        {
            Symbol = symbol;
            From = from;
            To = to;
            PageNumber = pageNumber;
            Limit = limit;
        }
    }
}