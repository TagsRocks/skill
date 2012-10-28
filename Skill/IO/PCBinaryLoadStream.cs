using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Skill.IO
{
    /// <summary>
    /// Implement pc(windows) version of BinaryLoadStream
    /// </summary>
    public class PCBinaryLoadStream : BinaryLoadStream
    {
        // variables
        private Stream _Stream;
        private BinaryReader _BinaryReader;

        /// <summary>
        /// Create an instance of PCBinaryLoadStream.
        /// </summary>
        /// <param name="fileName"> valid full path of filename. </param>        
        public PCBinaryLoadStream(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Invalie fileName.");
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Invalie fileName.");

            _Stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            _BinaryReader = new BinaryReader(_Stream);
        }

        /// <summary>
        /// Create an instance of PCBinaryLoadStream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        public PCBinaryLoadStream(Stream stream)
        {
            _Stream = stream;
            if (_Stream == null)
                throw new ArgumentNullException("Stream is null.");
            if (!_Stream.CanRead)
                throw new ArgumentException("Stream is not readable.");

            _BinaryReader = new BinaryReader(_Stream);
        }

        /// <summary>
        /// Close Stream
        /// </summary>
        public void Close()
        {
            _BinaryReader.Close();
            _Stream.Close();
        }

        /// <summary>
        /// Read an int32 value for stream
        /// </summary>
        /// <returns>Int32</returns>
        public override int ReadInt()
        {
            return _BinaryReader.ReadInt32();
        }

        /// <summary>
        /// Read a float value for stream
        /// </summary>
        /// <returns>float</returns>
        public override float ReadFloat()
        {
            return _BinaryReader.ReadSingle();
        }

        /// <summary>
        /// Read a boolean value for stream
        /// </summary>
        /// <returns>bool</returns>
        public override bool ReadBoolean()
        {
            return _BinaryReader.ReadBoolean();
        }

        /// <summary>
        /// Read a string value for stream
        /// </summary>
        /// <returns>string</returns>
        public override string ReadString()
        {
            return _BinaryReader.ReadString();
        }
    }
}
