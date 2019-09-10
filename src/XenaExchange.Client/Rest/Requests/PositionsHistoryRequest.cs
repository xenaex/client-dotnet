using System;

namespace XenaExchange.Client.Rest.Requests
{
    /// <summary>
    /// Positions history request.
    /// </summary>
    public class PositionsHistoryRequest
    {
        /// <summary>
        /// Account ID.
        /// </summary>
        public ulong Account { get; set; }

        /// <summary>
        /// ID of a concrete position to get an only one.
        /// </summary>
        public ulong? PositionId { get; set; }

        /// <summary>
        /// Parent position id to get all positions with specified parent.
        /// </summary>
        public ulong? ParentPositionId { get; set; }

        /// <summary>
        /// Symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Position open date "From".
        /// </summary>
        public DateTime? OpenFrom { get; set; }

        /// <summary>
        /// Position open date "To".
        /// </summary>
        public DateTime? OpenTo { get; set; }

        /// <summary>
        /// Position close date "From" (filters by SettlDate).
        /// </summary>
        public DateTime? CloseFrom { get; set; }

        /// <summary>
        /// Position close date "To" (filters by SettlDate).
        /// </summary>
        public DateTime? CloseTo { get; set; }

        /// <summary>
        /// Page number (pagination).
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Number of positions to get (pagination).
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Creates a new positions history request.
        /// </summary>
        /// <param name="account">Account ID.</param>
        /// <param name="positionId">ID of a concrete position to get an only one.</param>
        /// <param name="parentPositionId">Parent position id to get all positions with specified parent.</param>
        /// <param name="symbol">Symbol.</param>
        /// <param name="openFrom">Position open date "From".</param>
        /// <param name="openTo">Position open date "To".</param>
        /// <param name="closeFrom">Position close date "From" (filters by SettlDate).</param>
        /// <param name="closeTo">Position close date "To" (filters by SettlDate).</param>
        /// <param name="pageNumber">Page number (pagination).</param>
        /// <param name="limit">Number of positions to get (pagination).</param>
        public PositionsHistoryRequest(
            ulong account,
            ulong? positionId = null,
            ulong? parentPositionId = null,
            string symbol = null,
            DateTime? openFrom = null,
            DateTime? openTo = null,
            DateTime? closeFrom = null,
            DateTime? closeTo = null,
            int? pageNumber = null,
            int? limit = null)
        {
            Account = account;
            PositionId = positionId;
            ParentPositionId = parentPositionId;
            Symbol = symbol;
            OpenFrom = openFrom;
            OpenTo = openTo;
            CloseFrom = closeFrom;
            CloseTo = closeTo;
            PageNumber = pageNumber;
            Limit = limit;
        }
    }
}