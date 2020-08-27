using System;
using System.Collections.Generic;
using System.Text;

namespace NemoMQ.Protocol
{
    public class Message
    {
        public MessageType Type { get; set; }
        public string Queue { get; set; }
        public string Payload { get; set; }
    }

    public enum MessageType
    {
        Publish = 128,
        Subscribe = 129,
        AddQueue = 130,
        SendData = 131
    }
}
