namespace XenaExchange.Client.Messages.Constants
{
    /// <summary>
    /// Contains fix message types constants.
    /// </summary>
    public static class MsgTypes
    {
        public const string Heartbeat = "0";
        public const string Logon = "A";
        public const string OrderMassStatusResponse = "U8";
        public const string MarketDataRequest = "V";
        public const string MarketDataRequestReject = "Y";
        public const string MarketDataIncrementalRefresh = "X";
        public const string MarketDataSnapshotFullRefresh = "W";
        public const string AccountStatusReport = "XAR";
        public const string MarginRequirementReport = "CJ";
        public const string AccountStatusReportRequest = "XAA";
        public const string AccountStatusUpdateReport = "XAF";
        public const string PositionMaintenanceRequest = "AL";
        public const string PositionMaintenanceReport = "AM";
        public const string PositionReport = "AP";
        public const string OrderMassStatusRequest = "AF";
        public const string RequestForPositions = "AN";
        public const string MassPositionReport = "MAP";
        public const string NewOrderSingle = "D";
        public const string OrderCancelRequest = "F";
        public const string OrderCancelReplaceRequest = "G";
        public const string OrderCancelReject = "9";
        public const string ExecutionReport = "8";
        public const string Reject = "3";
        public const string ListStatus = "N";
        public const string OrderMassCancelRequest = "q";
        public const string OrderMassCancelReport = "r";
    }
}