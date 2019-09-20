using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Rest.Exceptions;
using XenaExchange.Client.Rest.MarketData;

namespace XenaExchange.Client.Examples.Rest
{
    public class MarketDataRestExample
    {
        private readonly IMarketDataRestClient _restClient;
        private readonly ILogger _logger;

        public MarketDataRestExample(IMarketDataRestClient restClient, ILogger<MarketDataRestExample> logger)
        {
            _restClient = restClient;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await TestMarketDataAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (RestClientException e)
            {
                _logger.LogError(e, $"Rest client exception: {e.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task TestMarketDataAsync(CancellationToken cancellationToken)
        {
            await TestGetCandlesAsync(cancellationToken).ConfigureAwait(false);
            await TestGetDomAsync(cancellationToken).ConfigureAwait(false);
            await TestListInstrumentsAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task TestGetCandlesAsync(CancellationToken cancellationToken)
        {
            var mdRefresh = await _restClient.GetCandlesAsync(
                    "BTC/USDT",
                    CandlesTimeframe.Timeframe1h,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation($"Candles: {mdRefresh}");
        }

        private async Task TestGetDomAsync(CancellationToken cancellationToken)
        {
            var symbol = "XBTUSD";
            var mdRefresh = await _restClient.GetDomAsync(symbol, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var mdEntry in mdRefresh.MDEntry)
            {
                if (mdEntry.MDEntryType == MDEntryType.Bid)
                    _logger.LogInformation($"{symbol} DOM Bid: {mdEntry}");
                if (mdEntry.MDEntryType == MDEntryType.Offer)
                    _logger.LogInformation($"{symbol} DOM Ask: {mdEntry}");
            }
        }

        private async Task TestListInstrumentsAsync(CancellationToken cancellationToken)
        {
            var instruments = await _restClient.ListInstrumentsAsync(cancellationToken).ConfigureAwait(false);

            var toPrint = string.Join('\n', instruments.Select(i => i.ToString()));
            _logger.LogInformation($"Instruments:\n{toPrint}");
        }
    }
}