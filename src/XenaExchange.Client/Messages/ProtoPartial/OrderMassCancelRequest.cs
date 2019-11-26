using XenaExchange.Client.Messages;
using Constants = XenaExchange.Client.Messages.Constants;

namespace Api
{
    public partial class OrderMassCancelRequest
    {
        public OrderMassCancelRequest(ulong account, string clOrdId, string symbol, string side, string positionEffect)
        {
            MsgType = Constants.MsgTypes.OrderMassCancelRequest;
            Account = account;
            ClOrdId = clOrdId;
            Symbol = symbol.Proto();
            Side = side.Proto();
            PositionEffect = positionEffect.Proto();

            MassCancelRequestType = string.IsNullOrWhiteSpace(Symbol)
                ? Constants.MassCancelRequestType.CancelAllOrders
                : Constants.MassCancelRequestType.CancelOrdersForASecurity;
        }

        public void Validate()
        {
            Validator.NotNullOrEmpty(nameof(ClOrdId), ClOrdId);
            if (!string.IsNullOrWhiteSpace(Side))
                Validator.OneOf(nameof(Side), Side, Constants.Side.All);
            if (!string.IsNullOrWhiteSpace(PositionEffect))
                Validator.OneOf(nameof(PositionEffect), PositionEffect, Constants.PositionEffect.All);
        }
    }
}