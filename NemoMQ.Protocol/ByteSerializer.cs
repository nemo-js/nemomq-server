using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NemoMQ.Protocol
{
    public class ByteSerializer : IMessageSerializer
    {
        private byte[] _unparsedBuffer;
        private int _unparsedBufferLength = 0;

        public static byte[] SerializeMessage(Message message)
        {         
            var queueame = Encoding.ASCII.GetBytes(message.Queue ?? "");
            var payload = Encoding.ASCII.GetBytes(message.Payload ?? "");

            ByteSerializableInt messageLength = payload.Length;

            var serialized = new byte[2 + 4 + queueame.Length + payload.Length];
            serialized[0] = (byte)message.Type;
            serialized[1] = (byte)queueame.Length;
            Buffer.BlockCopy(queueame, 0, serialized, 2, queueame.Length);
            var pos = 2 + queueame.Length;

            serialized[pos++] = messageLength.Byte0;
            serialized[pos++] = messageLength.Byte1;
            serialized[pos++] = messageLength.Byte2;
            serialized[pos++] = messageLength.Byte3;

            Buffer.BlockCopy(payload, 0, serialized, pos, payload.Length);

            return serialized;
        }

        public List<Message> DeserializeMessages(byte[] bytes, int count)
        {
            if (_unparsedBufferLength == 0)
            {
                _unparsedBuffer = bytes;
            }
            else
            {
                var newBuffer = new byte[_unparsedBufferLength + count];
                Buffer.BlockCopy(_unparsedBuffer, 0, newBuffer, 0, _unparsedBufferLength);
                Buffer.BlockCopy(bytes, 0, newBuffer, _unparsedBufferLength, count);
                _unparsedBuffer = newBuffer;
            }

            _unparsedBufferLength = _unparsedBufferLength + count;

            var messages = new List<Message>();

            var msg = ParseMessage();
            while (msg != null)
            {
                messages.Add(msg);
                msg = ParseMessage();
            }

            return messages;
        }

        private Message ParseMessage()
        {
            if (_unparsedBufferLength < 6)
            {
                return null;
            }

            var queueNameLength = (int)_unparsedBuffer[1];
            if (_unparsedBufferLength < queueNameLength + 3)
            {
                return null;
            }

            if (!Enum.IsDefined(typeof(MessageType), (int)_unparsedBuffer[0]))
            {
                return null;
            }

            Message msg = new Message
            {
                Type = (MessageType)_unparsedBuffer[0],
                Queue = Encoding.ASCII.GetString(_unparsedBuffer, 2, queueNameLength)
            };

            var pos = 2 + queueNameLength;
            if (_unparsedBufferLength < pos + 4)
            {
                return null;
            }

            var messageLength = new ByteSerializableInt
            {
                Byte0 = _unparsedBuffer[pos++],
                Byte1 = _unparsedBuffer[pos++],
                Byte2 = _unparsedBuffer[pos++],
                Byte3 = _unparsedBuffer[pos++],
            };
            var payloadLength = messageLength.Int32;

            if (_unparsedBufferLength < payloadLength + pos)
            {
                return null;
            }

            msg.Payload = Encoding.ASCII.GetString(_unparsedBuffer, pos, payloadLength);
            pos += payloadLength;

            var remaing = _unparsedBufferLength - pos;

            if (remaing == 0)
            {
                _unparsedBufferLength = 0;
            }
            else
            {
                var remainingData = new byte[remaing];
                Buffer.BlockCopy(_unparsedBuffer, pos, remainingData, 0, remaing);
                _unparsedBuffer = remainingData;
                _unparsedBufferLength = remaing;
            }

            return msg;
        }
    }

    //Borrowed this technique from TomTom's answer here: https://stackoverflow.com/questions/8827649/fastest-way-to-convert-int-to-4-bytes-in-c-sharp
    [StructLayout(LayoutKind.Explicit)]
    struct ByteSerializableInt
    {
        [FieldOffset(0)]
        public byte Byte0;
        [FieldOffset(1)]
        public byte Byte1;
        [FieldOffset(2)]
        public byte Byte2;
        [FieldOffset(3)]
        public byte Byte3;

        [FieldOffset(0)]
        public int Int32;

        public static implicit operator ByteSerializableInt(int value)
        {
            return new ByteSerializableInt
            {
                Int32 = value
            };
        }
    }
}
