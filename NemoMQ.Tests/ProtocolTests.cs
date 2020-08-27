using Microsoft.VisualStudio.TestTools.UnitTesting;
using NemoMQ.Protocol;

namespace NemoMQ.Tests
{
    [TestClass]
    public class ProtocolTests
    {
        [TestMethod]
        public void CanSerilaze()
        {
            var conv1 = new ByteSerializer();

            var msg = CreateSampleMessage();

            var data = ByteSerializer.SerializeMessage(msg);
            var messages = conv1.DeserializeMessages(data, data.Length);

            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(msg.Type, messages[0].Type);
            Assert.AreEqual(msg.Queue, messages[0].Queue);
            Assert.AreEqual(msg.Payload, messages[0].Payload);
        }

        [TestMethod]
        public void CanSerilazeMessageInParts()
        {
            var conv1 = new ByteSerializer();

            var msg = CreateSampleMessage();

            var data = ByteSerializer.SerializeMessage(msg);

            var part1 = new byte[10];
            System.Buffer.BlockCopy(data, 0, part1, 0, part1.Length);

            var part2 = new byte[data.Length - part1.Length];
            System.Buffer.BlockCopy(data, part1.Length, part2, 0, part2.Length);

            var messages1 = conv1.DeserializeMessages(part1, part1.Length);
            Assert.AreEqual(0, messages1.Count);

            var messages = conv1.DeserializeMessages(part2, part2.Length);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(msg.Type, messages[0].Type);
            Assert.AreEqual(msg.Queue, messages[0].Queue);
            Assert.AreEqual(msg.Payload, messages[0].Payload);
        }

        [TestMethod]
        public void CanSerilazeWithGarbage()
        {
            var conv1 = new ByteSerializer();

            var msg = CreateSampleMessage();

            var data = ByteSerializer.SerializeMessage(msg);
            var finalData = new byte[data.Length +  10];
            System.Buffer.BlockCopy(data, 0, finalData, 0, data.Length);

            var messages = conv1.DeserializeMessages(finalData, finalData.Length);

            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(msg.Type, messages[0].Type);
            Assert.AreEqual(msg.Queue, messages[0].Queue);
            Assert.AreEqual(msg.Payload, messages[0].Payload);
        }

        [TestMethod]
        public void CanSerilazeManyMessagesInOneBatch()
        {
            var conv1 = new ByteSerializer();

            var msg1 = CreateSampleMessage();
            msg1.Payload = "Message NO: 1";
            var msg2 = CreateSampleMessage();
            msg2.Payload = "Message NO: 2";

            var data1 = ByteSerializer.SerializeMessage(msg1);
            var data2 = ByteSerializer.SerializeMessage(msg2);

            var bigData = new byte[data1.Length + data2.Length];
            System.Buffer.BlockCopy(data1, 0, bigData, 0, data1.Length);
            System.Buffer.BlockCopy(data2, 0, bigData, data1.Length, data2.Length);

            var messages = conv1.DeserializeMessages(bigData, bigData.Length);

            Assert.AreEqual(2, messages.Count);
            Assert.AreEqual(msg1.Payload, messages[0].Payload);
            Assert.AreEqual(msg2.Payload, messages[1].Payload);
        }

        [TestMethod]
        public void CanSerializeBigMessage()
        {
            var conv1 = new ByteSerializer();

            var msg = CreateSampleMessage();
            for (var i = 0; i < 10; i++)
            {
                msg.Payload += msg.Payload;
            }

            var data = ByteSerializer.SerializeMessage(msg);
            var messages = conv1.DeserializeMessages(data, data.Length);

            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(msg.Type, messages[0].Type);
            Assert.AreEqual(msg.Queue, messages[0].Queue);
            Assert.AreEqual(msg.Payload, messages[0].Payload);
        }

        private Message CreateSampleMessage()
        {
            return new Message
            {
                Type = MessageType.Publish,
                Queue = "YOLO_QUEUE",
                Payload = "test test test"
            };
        }
    }
}
