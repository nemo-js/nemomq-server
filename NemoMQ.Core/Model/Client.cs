using NemoMQ.Protocol;
using System.Net.Sockets;

namespace NemoMQ.Core.Model
{
    class Client
    {
        public string Id { get; set; }
        public TcpClient TcpClient { get; internal set; }

        internal void Send(string body)
        {
            var message = new Message
            { 
                Header = new MessageHeader
                {
                    Type = MessageType.SendData  
                },
                Payload = body
            };

            var data = ByteSerializer.SerializeMessage(message);

            NetworkStream stream = TcpClient.GetStream();
            stream.Write(data, 0, data.Length);
        }
    }
}
