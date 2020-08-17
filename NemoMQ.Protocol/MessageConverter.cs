using System.Collections.Generic;
using System.Text;

namespace NemoMQ.Protocol
{
    public class MessageConverter
    {
        private static readonly string MESSAGE_END = "--END--";
        private string _unparsedData = "";

        public static byte[] SerializeMessage(Message message)
        {
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(message) + MESSAGE_END;

            return Encoding.ASCII.GetBytes(serialized);
        }

        public List<Message> OnNewData(byte[] bytes, int count)
        {
            var messages = new List<Message>();

            _unparsedData += Encoding.ASCII.GetString(bytes, 0, count);

            var indexOfEnd = _unparsedData.IndexOf(MESSAGE_END);
            while (indexOfEnd != -1)
            {
                var fullMessage = _unparsedData.Substring(0, indexOfEnd);
                Message msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(fullMessage);
                messages.Add(msg);
                _unparsedData = _unparsedData.Substring(indexOfEnd + MESSAGE_END.Length);
                indexOfEnd = _unparsedData.IndexOf(MESSAGE_END);
            }

            return messages;
        }
    }
}
