using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Skill.Net
{

    /// <summary>
    /// Handle incoming message
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="msg">Message</param>
    public delegate void MessageHandler(object sender, Message msg);

    /// <summary>
    /// Handle connection and messaging of socket
    /// </summary>
    public abstract class Worker
    {
        // message translator
        private IMessageTranslator _MessageTranslator;
        private IAsyncResult _Result;
        private AsyncCallback _AsyncCallback;
        private byte[] _DataBuffer;
        private MessageStream _InStream;
        private MessageStream _OutStream;

        // a reserved buffer for large messages
        // if bufferSize be lower than size of incoming message then this buffer comes to hand for adaptability
        private MessageStream _LargeInStream;

        private MessageHeader _InHeader;
        private MessageHeader _OutHeader;
        private bool _MultipartMsg;
        private int _DataRecieved;

        /// <summary> Socket - only valid when connected </summary>
        public Socket Socket { get; private set; }
        /// <summary> Optional name for worker </summary>
        public string Name { get; set; }
        /// <summary> Status of Asynchronous operation </summary>
        public IAsyncResult Result { get { return _Result; } }
        /// <summary> Is worker connected? </summary>
        public bool IsConnected { get; private set; }
        /// <summary>
        /// Exception if conection closed by
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary> Occurs when worker closed </summary>
        public event EventHandler Closed;
        /// <summary> Occurs when worker closed </summary>
        protected virtual void OnClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when a message recieved bt worker
        /// </summary>
        public event MessageHandler Message;
        /// <summary>
        /// Occurs when a message recieved bt worker
        /// </summary>
        /// <param name="msg">Recieved message</param>
        protected virtual void OnMessage(Message msg)
        {
            if (Message != null) Message(this, msg);
        }

        /// <summary>
        /// Create a worker
        /// </summary>        
        /// <param name="bufferSize">Size of buffer</param>
        public Worker(IMessageTranslator messageTranslator, int bufferSize)
        {
            if (messageTranslator == null) throw new ArgumentNullException("Invalid MessageTranslator");
            this._MessageTranslator = messageTranslator;
            if (bufferSize <= MessageHeader.HeaderSize * 2) throw new ArgumentException(string.Format("Invalid bufferSize. size of buffer must be at least {0} bytes", MessageHeader.HeaderSize * 2));
            this._DataBuffer = new byte[bufferSize];
            this._InHeader = new MessageHeader();
            this._OutHeader = new MessageHeader();
            this._AsyncCallback = new AsyncCallback(OnDataReceived);
        }

        /// <summary>
        /// Start worker
        /// </summary>
        internal void Start(Socket socket)
        {
            IsConnected = true;
            Exception = null;
            this._InStream = new MessageStream(_DataBuffer) { CanRead = true };
            this._OutStream = new MessageStream(_DataBuffer.Length) { CanWrite = true };
            this.Socket = socket;
            WaitForData();
        }

        // Start waiting for data from the client
        private void WaitForData()
        {
            try
            {
                _Result = Socket.BeginReceive(_DataBuffer, 0, _DataBuffer.Length, SocketFlags.None, _AsyncCallback, null);
            }
            catch (SocketException se)
            {
                Exception = se;
                Logger.LogError(se);
                Close();
            }
        }



        // This the call back function which will be invoked when the socket
        // detects any client writing of data on the stream
        private void OnDataReceived(IAsyncResult asyn)
        {

            try
            {
                if (Socket == null || !IsConnected)
                    return;

                // Complete the BeginReceive() asynchronous call by EndReceive() method
                // which will return the number of characters written to the stream 
                // by the client
                int dataLenght = Socket.EndReceive(asyn);

                if (dataLenght > 0)
                {
                    if (_MultipartMsg) // this should be the next part of a previous message
                    {
                        if (dataLenght > 0)
                        {
                            _LargeInStream.CanWrite = true;
                            _LargeInStream.Write(_DataBuffer, 0, dataLenght);
                            _LargeInStream.CanWrite = false;
                            this._DataRecieved += dataLenght;
                        }
                    }
                    else // this is the first part of a message
                    {
                        if (dataLenght > MessageHeader.HeaderSize)
                        {
                            this._InHeader.ReadFrom(_DataBuffer);
                            this._DataRecieved = dataLenght - MessageHeader.HeaderSize;
                            if (this._InHeader.SizeInBytes > this._DataRecieved)
                            {
                                // the message has another part
                                _MultipartMsg = true;
                                if (_LargeInStream == null)
                                {
                                    _LargeInStream = new MessageStream(this._InHeader.SizeInBytes) { CanRead = true };
                                    Logger.LogWarning(string.Format("Size of message({0}) is greater than buffersize({1})", this._InHeader.SizeInBytes, _DataBuffer.Length));
                                }
                                else
                                {
                                    _LargeInStream.Seek(0, SeekOrigin.Begin);
                                }
                                _LargeInStream.CanWrite = true;
                                _LargeInStream.Write(_DataBuffer, MessageHeader.HeaderSize, _DataRecieved);
                                _LargeInStream.CanWrite = false;
                            }
                            else
                            {

                                _InStream.CanWrite = true;
                                _InStream.Seek(0, SeekOrigin.Begin);
                                _InStream.Write(_DataBuffer, MessageHeader.HeaderSize, _DataRecieved);
                                _InStream.CanWrite = false;
                            }
                        }
                    }

                    Message msg = null;

                    if (_MultipartMsg)
                    {
                        if (this._DataRecieved >= this._InHeader.SizeInBytes)
                        {
                            // this is the last part of message
                            _MultipartMsg = false;
                            _LargeInStream.Seek(0, SeekOrigin.Begin);
                            msg = HandleMessage(_LargeInStream, this._InHeader);
                            this._InHeader.SizeInBytes = 0;
                        }
                    }
                    else
                    {
                        _InStream.Seek(0, SeekOrigin.Begin);
                        msg = HandleMessage(_InStream, this._InHeader);
                        this._InHeader.SizeInBytes = 0;
                    }

                    if (msg != null && msg.Type == (int)MessageType.Disconnect)
                    {
                        CloseConnection();
                    }
                    else
                    {
                        // Continue the waiting for data on the Socket
                        WaitForData();
                    }
                }

            }
            catch (ObjectDisposedException ex)
            {
                Exception = ex;
                System.Diagnostics.Debugger.Log(0, "Errors", "Socket has been closed");
                Close();
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                {
                    string msg = string.Format("Client {0} Disconnected", Name);
                    Logger.LogMessage(msg);

                }
                else
                {
                    Logger.LogError(se);
                }
                Exception = se;
                Close();
            }
        }

        /// <summary>
        /// handle recieved message
        /// </summary>
        /// <param name="stream">Stream that contains message data</param>
        /// <param name="header">Header of message</param>
        /// <returns>Message</returns>
        private Message HandleMessage(MessageStream stream, MessageHeader header)
        {
            if (_MessageTranslator != null)
            {
                stream.Flush();
                Message msg = _MessageTranslator.Translate(header.Type);
                if (msg != null)
                {
                    msg.SizeInBytes = header.SizeInBytes;
                    msg.ReadData(stream);
                    OnMessage(msg);
                }
                else
                    Logger.LogWarning(string.Format("Can not translate message id : ", header.Type));
                return msg;
            }
            else
                return null;
        }

        /// <summary>
        /// Close worker
        /// </summary>
        public void Close()
        {
            if (IsConnected && Exception == null)
            {
                DisconnectMessage msg = new DisconnectMessage();
                SendMessage(msg);
            }
            CloseConnection();
        }

        private void CloseConnection()
        {
            IsConnected = false;
            if (this._InStream != null) this._InStream.Close();
            if (this._LargeInStream != null) this._LargeInStream.Close();
            if (this._OutStream != null) this._OutStream.Close();
            if (this.Socket != null) this.Socket.Close();

            this._OutStream = null;
            this._InStream = null;
            this._LargeInStream = null;
            this.Socket = null;

            OnClosed();
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="msg">Message to send</param>
        public void SendMessage(Message msg)
        {
            if (IsConnected)
            {

                // offset stream to write header later
                this._OutStream.Seek(MessageHeader.HeaderSize, SeekOrigin.Begin);
                this._OutStream.ResetByteCounter();

                msg.WriteData(this._OutStream);

                // write header of message at begin of stream
                this._OutStream.Seek(0, SeekOrigin.Begin);
                this._OutHeader.Type = msg.Type;
                this._OutHeader.SizeInBytes = this._OutStream.ByteCounter;
                this._OutHeader.WiteTo(this._OutStream);

                using (NetworkStream networkStream = new NetworkStream(Socket))
                {
                    networkStream.Write(this._OutStream.GetBuffer(), 0, MessageHeader.HeaderSize + this._OutHeader.SizeInBytes);
                    networkStream.Flush();
                }
            }
            else
                Logger.LogError("Can not send message via diconnected connection");
        }

    }
}
