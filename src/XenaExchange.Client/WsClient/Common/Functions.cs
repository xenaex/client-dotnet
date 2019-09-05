using System;

namespace XenaExchange.Client.WsClient.Common
{
    public static class Functions
    {
        public static long NowUnixNano()
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow - epochStart).Ticks * 100;
        }
    }
}