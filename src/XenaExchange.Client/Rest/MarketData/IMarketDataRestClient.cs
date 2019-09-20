using System;
using System.Threading;
using System.Threading.Tasks;
using Api;

namespace XenaExchange.Client.Rest.MarketData
{
    /// <summary>
    /// Xena market data rest client interface.
    /// </summary>
    public interface IMarketDataRestClient
    {
        /// <summary>
        /// Get candles for specified symbol, timeframe and time range.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="timeFrame">Timeframe.</param>
        /// <param name="from">Start date.</param>
        /// <param name="to">End date.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="MarketDataRefresh"/> with candles enumerated in <see cref="MarketDataRefresh.MDEntry"/>.</returns>
        Task<MarketDataRefresh> GetCandlesAsync(
            string symbol,
            string timeFrame,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get L2 snapshot for specified symbol.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="aggregation">DOM prices are rounded to TickSize*aggregation and than aggregated.
        /// Symbol TickSize could be obtained from https://trading.xena.exchange/en/platform-specification/instruments
        /// Available aggregation values are listed in DOMAggregation constants.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="MarketDataRefresh"/> with bids and asks enumerated in <see cref="MarketDataRefresh.MDEntry"/>.</returns>
        Task<MarketDataRefresh> GetDomAsync(
            string symbol,
            long aggregation = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// List spot and margin instruments.
        /// Check out Common.cs for available instruments constants.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>List of instruments.</returns>
        Task<Instrument[]> ListInstrumentsAsync(CancellationToken cancellationToken = default);
    }
}