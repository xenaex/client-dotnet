using System.Linq;
using Api;
using XenaExchange.Client.Websocket.Client.Common;
using XenaExchange.Client.Websocket.Messages;
using XenaExchange.Client.Websocket.Messages.Constants;

namespace XenaExchange.Client.Websocket
{
    /// <summary>
    /// Helper-class with more convenient orders creation methods.
    /// </summary>
    public static class OrderExtensions
    {
        public static NewOrderSingle NewOrderSingle(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            string ordType,
            decimal price = 0,
            decimal stopPx = 0,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0)
        {
            var command = new NewOrderSingle
            {
                MsgType = MsgTypes.NewOrderSingle,
                ClOrdId = clOrdId,
                Symbol = symbol,
                Side = side,
                OrderQty = orderQty.ToFixString(),
                Price = price.ToFixString(),
                Account = account,
                OrdType = ordType.Proto(),
                StopPx = stopPx.ToFixString(),
                TimeInForce = timeInForce.Proto(),
                PositionID = positionId,
            };

            if (positionId != 0)
                command.PositionEffect = PositionEffect.Close;

            if (execInst?.Any() == true)
                command.ExecInst.AddRange(execInst);

            if (stopLossPrice != 0)
                command.AddStopLoss(stopLossPrice);
            if (takeProfitPrice != 0)
                command.AddTakeProfit(takeProfitPrice);
            if (trailingOffset != 0)
                command.AddTrailingStopLoss(trailingOffset, capPrice);

            command.TransactTime = Functions.NowUnixNano();
            return command;
        }

        public static NewOrderSingle NewMarketOrder(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0)
        {
            return NewOrderSingle(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                OrdType.Market,
                timeInForce: timeInForce,
                execInst: execInst,
                positionId: positionId,
                stopLossPrice: stopLossPrice,
                takeProfitPrice: takeProfitPrice);
        }

        public static NewOrderSingle NewLimitOrder(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal price,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0)
        {
            return NewOrderSingle(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                OrdType.Limit,
                price: price,
                timeInForce: timeInForce,
                execInst: execInst,
                positionId: positionId,
                stopLossPrice: stopLossPrice,
                takeProfitPrice: takeProfitPrice,
                trailingOffset: trailingOffset,
                capPrice: capPrice);
        }

        public static NewOrderSingle NewStopOrder(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal stopPx,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0)
        {
            return NewOrderSingle(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                OrdType.Stop,
                stopPx: stopPx,
                timeInForce: timeInForce,
                execInst: execInst,
                positionId: positionId,
                stopLossPrice: stopLossPrice,
                takeProfitPrice: takeProfitPrice,
                trailingOffset: trailingOffset,
                capPrice: capPrice);
        }

        public static NewOrderSingle NewMarketIfTouchOrder(
            string clOrdId,
            string symbol,
            string side,
            decimal orderQty,
            ulong account,
            decimal stopPx,
            string timeInForce = null,
            string[] execInst = null,
            ulong positionId = 0,
            decimal stopLossPrice=0,
            decimal takeProfitPrice=0,
            decimal trailingOffset=0,
            decimal capPrice=0)
        {
            return NewOrderSingle(
                clOrdId,
                symbol,
                side,
                orderQty,
                account,
                OrdType.MarketIfTouched,
                stopPx: stopPx,
                timeInForce: timeInForce,
                execInst: execInst,
                positionId: positionId,
                stopLossPrice: stopLossPrice,
                takeProfitPrice: takeProfitPrice,
                trailingOffset: trailingOffset,
                capPrice: capPrice);
        }

        public static void AddStopLoss(this NewOrderSingle command, decimal stopPrice)
        {
            command.SLTP.Add(new SLTP
            {
                OrdType = OrdType.Stop,
                StopPx = stopPrice.ToFixString()
            });
        }

        public static void AddTakeProfit(this NewOrderSingle command, decimal price)
        {
            command.SLTP.Add(new SLTP
            {
                OrdType = OrdType.Limit,
                Price = price.ToFixString()
            });
        }

        public static void AddTrailingStopLoss(this NewOrderSingle command, decimal trailingOffset, decimal capPrice = 0)
        {
            var sltp = new SLTP
            {
                OrdType = OrdType.Stop,
                PegPriceType = PegPriceType.TrailingStopPeg,
                PegOffsetType = PegOffsetType.BasisPoints,
                PegOffsetValue = trailingOffset.ToFixString(),
            };
            if (capPrice != 0)
                sltp.CapPrice = capPrice.ToFixString();

            command.SLTP.Add(sltp);
        }

        public static void ForPosition(this NewOrderSingle command, ulong positionId)
        {
            command.PositionID = positionId;
            command.PositionEffect = PositionEffect.Close;
        }

        public static OrderCancelRequest ToOrderCancelRequest(this ExecutionReport er, string clOrdId)
        {
            return new OrderCancelRequest
            {
                MsgType = MsgTypes.OrderCancelRequest,
                ClOrdId = clOrdId,
                OrigClOrdId = er.ClOrdId,
                Symbol = er.Symbol,
                Side = er.Side,
                TransactTime = Functions.NowUnixNano(),
                Account = er.Account,
            };
        }

        public static OrderCancelReplaceRequest ToOrderCancelReplaceRequest(this ExecutionReport er, string clOrdId)
        {
            var request = new OrderCancelReplaceRequest
            {
                MsgType = MsgTypes.OrderCancelReplaceRequest,
                ClOrdId = clOrdId,
                OrigClOrdId = er.OrigClOrdId,
                OrderId = er.OrderId,
                Symbol = er.Symbol,
                Side = er.Side,
                Account = er.Account,
                OrderQty = er.OrderQty,
                Price = er.Price,
                StopPx = er.StopPx,
                CapPrice = er.CapPrice,
                PegPriceType = er.PegPriceType,
                PegOffsetType = er.PegOffsetType,
                PegOffsetValue = er.PegOffsetValue,
                TransactTime = Functions.NowUnixNano(),
            };

            request.ExecInst.AddRange(er.ExecInst);
            request.SLTP.AddRange(er.SLTP);

            return request;
        }

        public static string Proto(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? string.Empty : str;
        }
    }
}