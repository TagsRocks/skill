using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Skill.Net
{
    /// <summary>
    /// handle connection of client
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="worker">Worker that handles connection</param>
    public delegate void ClientConnectionHandler(object sender, Worker worker);

    /// <summary>
    /// Server to listen for incoming connections
    /// </summary>
    public class Server
    {
        // message translator
        private IMessageTranslator _MessageTranslator;
        // An ArrayList is used to keep track of worker sockets that are designed
        // to communicate with each connected client. Make it a synchronized ArrayList
        // For thread safety
        private ArrayList _WorkerList;
        // An ArrayList is used to keep track of worker sockets that are designed
        // to communicate with each connected client. Make it a synchronized ArrayList
        // For thread safety
        private Socket _Listener;

        // The following variable will keep track of the cumulative 
        // total number of clients connected at any time. Since multiple threads
        // can access this variable, modifying this variable should be done
        // in a thread safe manner
        private int _IdGenerator;

        /// <summary> Port of server to use for listening</summary>
        public int Port { get; private set; }
        /// <summary> Size of buffer to recieve data</summary>
        public int BufferSize { get; private set; }
        /// <summary> Is server listening</summary>
        public bool IsListening { get; private set; }
        /// <summary> Number of connected client</summary>
        public int ClientCount { get { return _WorkerList.Count; } }

        /// <summary>
        /// Retrieves workers by index
        /// </summary>
        /// <param name="index">Index of worker</param>
        /// <returns>ServerWorker</returns>
        public ServerWorker this[int index] { get { return _WorkerList[index] as ServerWorker; } }

        /// <summary>
        /// Retrieves workers by name
        /// </summary>
        /// <param name="name">name of worker</param>
        /// <returns>ServerWorker</returns>
        public ServerWorker this[string name]
        {
            get
            {
                lock (_WorkerList)
                {
                    foreach (var w in _WorkerList)
                    {
                        ServerWorker sw = w as ServerWorker;
                        if (sw.Name == name)
                            return sw;
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// Occurs when a client connected
        /// </summary>
        public event ClientConnectionHandler ClientConnected;
        /// <summary>
        /// Occurs when a client connected
        /// </summary>
        /// <param name="worker">The worker that handles connection</param>
        protected virtual void OnClientConnected(ServerWorker worker)
        {
            if (ClientConnected != null)
                ClientConnected(this, worker);
        }

        /// <summary>
        /// Occurs when a client disconnected
        /// </summary>        
        public event ClientConnectionHandler ClientDisconnected;
        /// <summary>
        /// Occurs when a client disconnected
        /// </summary>
        /// <param name="worker">The worker that handles connection</param>
        protected virtual void OnClientDisconnected(ServerWorker worker)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, worker);
        }

        /// <summary>
        /// Create server
        /// </summary>
        /// <param name="messageTranslator">Message translator</param>
        /// <param name="bufferSize">Size of buffer for each client</param>
        public Server(IMessageTranslator messageTranslator, int bufferSize = 1024)
        {
            if (messageTranslator == null) throw new ArgumentNullException("Invalid MessageTranslator");
            this._MessageTranslator = messageTranslator;
            this._IdGenerator = 0;
            if (bufferSize <= MessageHeader.HeaderSize * 2) throw new ArgumentException(string.Format("Invalid bufferSize. size of buffer must be at least {0} bytes", MessageHeader.HeaderSize * 2));
            this.BufferSize = bufferSize;
            this._WorkerList = System.Collections.ArrayList.Synchronized(new ArrayList());
        }

        /// <summary>
        /// Start listening at given port
        /// </summary>
        /// <param name="port">Port to listen</param>
        /// <param name="backlog">The maximum length of the pending connections queue.</param>
        public void Start(int port, int backlog = 100)
        {
            this.Port = port;
            if (IsListening)
                throw new InvalidOperationException("Server already started.");
            //create the listening socket...
            _Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, Port);
            //bind to local IP Address...
            _Listener.Bind(ipLocal);
            //start listening...
            _Listener.Listen(backlog);
            // create the call back for any client connections...
            _Listener.BeginAccept(OnClientConnect, null);

            IsListening = true;
        }

        /// <summary>
        /// Close server and all workers
        /// </summary>
        public void Close()
        {
            if (_Listener != null)
                _Listener.Close();
            _Listener = null;
            lock (_WorkerList)
            {
                while (_WorkerList.Count > 0)
                {
                    ServerWorker serverWorker = _WorkerList[0] as ServerWorker;
                    if (serverWorker != null)
                        serverWorker.Close(); // by closing a worker it remove itself
                    else
                        _WorkerList.RemoveAt(0);
                }
            }
            IsListening = false;
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                if (_Listener == null)
                    return;
                Socket socketWorker = _Listener.EndAccept(asyn);
                ServerWorker serverWorker = new ServerWorker(_MessageTranslator, BufferSize, this);
                serverWorker.Name = string.Format("Client{0}", _IdGenerator);
                System.Threading.Interlocked.Increment(ref _IdGenerator);

                // Add the workerSocket reference to our ArrayList
                lock (_WorkerList)
                {
                    _WorkerList.Add(serverWorker);
                }

                // Since the main Socket is now free, it can go back and wait for
                // other clients who are attempting to connect
                _Listener.BeginAccept(OnClientConnect, null);

                serverWorker.Start(socketWorker);
                OnClientConnected(serverWorker);


            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "Errors", "OnClientConnection: Socket has been closed");
            }
            catch (SocketException se)
            {
                Logger.LogError(se);
            }
        }


        /// <summary>
        /// Notify worker disconnect
        /// </summary>
        /// <param name="serverWorker">The worker that disconnected</param>
        internal void NotifyWorkerDisconnected(ServerWorker serverWorker)
        {
            // Remove the reference to the worker socket of the closed client
            // so that this object will get garbage collected
            lock (_WorkerList)
            {
                _WorkerList.Remove(serverWorker);
            }
            OnClientDisconnected(serverWorker);
        }

        /// <summary>
        /// Get local ip of current system
        /// </summary>
        /// <returns>IP</returns>
        public static string GetMyIP()
        {
            string strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipaddress.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Get local ip of current system
        /// </summary>
        /// <returns>IP</returns>
        public static string GetMyIPV6()
        {
            string strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return ipaddress.ToString();
                }
            }
            return string.Empty;
        }
    }
}
