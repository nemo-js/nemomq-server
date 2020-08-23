using System;
using System.Collections.Generic;
using System.Text;

namespace NemoMQ.Protocol
{
    public interface IMessageSerializer
    {
        static byte[] SerializeMessage(Message message) => throw new NotImplementedException();
        public List<Message> DeserializeMessages(byte[] bytes, int count);
    }
}
