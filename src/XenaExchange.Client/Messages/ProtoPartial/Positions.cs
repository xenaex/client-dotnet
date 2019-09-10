using XenaExchange.Client.Messages.Constants;
using ClientConstants = XenaExchange.Client.Messages.Constants;

namespace Api
{
    public partial class PositionMaintenanceRequest
    {
        public PositionMaintenanceRequest(ulong accountId, string symbol, string requestId = null)
        {
            MsgType = ClientConstants.MsgTypes.PositionMaintenanceRequest;
            Account = accountId;
            Symbol = symbol;
            PosReqId = string.IsNullOrWhiteSpace(requestId) ? "" : requestId;
            PosTransType = ClientConstants.PosTransType.Collapse;
            PosMaintAction = ClientConstants.PosMaintAction.Replace;
        }
    }
}