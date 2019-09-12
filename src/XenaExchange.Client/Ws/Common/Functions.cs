using System;

namespace XenaExchange.Client.Ws.Common
{
    public static class Functions
    {
        private static readonly DateTime _epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long NowUnixNano()
        {
            return ToUnixNano(DateTime.UtcNow);
        }

        public static long ToUnixNano(this DateTime dateTime)
        {
            return (dateTime - _epochStart).Ticks * 100;
        }
    }
}