using System.Globalization;
using Api;
using XenaExchange.Client.Messages.Constants;

namespace XenaExchange.Client.Messages
{
    public static class MessagesExtensions
    {
        public static string ToFixString(this decimal d)
        {
            return d == 0 ? string.Empty : d.ToString(CultureInfo.InvariantCulture);
        }

        public static bool IsSnapshot(this MarketDataRefresh marketDataRefresh)
        {
            return marketDataRefresh.MsgType == MsgTypes.MarketDataSnapshotFullRefresh;
        }
    }
}