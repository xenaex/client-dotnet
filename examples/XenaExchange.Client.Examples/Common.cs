using System;

namespace XenaExchange.Client.Examples
{
    public static class CommonFuncs
    {
        public static string NewClOrdId(string prefix) => $"{prefix}-{DateTime.UtcNow.Ticks.ToString()}";
    }
}