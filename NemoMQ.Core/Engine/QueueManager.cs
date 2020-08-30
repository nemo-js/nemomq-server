using NemoMQ.Core.Model;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NemoMQ.Core.Engine
{
    class QueueManager
    {
        private ConcurrentDictionary<string, Queue> _queues { get; set; }
        private readonly string _persistenceFilePath;

        public QueueManager()
        {
            _queues = new ConcurrentDictionary<string, Queue>();
            _persistenceFilePath = Path.Combine(Helper.GetDataPath(), "queues.json");
            Load();
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

        public void ClientDisconnected(Client client)
        {
            foreach (var queue in _queues)
            {
                queue.Value.ClientDisconnected(client);
            }
        }

        public void AddQueue(string queueName, QueueSettings settings)
        {
            if (_queues.ContainsKey(queueName))
            {
                return;
            }

            if (_queues.TryAdd(queueName, new Queue(queueName, settings)))
            {
                if (settings.IsDurable)
                {
                    Save();
                }
            }
        }

        public void Tick()
        {
            foreach (var queue in _queues.Values)
            {
                queue.Enqueue();
            }
        }

        private void Load()
        {
            if (!File.Exists(_persistenceFilePath))
            {
                return;
            }

            var queues = JsonConvert.DeserializeObject<List<QueueDTO>>(File.ReadAllText(_persistenceFilePath));

            foreach (var dto in queues)
            {
                var queue = new Queue(dto.Name, new QueueSettings {
                    IsDurable = true
                })
                {
                    DateCreated = dto.DateCreated
                };

                _queues.TryAdd(queue.Name, queue);
            }
        }

        private void Save()
        {
            var durableQueues = _queues.Values
                .Where(q => q.Settings.IsDurable)
                .Select(q => new QueueDTO
                {
                    Name = q.Name,
                    DateCreated = q.DateCreated
                })
                .ToList();
            
            File.WriteAllText(_persistenceFilePath, JsonConvert.SerializeObject(durableQueues));
        }
    }
}
