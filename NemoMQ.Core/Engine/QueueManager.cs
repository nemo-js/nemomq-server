using NemoMQ.Core.Model;
using System.Collections.Concurrent;

namespace NemoMQ.Core.Engine
{
    class QueueManager
    {
        private ConcurrentDictionary<string, Queue> _queues { get; set; }

        public QueueManager()
        {
            _queues = new ConcurrentDictionary<string, Queue>();
        }

        public void PublishMessage(Client client, string queueName, string message)
        {
            if (!_queues.TryGetValue(queueName, out Queue queue))
            {
                return;
            }

            queue.Publish(client, message);
        }

        public void AddSubscriber(Client client, string queueName)
        {
            if (!_queues.TryGetValue(queueName, out Queue queue))
            {
                return;
            }

            queue.AddSubscriber(client);
        }

        public void AddQueue(string queueName)
        {
            if (_queues.ContainsKey(queueName))
            {
                return;
            }

            _queues.TryAdd(queueName, new Queue());
        }

        public void Tick()
        {
            foreach (var queue in _queues.Values)
            {
                queue.Enqueue();
            }
        }
    }
}
