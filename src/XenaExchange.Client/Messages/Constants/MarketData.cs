using System.Collections.Generic;

namespace XenaExchange.Client.Messages.Constants
{
    public static class CandlesTimeframe
    {
        public const string Timeframe1m = "1m";
        public const string Timeframe5m = "5m";
        public const string Timeframe15m = "15m";
        public const string Timeframe30m = "30m";
        public const string Timeframe1h = "1h";
        public const string Timeframe4h = "4h";
        public const string Timeframe12h = "12h";
        public const string Timeframe1d = "24h";
        public const string Timeframe1w = "168h";
        public static readonly IReadOnlyCollection<string> All = new[]
        {
            Timeframe1m,
            Timeframe5m,
            Timeframe15m,
            Timeframe30m,
            Timeframe1h,
            Timeframe4h,
            Timeframe12h,
            Timeframe1d,
            Timeframe1w,
        };
    }

    public static class SubscriptionRequestType
    {
        public const string SnapshotAndUpdates = "1";
        public const string DisablePreviousSnapshot = "2";
        public static readonly IReadOnlyCollection<string> All = new[] { SnapshotAndUpdates, DisablePreviousSnapshot };
    }

    public static class ThrottleType
    {
        public const string InboundRate = "0";
        public const string OutstandingRequests = "1";
        public static readonly IReadOnlyCollection<string> All = new[] { InboundRate, OutstandingRequests };
    }

    public static class ThrottleTimeUnit
    {
        public const string Seconds = "0";
        public const string TenthsOfASecond = "1";
        public const string HundredthsOfASecond = "2";
        public const string Milliseconds = "3";
        public const string Microseconds = "4";
        public const string Nanoseconds = "5";
        public const string Minutes = "10";
        public const string Hours = "11";
    }

    /// <summary>
    /// Defines supported by Xena throttling intervals.
    /// </summary>
    public static class ThrottlingMs
    {
        public static class DOM
        {
            public const long Throttling0 = 0;
            public const long Throttling500ms = 500;
            public const long Throttling5s = 5000;

        }

        public static class Candles
        {
            public const long Throttling0 = 0;
            public const long Throttling250ms = 250;
            public const long Throttling1s = 1000;
        }

        public static class Trades
        {
            public const long Throttling0 = 0;
            public const long Throttling500ms = 500;
            public const long Throttling5s = 5000;
        }
    }
}