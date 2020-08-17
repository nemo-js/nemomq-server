using System.Collections.Generic;
using System.Linq;

namespace NemoMQ.Core.Model
{
    class Queue
    {
        private List<Client> _subscribers { get; set; }
        private Queue<string> _messages { get; set; }

        public Queue()
        {
            _subscribers = new List<Client>();
            _messages = new Queue<string>();
        }

        internal void Publish(Client client, string message)
        {
            _messages.Enqueue(message);
        }

        internal void AddSubscriber(Client client)
        {
            _subscribers.Add(client);
        }

        internal void Enqueue()
        {
            if (!_messages.Any())
            {
                return;
            }

            if (!_subscribers.Any())
            {
                return;
            }

            var sub = _subscribers.First();
            var msg = _messages.Dequeue();

            sub.Send(msg);
        }
    }
}
