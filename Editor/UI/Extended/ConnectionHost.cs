using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;


namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// manage connections and connection creation with drag mouse
    /// it must be parent control of connectors in upper hierarchy levels
    /// </summary>
    public class ConnectionHost : Grid
    {
        private bool _IsDetecting;
        private IConnector _StartConnector;
        private Vector2 _EndPreviewConnector;

        /// <summary> Preview connection color (default : red) </summary>
        public Color PreviewColor { get; set; }
        /// <summary> Preview connection thickness (default : 3) </summary>
        public int PreviewThickness { get; set; }
        /// <summary> Preview connection num segments (default : 20) </summary>
        public int PreviewNumSegments { get; set; }
        /// <summary> Whether user can create connection with mouse in a interactive way (default : true)</summary>
        public bool InteractiveCreateConnection { get; set; }

        /// <summary> Occurs when a connection created by mouse drag </summary>
        public event ConnectionEventHandler CreateConnection;
        /// <summary> Occurs when a connection created by mouse drag </summary>
        /// <param name="newConnection">New connection</param>
        protected virtual void OnCreateConnection(Connection newConnection)
        {
            if (CreateConnection != null) CreateConnection(this, new ConnectionEventArgs(newConnection));
        }

        /// <summary>
        /// Create a ConnectionHost
        /// </summary>
        public ConnectionHost()
        {
            this.PreviewColor = Color.red;
            this.PreviewThickness = 3;
            this.PreviewNumSegments = 20;
            this.InteractiveCreateConnection = true;
        }

        internal bool BeginConnectionDetect(Connector startConnector)
        {
            if (InteractiveCreateConnection && !_IsDetecting)
            {
                Frame of = OwnerFrame;
                if (of != null)
                    _IsDetecting = OwnerFrame.RegisterPrecedenceEvent(this);
                if (_IsDetecting)
                {
                    _StartConnector = startConnector;
                    this._EndPreviewConnector = startConnector.ConnectionPoint;
                }
                return true;
            }
            return false;
        }

        private List<IConnector> FindAllConnectors()
        {
            List<IConnector> connectorsList = new List<IConnector>();
            FindAllConnectors(this, connectorsList);
            return connectorsList;
        }

        private void FindAllConnectors(Panel panel, List<IConnector> connectorsList)
        {
            foreach (var c in panel.Controls)
            {
                if (c.ControlType == Skill.Framework.UI.ControlType.Panel)
                {
                    FindAllConnectors((Panel)c, connectorsList);
                }
                else if (c.ControlType == Skill.Framework.UI.ControlType.Control)
                {
                    if (c is IConnector && c != _StartConnector)
                        connectorsList.Add((IConnector)c);
                }
            }
        }

        /// <summary>
        /// HandleEvent
        /// </summary>
        /// <param name="e">Event</param>
        public override void HandleEvent(Event e)
        {
            if (_IsDetecting && Parent != null && e != null)
            {
                if (e.type == EventType.MouseDrag)
                {
                    this._EndPreviewConnector = e.mousePosition;
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
                {
                    Frame of = OwnerFrame;
                    if (of != null)
                    {
                        of.UnregisterPrecedenceEvent(this);
                        _IsDetecting = false;

                        List<IConnector> connectorsList = FindAllConnectors();
                        Vector2 mousePosition = e.mousePosition;

                        foreach (var c in connectorsList)
                        {
                            if (c.RenderArea.Contains(mousePosition) && !c.IsConnectedTo(_StartConnector))
                            {
                                Connection newConnection = new Connection(_StartConnector, c);
                                OnCreateConnection(newConnection);
                                Controls.Add(newConnection);
                                break;
                            }
                        }
                    }
                    e.Use();
                }
            }
            else
                base.HandleEvent(e);
        }

        /// <summary> Render </summary>
        protected override void Render()
        {
            base.Render();
            if (_IsDetecting)
            {
                Connection.DrawConnection(this._StartConnector.ConnectionPoint, this._EndPreviewConnector, this.PreviewColor, this.PreviewThickness, this.PreviewNumSegments);
            }
        }

    }
}