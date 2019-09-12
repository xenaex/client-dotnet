using Api;
using FluentAssertions;
using NUnit.Framework;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Serialization.Fix;

namespace XenaExchange.Client.Tests.Unit
{
    public class FixSerializerTests
    {
        private readonly IFixSerializer _serializer = new FixSerializer();

        [Test]
        public void Serialize_Empty()
        {
            var json = _serializer.Serialize(new NewOrderSingle());
            json.Should().Be("{}");
        }

        [Test]
        public void Serialize()
        {
            var message = new NewOrderSingle
            {
                MsgType = MsgTypes.NewOrderSingle, // camel case, simple string
                Account = 123456789, // all lower, number
                Price = 111111.1111M.ToFixString(), // decimal
                CapPrice = 0M.ToFixString(), // zero decimal
                ExecInst = { "exec_inst_1", "exec_inst_2"}, // array
                PositionId = 234567891, // "Id" serialization
                SLTP = { new []{new SLTP // should be all lower case
                    {
                        // check some nested fields
                        Price = 666666.6666M.ToFixString(),
                        CapPrice = 0M.ToFixString(),
                        OrdType = OrdType.Pegged,
                    },
                }}
            };

            const string expected = "{\"35\":\"D\",\"44\":\"111111.1111\",\"1\":123456789," +
                                    "\"18\":[\"exec_inst_1\",\"exec_inst_2\"],\"2618\":234567891,\"5000\":[{" +
                                    "\"40\":\"P\",\"44\":\"666666.6666\"}]}";

            var json = _serializer.Serialize(message);
            json.Should().Be(expected);
        }

        [Test]
        public void Deserialize_Empty()
        {
            var message = _serializer.Deserialize("{\"35\":\"8\"}");
            message.Should().BeEquivalentTo(new ExecutionReport{MsgType = MsgTypes.ExecutionReport});
        }

        [Test]
        public void Deserialize()
        {
            const string json = "{\"35\":\"8\",\"44\":\"111111.1111\",\"1\":123456789," +
                                "\"18\":[\"exec_inst_1\",\"exec_inst_2\"],\"2618\":234567891,\"5000\":[{" +
                                "\"40\":\"P\",\"44\":\"666666.6666\"}]}";

            var expected = new ExecutionReport
            {
                MsgType = MsgTypes.ExecutionReport, // camel case, simple string
                Account = 123456789, // all lower, number
                Price = 111111.1111M.ToFixString(), // decimal
                CapPrice = 0M.ToFixString(), // zero decimal
                ExecInst = { "exec_inst_1", "exec_inst_2"}, // array
                PositionId = 234567891, // "Id" serialization
                SLTP = { new []{new SLTP // should be all lower case
                    {
                        // check some nested fields
                        Price = 666666.6666M.ToFixString(),
                        CapPrice = 0M.ToFixString(),
                        OrdType = OrdType.Pegged,
                    },
                }}
            };

            var message = _serializer.Deserialize(json);
            message.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void PositionMaintenanceRequest_PosMainAction()
        {
            var message = new PositionMaintenanceRequest{ PosMaintAction = PosMaintAction.Replace };
            var actual = _serializer.Serialize(message);
            var expected = "{\"712\":\"2\"}";
            expected.Should().Be(actual);
        }

        [Test]
        public void PositionMaintenanceReport_PosMainAction()
        {
            var message = new PositionMaintenanceReport{ PosMaintAction = PosMaintAction.Replace };
            var actual = _serializer.Serialize(message);
            var expected = "{\"712\":\"2\"}";
            expected.Should().Be(actual);
        }
    }
}