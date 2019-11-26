using System.Collections.Generic;

namespace XenaExchange.Client.Messages.Constants
{
    public static class OrdType
    {
        /// <summary>
        /// Market order type is a order type for selling or buying instrument
        /// by current market prices.
        /// </summary>
        public const string Market = "1";

        /// <summary>
        /// Limit is a deferred order type. Orders by this type could be executed
        /// by best price or by order price.
        /// </summary>
        public const string Limit = "2";

        /// <summary>
        /// Stop is a deferred order type. It executed when current quotes achieve
        /// order's stop-price of this type. After execution order of this type will
        /// be converted to market order.
        /// </summary>
        public const string Stop = "3";

        /// <summary>
        /// StopLimit is as deferred order type. Almost the same as Stop order,
        /// except after execution it will be converted to market order with certain
        /// price.
        /// </summary>
        public const string StopLimit = "3";

        public const string MarketIfTouched = "J";

        public const string Pegged = "P";

        public static readonly IReadOnlyCollection<string> All = new[] { Market, Limit, Stop, StopLimit, MarketIfTouched, Pegged };
    }

    public static class OrdStatus
    {
        public const string NewOrd = "0";
        public const string PartiallyFilled = "1";
        public const string Filled = "2";
        public const string CanceledOrd = "4";
        public const string PendingCancelOrd = "6";
        public const string Stopped = "7";
        public const string RejectedOrd = "8";
        public const string Suspended = "9";
        public const string PendingNewOrd = "A";
        public const string Expired = "C";
        public const string PendingReplaceOrd = "E";
    }

    public static class Side
    {
        public const string Buy = "1";
        public const string Sell = "2";
        public static readonly IReadOnlyCollection<string> All = new[] { Buy, Sell };
    }

    public static class PositionEffect
    {
        public const string Close = "C";
        public const string Default = "D";
        public const string Open = "O";
        public static readonly IReadOnlyCollection<string> All = new[] { Close, Open, Default };
    }

    public static class PosTransType
    {
        public const string Collapse = "20";
        public static readonly IReadOnlyCollection<string> All = new[] { Collapse };
    }

    public static class PosMaintAction
    {
        public const string Replace = "2";
        public static readonly IReadOnlyCollection<string> All = new[] { Replace };
    }

    public static class ExecInst
    {
        public const string IgnoreNotionalValueChecks = "x";
        public const string Open = "O";
        public static readonly IReadOnlyCollection<string> All = new[] { IgnoreNotionalValueChecks, Open };
    }

    public static class PegOffsetType
    {
        public const string BasisPoints = "2"; // diverges with real fix value "1"
        public static readonly IReadOnlyCollection<string> All = new[] { BasisPoints };
    }

    public static class PegPriceType
    {
        public const string TrailingStopPeg = "8";
        public static readonly IReadOnlyCollection<string> All = new[] { TrailingStopPeg };
    }

    public static class TimeInForce
    {
        public const string GoodTillCancel = "1";
        public const string ImmediateOrCancel = "3";
        public const string FillOrKill = "4";
        public static readonly IReadOnlyCollection<string> All = new[] { GoodTillCancel, ImmediateOrCancel, FillOrKill };
    }

    public static class BusinessRejectReason
    {
        public const string ApplicationNotAvailable = "4";
        public const string ThrottleLimitExceeded = "8";
    }

    public static class MarginAmtType
    {
        public const string CoreMargin = "7";
        public const string InitialMargin = "11";
    }

    public static class MarginReqmtRptType
    {
        public const string SummaryType = "0";
    }

    public static class ExecType
    {
        public const string NewExec = "0";
        public const string CanceledExec = "4";
        public const string ReplacedExec = "5";
        public const string PendingCancelExec = "6";
        public const string RejectedExec = "8";
        public const string SuspendedExec = "9";
        public const string PendingNewExec = "A";
        public const string Restated = "D";
        public const string PendingReplaceExec = "E";
        public const string Trade = "F";
        public const string OrderStatus = "I";
    }

    public static class ExecRestatementReason
    {
        public const string RepricingOfOrder = "3";
    }

    public static class OrdRejReason
    {
        public const string UnknownSymbol = "1";
        public const string ExchangeClosed = "2";
        public const string OrderExceedsLimit = "3";
        public const string DuplicateOrder = "6";
        public const string UnsupportedOrderCharacteristic = "11";
        public const string IncorrectQuantity = "13";
        public const string UnknownAccount = "15";
        public const string PriceExceedsCurrentPriceBand = "16";
        public const string Other = "99";
        public const string StopPriceInvalid = "100";
    }

    public static class LiquidityInd
    {
        public const string AddedLiquidity = "1";
        public const string RemovedLiquidity = "2";
    }
    public static class SettlType
    {
        public const string Regular = "0";
        public const string Cash = "1";
    }

    public static class CxlRejResponseTo
    {
        public const string OrderCancelRequestCxlRejResponseTo = "1";
        public const string OrderCancelReplaceRequestResponseTo = "2";
    }

    public static class CxlRejReason
    {
        public const string TooLateToCancel = "0";
        public const string UnknownOrder = "1";
        public const string OrderAlreadyInPendingStatus = "3";
        public const string DuplicateClOrdId = "6";
        public const string OtherCxlRejReason = "99";
    }

    public static class BidType
    {
        public const string NonDisclosed = "1";
        public const string Disclosed = "2";
        public const string NoBiddingProcess = "3";
    }

    public static class ContingencyType
    {
        public const string OneCancelsTheOther = "1";
        public const string OneTriggersTheOther = "2";
        public const string OneUpdatesTheOtherAbsolute = "3";
        public const string OneUpdatesTheOtherProportional = "4";
    }

    public static class ListStatusType
    {
        public const string AckListStatusType = "1";
        public const string ResponseListStatusType = "2";
        public const string TimedListStatusType = "3";
        public const string ExecStartedListStatusType = "4";
        public const string AllDoneListStatusType = "5";
        public const string AlertListStatusType = "6";
    }

    public static class ListOrderStatus
    {
        public const string InBiddingProcessListOrderStatus = "1";
        public const string ReceivedForExecutionListOrderStatus = "2";
        public const string ExecutingListOrderStatus = "3";
        public const string CancellingListOrderStatus = "4";
        public const string AlertListOrderStatus = "5";
        public const string AllDoneListOrderStatus = "6";
        public const string RejectListOrderStatus = "7";
    }

    public static class ListRejectReason
    {
        public const string UnsupportedOrderCharacteristicListRejectReason = "11";
        public const string ExchangeClosedListRejectReason = "2";
        public const string TooLateToEnterListRejectReason = "4";
        public const string UnknownOrderListRejectReason = "5";
        public const string DuplicateOrderListRejectReason = "6";
        public const string OtherListRejectReason = "99";
    }

    public static class TriggerAction
    {
        public const string Activate = "1";
        public const string SetNewCapPrice = "4";
        public const string Transform = "3101";
    }

    public static class TriggerType
    {
        public const string PartialExecution = "1";
    }

    public static class TriggerScope
    {
        public const string OtherOrder = "1";
    }

    public static class PartieRole
    {
        public const string ClientId = "3";
        public const string ContraFirm = "17";
    }

    public static class AccountKind
    {
        public const string Spot = "Spot";
        public const string Margin = "Margin";
    }

    public static class MassCancelRequestType
    {
        public const string CancelOrdersForASecurity = "1";
        public const string CancelAllOrders = "7";
    }
}