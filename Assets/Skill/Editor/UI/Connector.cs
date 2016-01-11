using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using Skill.Editor.UI;
using System.Collections.Generic;

namespace Skill.Editor.UI
{
    /// <summary>
    /// ConnectionEventArgs 
    /// </summary>
    public class ConnectionEventArgs : System.EventArgs
    {
        /// <summary> Connection </summary>
        public Connection Connection { get; private set; }
        /// <summary>
        /// Create a ConnectionEventArgs
        /// </summary>
        /// <param name="connection">Connection</param>
        public ConnectionEventArgs(Connection connection)
        {
            this.Connection = connection;
        }
    }

    /// <summary>
    /// Handler for  Connection Events
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="args">Arguments</param>
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs args);


    /// <summary>
    /// Types of connectors
    /// </summary>
    public enum ConnectorType
    {
        Input = 0,
        Output = 1
    }
    /// <summary>
    /// Base clas for Connectors
    /// </summary>
    public interface IConnector : IEnumerable<Connection>
    {
        /// <summary> Occurs when connector connect with a connection </summary>
        event ConnectionEventHandler Connect;
        /// <summary> Occurs when connector disconnect From a connection </summary>
        event ConnectionEventHandler Disconnect;

        /// <summary> Location of connection to draw connected line </summary>
        Vector2 ConnectionPoint { get; }

        /// <summary> Type of connector </summary>
        ConnectorType ConnectorType { get; }

        /// <summary> Can have multi connection of not </summary>
        bool SingleConnection { get; }

        /// <summary> Render area of connector </summary>
        Rect RenderArea { get; }

        /// <summary> Host of connections </summary>
        ConnectionHost Host { get; }

        /// <summary> Number of connections </summary>
        int ConnectionCount { get; }

        /// <summary> User data </summary>
        object UserData { get; set; }

        /// <summary> Access connections by index </summary>
        /// <param name="index"></param>
        /// <returns>Index of connection</returns>
        Connection this[int index] { get; }

        /// <summary>
        /// Is connected to another connector with a connection?
        /// </summary>
        /// <param name="other">Other connector to check for connection</param>
        /// <returns>True if connected, otherwise false</returns>
        bool IsConnectedTo(IConnector other);

        /// <summary> Connect to another connector with given connection </summary>
        /// <param name="connection">Connection</param>
        void AddConnection(Connection connection);

        /// <summary> Disconnect to another connector with given connection </summary>
        /// <param name="connection">Connection</param>
        void RemoveConnection(Connection connection);


    }

    /// <summary>
    /// default implementation of Connector
    /// </summary>
    public class Connector : Box, IConnector
    {
        private List<Connection> _Connections;

        /// <summary> Location of connection to draw connected line </summary>
        public virtual Vector2 ConnectionPoint { get { return RenderArea.center; } }

        /// <summary> Can have multi connection of not </summary>
        public bool SingleConnection { get; set; }

        /// <summary> Host of connections </summary>
        public ConnectionHost Host
        {
            get
            {
                IControl parent = Parent;
                while (parent != null)
                {
                    if (parent is ConnectionHost)
                        return (ConnectionHost)parent;
                    parent = parent.Parent;
                }
                return null;
            }
        }

        /// <summary> Type of connector </summary>
        public ConnectorType ConnectorType { get; private set; }


        /// <summary> Number of connections </summary>
        public int ConnectionCount { get { return _Connections.Count; } }
        /// <summary> Access connections by index </summary>
        /// <param name="index"></param>
        /// <returns>Index of connection</returns>
        public Connection this[int index] { get { return _Connections[index]; } }

        /// <summary> Occurs when connector connect with a connection </summary>
        public event ConnectionEventHandler Connect;
        /// <summary> Occurs when connector connect with a connection </summary>
        /// <param name="connection">Connection</param>
        protected virtual void OnConnect(Connection connection)
        {
            if (Connect != null) Connect(this, new ConnectionEventArgs(connection));
        }

        /// <summary> Occurs when connector disconnect From a connection </summary>
        public event ConnectionEventHandler Disconnect;
        /// <summary> Occurs when connector disconnect From a connection </summary>
        /// <param name="connection">Connection</param>
        protected virtual void OnDisconnect(Connection connection)
        {
            if (Disconnect != null) Disconnect(this, new ConnectionEventArgs(connection));
        }

        /// <summary>
        /// Create a connector
        /// </summary>
        public Connector(ConnectorType type)
        {
            this.ConnectorType = type;
            Width = Height = 10;
            SingleConnection = false;
            _Connections = new List<Connection>();
            WantsMouseEvents = true;
            this.ContextMenu = new ConnectorContextMenu(this);
            this.Name = "New Connector";
        }

        /// <summary>
        /// Is connected to another connector with a connection?
        /// </summary>
        /// <param name="other">Other connector to check for connection</param>
        /// <returns>True if connected, otherwise false</returns>
        public bool IsConnectedTo(IConnector other)
        {
            if (other == this) return true;
            foreach (var c in _Connections)
            {
                if (c.Start == this && c.End == other) return true;
                if (c.End == this && c.Start == other) return true;
            }
            return false;
        }
        /// <summary> Connect to another connector with given connection </summary>
        /// <param name="connection">Connection</param>
        public void AddConnection(Connection connection)
        {
            if (_Connections.Contains(connection))
                throw new System.InvalidOperationException("Can not connect to a connection twice");
            if (SingleConnection)
            {
                Connection[] cs = _Connections.ToArray();
                foreach (var c in cs)
                    c.Break();
                _Connections.Clear();
            }
            _Connections.Add(connection);
            OnConnect(connection);
        }
        /// <summary> Disconnect to another connector with given connection </summary>
        /// <param name="connection">Connection</param>
        public void RemoveConnection(Connection connection)
        {
            if (_Connections.Contains(connection))
            {
                _Connections.Remove(connection);
                OnDisconnect(connection);
            }
        }

        /// <summary> Returns an enumerator that iterates through the connections. </summary>
        /// <returns> Returns an enumerator that iterates through the connections. </returns>
        public IEnumerator<Connection> GetEnumerator() { return _Connections.GetEnumerator(); }
        /// <summary> Returns an enumerator that iterates through the connections. </summary>
        /// <returns> Returns an enumerator that iterates through the connections. </returns>
        IEnumerator IEnumerable.GetEnumerator() { return (_Connections as IEnumerable).GetEnumerator(); }

        /// <summary>
        /// MouseDownEvent
        /// </summary>
        /// <param name="args">Args</param>
        protected override void MouseDownEvent(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                ConnectionHost host = Host;
                if (host != null)
                {
                    if (host.BeginConnectionDetect(this))
                        args.Handled = true;
                }
            }
            base.MouseDownEvent(args);
        }



        class ConnectorContextMenu : Skill.Editor.UI.ContextMenu
        {
            public IConnector Connector { get; private set; }

            public ConnectorContextMenu(IConnector connector)
            {
                if (connector == null)
                    throw new System.ArgumentNullException("Invalid Connector");
                this.Connector = connector;
            }

            protected override void ApplyChanges()
            {
                this.Clear();
                if (Connector.ConnectionCount == 0)
                {
                    MenuItem item = new MenuItem("No Connection");
                    Add(item);
                }
                else
                {
                    foreach (Connection c in Connector)
                    {
                        MenuItem item = new MenuItem(string.Format("Break {0}", c.Name)) { UserData = c };
                        item.Click += item_Click;
                        base.Add(item);
                    }
                }
                base.ApplyChanges();
            }

            void item_Click(object sender, System.EventArgs e)
            {
                MenuItem item = sender as MenuItem;
                Connection connection = item.UserData as Connection;
                if (connection != null)
                    connection.Break();
            }

        }
    }


}