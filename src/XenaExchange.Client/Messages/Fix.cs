using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using XenaExchange.Client.Messages.Constants;

namespace XenaExchange.Client.Messages
{
    /// <summary>
    /// Contains fix-specific serialization details.
    /// </summary>
    public static class Fix
    {
        // Map contains only types to be deserialized
        public static readonly Dictionary<string, Type> DeserializeMap = new Dictionary<string, Type>()
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

        /// <summary>
        /// Gets all Xena protobuf messages using reflection and build fix tags mapping for usual fields.
        /// *Fix tag for the field is a protobuf field number.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> InitDictionary()
        {
            // Type -> Field name -> Field number
            var fixDictionary = new Dictionary<string, Dictionary<string, string>>();

            var descriptorWrappers = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(t => typeof(IMessage).IsAssignableFrom(t))
                .Select(t => t.GetProperty(nameof(Logon.Descriptor), BindingFlags.Public | BindingFlags.Static))
                .Select(f => new
                {
                    TypeName = f.DeclaringType.FullName,
                    Descriptor = (MessageDescriptor)f.GetValue(null),
                })
                .ToArray();

            foreach (var descriptorWrapper in descriptorWrappers)
            {
                var fixTags = new Dictionary<string, string>();
                foreach (var field in descriptorWrapper.Descriptor.Fields.InDeclarationOrder())
                    fixTags.Add(field.Name, field.FieldNumber.ToString());

                fixDictionary.Add(descriptorWrapper.TypeName, fixTags);
            }

            return fixDictionary;
        }
    }
}