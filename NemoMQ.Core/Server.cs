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
        private readonly MessageParser _messageParser;
        private readonly TcpListener server;

        public Server(string ip, int port)
        {
            _queueManager = new QueueManager();
            _messageParser = new MessageParser(_queueManager);

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

            var converter = new MessageConverter();
            try
            {
                while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    var messages = converter.OnNewData(bytes, i);
                    foreach (var msg in messages)
                    {
                        _messageParser.ParseMessage(client, msg);
                    }
                }
            }
            //TODO: handle client disconnect
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }
    }
}
