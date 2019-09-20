namespace XenaExchange.Client.Messages.Constants
{
    public static class InstrumentType
    {
        public const string Spot = "Spot";
        public const string Margin = "Margin";
        public const string XenaListedPerpetual = "XenaListedPerpetual";
        public const string XenaFuture = "XenaFuture";
        public const string Index = "Index";
    }

    public static class AddUvmToFreeMargin
    {
        public const string No = "No";
        public const string LossOnly = "LossOnly";
        public const string ProfitAndLoss = "ProfitAndLoss";
    }

    public static class NettingType
    {
        public const string Off = "Off";
        public const string Positions = "Positions";
        public const string PositionsAndOrders = "PositionsAndOrders";
    }
}