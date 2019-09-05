using Api;

namespace XenaExchange.Client.WsClient.MarketData
{
    internal class Subscription
    {
        public MarketDataRequest Request { get; }

        public XenaMdWsHandler Handler { get; }

        public Subscription(MarketDataRequest request, XenaMdWsHandler handler)
        {
            Request = request;
            Handler = handler;
        }
    }
}