using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Skill.Net
{
    /// <summary>
    /// Client to connect to server
    /// </summary>
    public class Client : Worker
    {
        /// <summary> Port of server</summary>
        public int Port { get; private set; }
        /// <summary> IP of server </summary>
        public IPAddress IP { get; private set; }
        /// <summary> Size of buffer for incoming messages </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// Create a client
        /// </summary>
        /// <param name="messageTranslator">Message translator</param>
        /// <param name="bufferSize">Size of buffer for incoming messages</param>
        public Client(IMessageTranslator messageTranslator, int bufferSize = 1024)
            : base(messageTranslator, bufferSize)
        {
        }

        // connection closed
        protected override void OnClosed()
        {
            base.OnClosed();
            this.Port = 0;
            this.IP = null;
        }

        /// <summary>
        /// Connect to server
        /// </summary>        
        /// <param name="ip"> IP of server</param>
        /// <param name="port">Port of server</param>
        public void Connect(IPAddress ip, int port)
        {
            this.Port = port;
            this.IP = ip;
            try
            {
                // Create the socket instance
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Create the end point 
                IPEndPoint ipEnd = new IPEndPoint(this.IP, this.Port);
                // Connect to the remote host
                clientSocket.Connect(ipEnd);
                if (clientSocket.Connected)
                {
                    this.Start(clientSocket);
                }
                else
                {
                    Close();
                }
            }
            catch (SocketException se)
            {
                string str = string.Format("Connection failed, is the server running? {0}", se.Message);
                Logger.LogError(str);
                Close();
            }
        }

        /// <summary>
        /// Connect to server
        /// </summary>        
        /// <param name="ip"> IP of server</param>
        /// <param name="port">Port of server</param>
        public void Connect(string ip, int port)
        {
            // Cet the remote IP address
            IPAddress iPAddress = IPAddress.Parse(ip);
            Connect(iPAddress, port);

        }
    }
}
