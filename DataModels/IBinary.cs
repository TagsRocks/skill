using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels
{
    public interface IBinaryReader
    {

        int Read(byte[] buffer, int offset, int count);
        int ReadByte();
        int PeekChar();
        bool ReadBoolean();
        char ReadChar();
        char[] ReadChars(int count);
        decimal ReadDecimal();
        double ReadDouble();
        short ReadInt16();
        int ReadInt32();
        long ReadInt64();
        float ReadSingle();
        sbyte ReadSByte();
        string ReadString();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();
    }

    public interface IBinaryWriter
    {
        void Write(byte[] buffer, int offset, int count);
        void WriteByte(byte value);
        void Write(bool value);
        void Write(byte[] buffer);
        void Write(char ch);
        void Write(char[] chars);
        void Write(decimal value);
        void Write(short value);
        void Write(int value);
        void Write(long value);
        void Write(double value);
        void Write(sbyte value);
        void Write(float value);
        void Write(string value);
        void Write(ushort value);
        void Write(uint value);
        void Write(ulong value);
    }

    public interface IBinary
    {
        void WriteTo(IBinaryWriter writer);
        void ReadFrom(IBinaryReader reader);
    }
}
