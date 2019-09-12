using Api;
using FluentAssertions;
using NUnit.Framework;
using XenaExchange.Client.Messages;
using XenaExchange.Client.Messages.Constants;
using XenaExchange.Client.Serialization.Rest;

namespace XenaExchange.Client.Tests.Unit
{
    public class RestSerializerTests
    {
        private readonly IRestSerializer _serializer = new RestSerializer();

        [Test]
        public void Serialize_Empty()
        {
            var command = new NewOrderSingle();
            var json = _serializer.Serialize(command);
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
                SLTP = { new []{new SLTP // array, should be all lower case
                    {
                        // check some nested fields
                        Price = 666666.6666M.ToFixString(),
                        CapPrice = 0M.ToFixString(),
                        OrdType = OrdType.Pegged,
                    },
                }}
            };

            const string expected =
                "{\"msgType\":\"D\",\"price\":\"111111.1111\",\"account\":123456789," +
                "\"execInst\":[\"exec_inst_1\",\"exec_inst_2\"],\"positionId\":234567891,\"sltp\":[{" +
                "\"ordType\":\"P\",\"price\":\"666666.6666\"}]}";

            var json = _serializer.Serialize(message);
            json.Should().Be(expected);
        }

        [Test]
        public void Deserialize_Empty()
        {
            var message = _serializer.Deserialize("{\"msgType\":\"8\"}");
            message.Should().BeEquivalentTo(new ExecutionReport{MsgType = MsgTypes.ExecutionReport});
        }

        [Test]
        public void Deserialize()
        {
            const string json = "{\"msgType\":\"8\",\"price\":\"111111.1111\",\"account\":123456789," +
                                "\"execInst\":[\"exec_inst_1\",\"exec_inst_2\"],\"positionId\":234567891,\"sltp\":[{" +
                                "\"ordType\":\"P\",\"price\":\"666666.6666\"}]}";

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
    }
}