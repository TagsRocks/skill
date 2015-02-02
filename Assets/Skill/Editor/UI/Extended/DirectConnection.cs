using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// ConnectionEventArgs 
    /// </summary>
    public class DirectConnectionEventArgs : System.EventArgs
    {
        /// <summary> Connection </summary>
        public DirectConnection Connection { get; private set; }
        /// <summary>
        /// Create a ConnectionEventArgs
        /// </summary>
        /// <param name="connection">Connection</param>
        public DirectConnectionEventArgs(DirectConnection connection)
        {
            this.Connection = connection;
        }
    }

    /// <summary>
    /// Handler for  Connection Events
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="args">Arguments</param>
    public delegate void DirectConnectionEventHandler(object sender, DirectConnectionEventArgs args);


    /// <summary>
    /// A connection between two IConnector
    /// </summary>
    public class DirectConnection : Control, Skill.Editor.UI.ISelectable
    {
        
        /// <summary> Color of connection </summary>
        public Color Color { get; set; }

        /// <summary> Color of connection when selected </summary>
        public Color SelectedColor { get; set; }

        /// <summary> Size of arrow </summary>
        public float ArrowSize { get; set; }

        /// <summary> Thickness of connection (default : 3) </summary>
        public int Thickness { get; set; }

        /// <summary> Start Connector </summary>
        public BaseControl Start { get; private set; }
        /// <summary> End Connector </summary>
        public BaseControl End { get; private set; }

        /// <summary> Texture to use for drawing arrow </summary>
        public Texture2D ArrowHead { get; set; }

        /// <summary> rotation offset of arrow (it depends on ArrowHead texture) </summary>
        public float RotationOffset { get; set; }

        /// <summary>
        /// Create a connection
        /// </summary>
        /// <param name="startConnector"> Start Connector</param>
        /// <param name="endConnector">End Connector</param> 
        /// <remarks>
        /// It is better to set valid and meaningful name to connections
        /// </remarks>
        public DirectConnection(BaseControl startConnector, BaseControl endConnector)
        {
            if (startConnector == null || endConnector == null)
                throw new System.ArgumentNullException("Invalid BaseControl to connect");
            this.Start = startConnector;
            this.End = endConnector;

            Color = Color.white;
            SelectedColor = Color.red;
            Thickness = 3;
            ArrowSize = 16;
            RotationOffset = 0;
            this.Name = "New Connection";
        }

        /// <summary> Render connector </summary>
        protected override void Render()
        {
            if (Start != null && End != null)
            {
                Vector2 p1 = this.Start.RenderArea.center;
                Vector2 p2 = this.End.RenderArea.center;
                if (ArrowHead == null) ArrowHead = Skill.Editor.Resources.UITextures.ArrowHead;
                RenderArea = DrawConnection(p1, p2, this.Color, this.ArrowSize * ScaleFactor, (IsSelected ? SelectedColor : Color), ArrowHead, RotationOffset);
            }
        }


        /// <summary>
        /// Helper method to draw a connection line between two points
        /// </summary>
        /// <param name="point1">start point</param>
        /// <param name="point2">end point</param>
        /// <param name="color">line color</param>
        /// <param name="thickness">line thickness</param>
        public static void DrawConnection(Vector2 point1, Vector2 point2, Color color)
        {
            Skill.Editor.LineDrawer.DrawLine(point1, point2, color);
        }

        /// <summary>
        /// Helper method to draw a connection line between two points
        /// </summary>
        /// <param name="point1">start point</param>
        /// <param name="point2">end point</param>
        /// <param name="color">line color</param>
        /// <param name="thickness">line thickness</param>
        /// <param name="arrowSize"> size of middle arrow </param>
        /// <param name="arrowColor"> color of arrow</param>
        /// <param name="arrowHeadTexture"> texture of arrow </param>
        /// <param name="rotationOffset"> rotation offset of arrowtexture </param>
        public static Rect DrawConnection(Vector2 point1, Vector2 point2, Color color, float arrowSize, Color arrowColor, Texture2D arrowHeadTexture, float rotationOffset)
        {
          Skill.Editor.LineDrawer.DrawLine(point1, point2, color);            

            Vector2 dir = point2 - point1;
            float length = dir.magnitude;
            dir.Normalize();

            float size2 = arrowSize * 0.5f;
            Vector2 center = point1 + (dir * (length * 0.5f - size2 - 2));
            Rect ra = new Rect(center.x - size2, center.y - size2, arrowSize, arrowSize);

            Rect? clipArea = Skill.Editor.LineDrawer.ClipArea;
            if (clipArea != null && clipArea.HasValue)
            {
                if (!clipArea.Value.Contains(ra.center))
                    return ra;
            }

            Matrix4x4 savedMatrix = GUI.matrix;
            Color savedColor = GUI.color;
            GUI.color = arrowColor;
            float rotationAngle = Skill.Framework.MathHelper.HorizontalAngle(dir.y, dir.x) + rotationOffset;
            GUIUtility.RotateAroundPivot(rotationAngle, ra.center);

            GUI.DrawTexture(ra, arrowHeadTexture, ScaleMode.StretchToFill);

            GUI.color = savedColor;
            GUI.matrix = savedMatrix;

            return ra;
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
            }
        }
    }

}