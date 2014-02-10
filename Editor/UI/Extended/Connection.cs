using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// A connection between two IConnector
    /// </summary>
    public class Connection : Control
    {

        private static Texture2D _AALineTexture = null;
        /// <summary> Global texture that uses to draw connection lines </summary>
        public static Texture2D AALineTexture
        {
            get
            {
                if (_AALineTexture == null)
                    _AALineTexture = Skill.Editor.LineDrawer.CreateAntiAliasLineTexture(3);
                return _AALineTexture;
            }
            set
            {
                _AALineTexture = value;
            }
        }

        /// <summary> Color of connection </summary>
        public Color Color { get; set; }
        /// <summary> Thickness of connection (default : 3) </summary>
        public int Thickness { get; set; }
        /// <summary>
        /// How many segment required to draw connection line (more segments = smoother connection)
        /// </summary>
        public int NumSegments { get; set; }

        /// <summary> Start Connector </summary>
        public IConnector Start { get; private set; }
        /// <summary> End Connector </summary>
        public IConnector End { get; private set; }


        /// <summary>
        /// Create a connection
        /// </summary>
        /// <param name="startConnector"> Start Connector</param>
        /// <param name="endConnector">End Connector</param> 
        /// <remarks>
        /// It is better to set valid and meaningful name to connections
        /// </remarks>
        public Connection(IConnector startConnector, IConnector endConnector)
        {
            if (startConnector == null || endConnector == null)
                throw new System.ArgumentNullException("Invalid IConnector to connect");
            this.Start = startConnector;
            this.End = endConnector;

            this.Start.AddConnection(this);
            this.End.AddConnection(this);

            Color = Color.white;
            Thickness = 3;
            NumSegments = 20;
            this.Name = "New Connection";
        }

        /// <summary>
        /// Brea connection and disconnect from terminal connectors
        /// </summary>
        public void Break()
        {
            if (Start != null)
            {
                Start.RemoveConnection(this);
                ConnectionHost host = Start.Host;
                if (host != null) host.Controls.Remove(this);
            }
            if (End != null)
            {
                End.RemoveConnection(this);
                ConnectionHost host = End.Host;
                if (host != null) host.Controls.Remove(this);
            }

            Start = null;
            End = null;
        }

        /// <summary> Render connector </summary>
        protected override void Render()
        {
            if (Start != null && End != null)
                DrawConnection(this.Start.ConnectionPoint, this.End.ConnectionPoint, this.Color, this.Thickness, this.NumSegments);
        }

        /// <summary>
        /// Helper method to draw a connection line between two points
        /// </summary>
        /// <param name="point1">Start point</param>
        /// <param name="point2">End point</param>
        /// <param name="color">line color</param>
        /// <param name="thickness">line thickness</param>
        /// <param name="numSegments">line segments</param>
        public static void DrawConnection(Vector2 point1, Vector2 point2, Color color, float thickness, int numSegments)
        {
            float offset = 5;
            Skill.Editor.LineDrawer.DrawBezierLine(
                point1, new Vector2(point1.x + offset + Mathf.Abs(point2.x - (point1.x + offset)) * 0.5f, point1.y + offset * 0.5f),
                point2, new Vector2(point2.x - Mathf.Abs(point2.x - (point1.x + offset)) * 0.5f, point2.y + offset * 0.5f),
                color, thickness, numSegments, AALineTexture);
        }
    }
}