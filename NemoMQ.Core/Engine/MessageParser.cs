using NemoMQ.Protocol;
using NemoMQ.Core.Model;

namespace NemoMQ.Core.Engine
{
    class MessageParser
    {
        public QueueManager _queueManager { get; set; }

        public MessageParser(QueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        public void ParseMessage(Client client, Message msg)
        {
            switch (msg.Header.Type)
            {
                case MessageType.Publish:
                    // TODO: get queue name
                    // TODO: get message payload
                    _queueManager.PublishMessage(client, msg.Header.Queue, msg.Payload);
                    break;
                case MessageType.Subscribe:
                    // TODO: get message payload
                    _queueManager.AddSubscriber(client, msg.Header.Queue);
                    break;
                case MessageType.AddQueue:
                    // TODO: get message payload
                    _queueManager.AddQueue(msg.Header.Queue);
                    break;
            }
        }
    }
}
