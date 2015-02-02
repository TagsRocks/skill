using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Skill.Net
{
    public abstract class Message
    {
        /// <summary> Type of message </summary>
        public abstract int Type { get; }
        /// <summary> Size of message in bytes </summary>
        public int SizeInBytes { get; internal set; }

        public abstract void WriteData(MessageStream stream);
        public abstract void ReadData(MessageStream stream);
    }


    internal struct MessageHeader
    {
        public const int HeaderSize = 8; // 8 byte

        /// <summary> Size of message in bytes </summary>
        public int SizeInBytes { get; set; }
        /// <summary> Type of message </summary>
        public int Type { get; set; }

        public void ReadFrom(MessageStream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            SizeInBytes = BitConverter.ToInt32(bytes, 0);
            stream.Read(bytes, 0, 4);
            this.Type = BitConverter.ToInt32(bytes, 0);
        }

        public void ReadFrom(byte[] data , int offset = 0)
        {
            this.SizeInBytes = BitConverter.ToInt32(data, offset);
            this.Type = BitConverter.ToInt32(data, 4);
        }

        public void WiteTo(MessageStream stream)
        {
            byte[] bytes = BitConverter.GetBytes(SizeInBytes);
            stream.Write(bytes, 0, 4);
            bytes = BitConverter.GetBytes(Type);
            stream.Write(bytes, 0, 4);
        }

        public void WiteTo(byte[] data, int startIndex = 0)
        {
            byte[] bytes;
            bytes = BitConverter.GetBytes(SizeInBytes);
            Array.Copy(bytes, 0, data, startIndex, bytes.Length);
            bytes = BitConverter.GetBytes(Type);
            startIndex += bytes.Length;
            Array.Copy(bytes, 0, data, startIndex, bytes.Length);
        }        
    }
}
