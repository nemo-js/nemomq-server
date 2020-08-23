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
            Assert.AreEqual(msg.Header.Type, messages[0].Header.Type);
            Assert.AreEqual(msg.Header.Queue, messages[0].Header.Queue);
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
            Assert.AreEqual(msg.Header.Type, messages[0].Header.Type);
            Assert.AreEqual(msg.Header.Queue, messages[0].Header.Queue);
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
            Assert.AreEqual(msg.Header.Type, messages[0].Header.Type);
            Assert.AreEqual(msg.Header.Queue, messages[0].Header.Queue);
            Assert.AreEqual(msg.Payload, messages[0].Payload);
        }

        private Message CreateSampleMessage()
        {
            return new Message
            {
                Header = new MessageHeader
                {
                    Type = MessageType.Publish,
                    Queue = "YOLO_QUEUE"
                },
                Payload = "test test test"
            };
        }
    }
}
