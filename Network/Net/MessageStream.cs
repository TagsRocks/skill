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

        #region Properties
        public bool CanRead { get; internal set; }
        public bool CanWrite { get; internal set; }
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
            this._Reader = new BinaryReader(this._Stream, Encoding.UTF8);
            this._Writer = new BinaryWriter(this._Stream, Encoding.UTF8);
        }

        #endregion

        #region Read methods
        private void CheckForRead()
        {
            if (!CanRead)
                throw new Exception("Can not read from not readable stream");
        }
        public int Read(byte[] buffer, int offset, int count) { CheckForRead(); return this._Reader.Read(buffer, offset, count); }
        public int ReadByte() { CheckForRead(); return this._Reader.ReadByte(); }
        public int PeekChar() { CheckForRead(); return _Reader.PeekChar(); }
        public bool ReadBoolean() { CheckForRead(); return _Reader.ReadBoolean(); }
        public char ReadChar() { CheckForRead(); return _Reader.ReadChar(); }
        public char[] ReadChars(int count) { CheckForRead(); return _Reader.ReadChars(count); }
        public decimal ReadDecimal() { CheckForRead(); return _Reader.ReadDecimal(); }
        public double ReadDouble() { CheckForRead(); return _Reader.ReadDouble(); }
        public short ReadInt16() { CheckForRead(); return _Reader.ReadInt16(); }
        public int ReadInt32() { CheckForRead(); return _Reader.ReadInt32(); }
        public long ReadInt64() { CheckForRead(); return _Reader.ReadInt64(); }
        public float ReadSingle() { CheckForRead(); return _Reader.ReadSingle(); }
        public sbyte ReadSByte() { CheckForRead(); return _Reader.ReadSByte(); }
        public string ReadString() { CheckForRead(); return _Reader.ReadString(); }
        public ushort ReadUInt16() { CheckForRead(); return _Reader.ReadUInt16(); }
        public uint ReadUInt32() { CheckForRead(); return _Reader.ReadUInt32(); }
        public ulong ReadUInt64() { CheckForRead(); return _Reader.ReadUInt64(); }

        public System.Xml.Linq.XDocument ReadXml()
        {
            CheckForRead();
            string xmlData = ReadString();
            StringReader reader = new StringReader(xmlData);
            System.Xml.Linq.XDocument document = System.Xml.Linq.XDocument.Load(reader);
            return document;
        }
        #endregion

        #region Write methods
        private void CheckForWrite()
        {
            if (!CanWrite)
                throw new Exception("Can not write to not writable stream");
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            CheckForWrite();
            this._Writer.Write(buffer, offset, count);
            this.ByteCounter += count * sizeof(byte);
        }
        public void WriteByte(byte value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(byte);
        }
        public void Write(bool value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(bool);
        }
        public void Write(byte[] buffer)
        {
            CheckForWrite();
            this._Writer.Write(buffer);
            this.ByteCounter += buffer.Length * sizeof(byte);
        }
        public void Write(char ch)
        {
            CheckForWrite();
            this._Writer.Write(ch);
            this.ByteCounter += sizeof(char);
        }
        public void Write(char[] chars)
        {
            CheckForWrite();
            this._Writer.Write(chars);
            this.ByteCounter += chars.Length * sizeof(char);
        }
        public void Write(decimal value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(decimal);
        }
        public void Write(double value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(double);
        }
        public void Write(short value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(short);
        }
        public void Write(int value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(int);
        }
        public void Write(long value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(long);
        }
        public void Write(sbyte value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(sbyte);
        }
        public void Write(float value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(float);
        }
        public void Write(string value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += System.Text.Encoding.UTF8.GetByteCount(value);
        }
        public void Write(ushort value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(ushort);
        }
        public void Write(uint value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(uint);
        }
        public void Write(ulong value)
        {
            CheckForWrite();
            this._Writer.Write(value);
            this.ByteCounter += sizeof(ulong);
        }

        public void Write(System.Xml.Linq.XDocument document)
        {
            CheckForWrite();
            var xmlDocument = new System.Xml.XmlDocument();
            using (var xmlReader = document.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            Write(xmlDocument.OuterXml);
        }
        #endregion

    }
}
