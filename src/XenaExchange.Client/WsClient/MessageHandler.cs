using System.Threading.Tasks;
using Google.Protobuf;
using XenaExchange.Client.WsClient.Interfaces;

namespace XenaExchange.Client.WsClient
{
    public delegate Task XenaTradingWsHandler<in TData>(ITradingWsClient wsClient, TData data)
        where TData : IMessage;

    public delegate Task XenaMdWsHandler(IMarketDataWsClient wsClient, IMessage data);

    public delegate Task XenaTradingWsHandler(ITradingWsClient wsClient, IMessage data);
}