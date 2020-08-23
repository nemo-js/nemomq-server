using NemoMQ.Core.Engine;
using NemoMQ.Core.Model;
using NemoMQ.Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NemoMQ.Core
{
    public class Server
    {
        private readonly QueueManager _queueManager;
        private readonly TcpListener server;

        public Server(string ip, int port)
        {
            _queueManager = new QueueManager();

            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();

            Start();
        }

        private void Start()
        {
            /*Task mainThread = new Task(() =>
            {
                while (true)
                {
                    _queueManager.Tick();
                }
            });*/

            Task serverThread = new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        ClientConnected(client);
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                    server.Stop();
                }
            });

            //mainThread.Start();
            serverThread.Start();

            serverThread.Wait();
        }

        public async void ClientConnected(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            var bytes = new byte[256];
            int i;

            var client = new Client
            {
                Id = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString(),
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
            //TODO: handle client disconnect
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }

        private void ParseMessage(Client client, Message msg)
        {
            switch (msg.Header.Type)
            {
                case MessageType.Publish:
                    _queueManager.PublishMessage(client, msg.Header.Queue, msg.Payload);
                    break;
                case MessageType.Subscribe:
                    _queueManager.AddSubscriber(client, msg.Header.Queue);
                    break;
                case MessageType.AddQueue:
                    _queueManager.AddQueue(msg.Header.Queue);
                    break;
            }
        }
    }
}
