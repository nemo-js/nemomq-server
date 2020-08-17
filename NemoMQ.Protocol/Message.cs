using System;
using System.Collections.Generic;
using System.Text;

namespace NemoMQ.Protocol
{
    public class Message
    {
        public MessageHeader Header { get; set; }
        public string Payload { get; set; }
    }

    public class MessageHeader 
    {
        public MessageType Type { get; set; }
        public string Queue { get; set; }
    }

    public enum MessageType
    {
        Publish,
        Subscribe,
        AddQueue
    }
}
