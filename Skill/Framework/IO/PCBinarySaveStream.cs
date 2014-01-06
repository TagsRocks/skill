using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Skill.Framework.IO
{
    /// <summary>
    /// Implement pc(windows) version of BinarySaveStream
    /// </summary>
    public class PCBinarySaveStream : BinarySaveStream
    {
        // variables

        private Stream _Stream;
        private BinaryWriter _BinaryWriter;

        /// <summary>
        /// Create an instance of PCBinarySaveStream.
        /// </summary>
        /// <param name="fileName"> valid full path of destination filename. </param>
        /// <param name="overwrite"> allow overwrite file? </param>
        public PCBinarySaveStream(string fileName, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Invalie fileName.");

            if (File.Exists(fileName))
            {
                if (overwrite)
                    File.Delete(fileName);
                else
                    throw new IOException("Can not overwite file.");
            }

            _Stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            _BinaryWriter = new BinaryWriter(_Stream);
        }

        /// <summary>
        /// Create an instance of PCBinarySaveStream.
        /// </summary>
        /// <param name="stream">Strea, to save</param>
        public PCBinarySaveStream(Stream stream)
        {
            _Stream = stream;
            if (_Stream == null)
                throw new ArgumentNullException("Stream is null.");
            if (!_Stream.CanWrite)
                throw new ArgumentException("Stream is not writable.");

            _BinaryWriter = new BinaryWriter(_Stream);
        }

        /// <summary>
        /// Close stream
        /// </summary>
        public void Close()
        {
            _BinaryWriter.Close();
            _Stream.Close();
        }

        /// <summary>
        /// Write given int value to stream
        /// </summary>
        /// <param name="i">value to write</param>
        public override void Write(int i)
        {
            _BinaryWriter.Write(i);
        }

        /// <summary>
        /// Write given float value to stream
        /// </summary>
        /// <param name="f">value to write</param>
        public override void Write(float f)
        {
            _BinaryWriter.Write(f);
        }

        /// <summary>
        /// Write given boolean value to stream
        /// </summary>
        /// <param name="b">value to write</param>
        public override void Write(bool b)
        {
            _BinaryWriter.Write(b);
        }

        /// <summary>
        /// Write given string value to stream
        /// </summary>
        /// <param name="s">value to write</param>
        public override void Write(string s)
        {
            if (s != null)
            {
                _BinaryWriter.Write(true);
                _BinaryWriter.Write(s);
            }
            else
            {
                _BinaryWriter.Write(false);
            }
        }
    }
}
