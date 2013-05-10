using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Skill.Net
{
    public sealed class MessageStream
    {
        #region Variables
        private MemoryStream _Stream;
        private BinaryReader _Reader;
        private BinaryWriter _Writer;
        #endregion

        #region internal methods
        internal long Seek(long offset, SeekOrigin loc)
        {
            return _Stream.Seek(offset, loc);
        }
        internal void Close()
        {
            _Reader.Close();
            _Writer.Close();
            _Stream.Close();

            _Stream.Dispose();
        }
        internal int ByteCounter { get; private set; }
        internal void ResetByteCounter() { ByteCounter = 0; }
        internal void Flush()
        {
            this._Stream.Flush();
        }
        internal byte[] GetBuffer() { return _Stream.GetBuffer(); }
        #endregion

        #region Constructors
        public MessageStream()
        {
            this._Stream = new MemoryStream();
            CreateRWStreams();
        }
        public MessageStream(int capacity)
        {
            this._Stream = new MemoryStream(capacity);
            CreateRWStreams();
        }
        public MessageStream(byte[] buffer)
        {
            this._Stream = new MemoryStream(buffer);
            CreateRWStreams();
        }

        private void CreateRWStreams()
        {
            this._Reader = new BinaryReader(this._Stream);
            this._Writer = new BinaryWriter(this._Stream);
        }

        #endregion

        #region Read methods
        public int Read(byte[] buffer, int offset, int count) { return this._Reader.Read(buffer, offset, count); }
        public int ReadByte() { return this._Reader.ReadByte(); }
        public int PeekChar() { return _Reader.PeekChar(); }
        public bool ReadBoolean() { return _Reader.ReadBoolean(); }
        public char ReadChar() { return _Reader.ReadChar(); }
        public char[] ReadChars(int count) { return _Reader.ReadChars(count); }
        public decimal ReadDecimal() { return _Reader.ReadDecimal(); }
        public double ReadDouble() { return _Reader.ReadDouble(); }
        public short ReadInt16() { return _Reader.ReadInt16(); }
        public int ReadInt32() { return _Reader.ReadInt32(); }
        public long ReadInt64() { return _Reader.ReadInt64(); }
        public float ReadSingle() { return _Reader.ReadSingle(); }
        public sbyte ReadSByte() { return _Reader.ReadSByte(); }
        public string ReadString() { return _Reader.ReadString(); }
        public ushort ReadUInt16() { return _Reader.ReadUInt16(); }
        public uint ReadUInt32() { return _Reader.ReadUInt32(); }
        public ulong ReadUInt64() { return _Reader.ReadUInt64(); }
        #endregion


        #region Write methods
        public void Write(byte[] buffer, int offset, int count)
        {
            this._Writer.Write(buffer, offset, count);
            this.ByteCounter += count * sizeof(byte);
        }

        public void WriteByte(byte value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(byte);
        }

        public void Write(bool value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(bool);
        }

        public void Write(byte[] buffer)
        {
            this._Writer.Write(buffer);
            this.ByteCounter += buffer.Length * sizeof(byte);
        }

        public void Write(char ch)
        {
            this._Writer.Write(ch);
            this.ByteCounter += sizeof(char);
        }

        public void Write(char[] chars)
        {
            this._Writer.Write(chars);
            this.ByteCounter += chars.Length * sizeof(char);
        }

        public void Write(decimal value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(decimal);
        }

        public void Write(double value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(double);
        }

        public void Write(short value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(short);
        }

        public void Write(int value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(int);
        }

        public void Write(long value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(long);
        }

        public void Write(sbyte value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(sbyte);
        }

        public void Write(float value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(float);
        }

        public void Write(string value)
        {
            this._Writer.Write(value);
            this.ByteCounter += System.Text.Encoding.Unicode.GetByteCount(value);
        }

        public void Write(ushort value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(ushort);
        }

        public void Write(uint value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(uint);
        }

        public void Write(ulong value)
        {
            this._Writer.Write(value);
            this.ByteCounter += sizeof(ulong);
        }

        #endregion

    }
}
