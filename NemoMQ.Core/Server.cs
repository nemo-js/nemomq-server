using NemoMQ.Core.Engine;
using NemoMQ.Core.Model;
using NemoMQ.Protocol;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NemoMQ.Core
{
    public class Server
    {
        private readonly QueueManager _queueManager;
        private readonly string _ip;
        private readonly int _port;

        public Server(string ip, int port)
        {
            _queueManager = new QueueManager();
            _ip = ip;
            _port = port;
        }

        public async Task Start()
        {
            IPAddress localAddr = IPAddress.Parse(_ip);
            var server = new TcpListener(localAddr, _port);
            server.Start();

            try
            {
                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    ClientConnected(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public async void ClientConnected(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            var bytes = new byte[512];
            int i;

            var endPoint = ((IPEndPoint)tcpClient.Client.RemoteEndPoint);

            var client = new Client
            {
                Id = endPoint.Address.ToString() + ":" + endPoint.Port,
                TcpClient = tcpClient
            };

            var converter = new ByteSerializer();
            try
            {
                while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    var messages = converter.DeserializeMessages(bytes, i);
                    foreach (var msg in messages)
                    {
                        ParseMessage(client, msg);
                    }
                }
            }
            catch (IOException)
            {
                _queueManager.ClientDisconnected(client);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }

        private void ParseMessage(Client client, Message msg)
        {
            switch (msg.Type)
            {
                case MessageType.Publish:
                    _queueManager.PublishMessage(client, msg.Queue, msg.Payload);
                    break;
                case MessageType.Subscribe:
                    _queueManager.AddSubscriber(client, msg.Queue);
                    break;
                case MessageType.AddQueue:
                    _queueManager.AddQueue(msg.Queue);
                    break;
            }
        }
    }
}
