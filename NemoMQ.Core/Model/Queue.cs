using System;
using System.Collections.Generic;
using System.Linq;

namespace NemoMQ.Core.Model
{
    class Queue
    {
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public QueueSettings Settings { get; set; }

        private List<Client> _subscribers { get; set; }
        private Queue<string> _messages { get; set; }

        public Queue(string name)
        {
            Name = name;
            DateCreated = DateTime.UtcNow;
            _subscribers = new List<Client>();
            _messages = new Queue<string>();
            Settings = new QueueSettings();
        }

        public Queue(string name, QueueSettings settings) : this(name)
        {
            Settings = settings;
        }

        internal void Publish(Client client, string message)
        {
            if (_subscribers.Any())
            {
                if (SendMessage(message))
                {
                    return;
                }
            }
            _messages.Enqueue(message);
        }

        internal void AddSubscriber(Client client)
        {
            _subscribers.Add(client);
            if (_messages.Any())
            {
                Enqueue();
            }
        }

        internal void Enqueue()
        {
            if (!_messages.Any())
            {
                return;
            }
            
            //TODO: if message failes to be delivered it should not be dequeued
            SendMessage(_messages.Dequeue());
        }

        internal bool SendMessage(string msg)
        {
            while (_subscribers.Any())
            {
                var sub = GetNextClient();

                if (!sub.TcpClient.Connected)
                {
                    ClientDisconnected(sub);
                    continue;
                }

                sub.Send(msg);

                return true;
            }

            return false; 
        }

        internal void ClientDisconnected(Client client)
        {
            _subscribers.Remove(client);
        }

        private Client GetNextClient()
        {
            return _subscribers.First();
        }
    }
}
