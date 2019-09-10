using System;
using System.Collections.Generic;
using Api;
using Google.Protobuf;
using Newtonsoft.Json;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Serialization.Fix;
using XenaExchange.Client.Serialization.Rest;

namespace XenaExchange.Client.Serialization
{
    /// <summary>
    /// Base Xena messages serializer.
    /// </summary>
    /// <typeparam name="TMessageWrapper">Type of message wrapper which is used to determine message type.</typeparam>
    public abstract class SerializerBase<TMessageWrapper> : ISerializer
        where TMessageWrapper : IMessageWrapper
    {
        // Map contains only types to be deserialized
        private readonly Dictionary<string, Type> _deserializeMap = new Dictionary<string, Type>
        {
            { MsgTypes.Heartbeat, typeof(Heartbeat) },

            { MsgTypes.Logon, typeof(Logon) },
            { MsgTypes.MarketDataRequestReject, typeof(MarketDataRequestReject) },
            { MsgTypes.MarketDataIncrementalRefresh, typeof(MarketDataRefresh) },
            { MsgTypes.MarketDataSnapshotFullRefresh, typeof(MarketDataRefresh) },

            { MsgTypes.OrderMassStatusResponse, typeof(OrderMassStatusResponse) },
            { MsgTypes.AccountStatusReport, typeof(BalanceSnapshotRefresh) },
            { MsgTypes.AccountStatusUpdateReport, typeof(BalanceIncrementalRefresh) },
            { MsgTypes.MarginRequirementReport, typeof(MarginRequirementReport) },
            { MsgTypes.MassPositionReport, typeof(MassPositionReport) },
            { MsgTypes.PositionMaintenanceReport, typeof(PositionMaintenanceReport) },
            { MsgTypes.PositionReport, typeof(PositionReport) },
            { MsgTypes.ExecutionReport, typeof(ExecutionReport) },
            { MsgTypes.Reject, typeof(Reject) },
            { MsgTypes.OrderCancelReject, typeof(OrderCancelReject) },
            { MsgTypes.ListStatus, typeof(ListStatus) },
        };

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        protected SerializerBase(Dictionary<string, Dictionary<string, string>> jsonNames)
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CustomJsonNameContractResolver(jsonNames)
            };
        }

        public IMessage Deserialize(string messageJson)
        {
            // Determine the type of incoming message.
            var wrapper = JsonConvert.DeserializeObject<TMessageWrapper>(messageJson);
            if (string.IsNullOrWhiteSpace(wrapper.MessageType))
                throw new NoMsgTypeException($"No msgType was returned in message: {messageJson}");

            var known = _deserializeMap.TryGetValue(wrapper.MessageType, out var messageType);
            if (!known)
            {
                var m = $"Message {wrapper.MessageType} not supported by {this.GetType().Name}.";
                throw new MsgNotSupportedException(m);
            }

            // Deserialize message into a concrete type.
            var message = JsonConvert.DeserializeObject(messageJson, messageType, _jsonSerializerSettings);
            return (IMessage)message;
        }

        public T Deserialize<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message, _jsonSerializerSettings);
        }

        public string Serialize<T>(T message) where T : IMessage
        {
            return JsonConvert.SerializeObject(message, _jsonSerializerSettings);
        }
    }
}