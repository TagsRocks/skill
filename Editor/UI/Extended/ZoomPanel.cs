using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// Simulate zoom for child controls
    /// Alt + Middle Mouse Button = Pan 
    /// Alt + Right Mouse Button = Zoom In/Out
    /// </summary>
    public class ZoomPanel : Panel
    {
        public const float MinZoomFactor = 0.1f;
        public const float MaxZoomFactor = 5.0f;

        private float _ZoomFactor;
        private Vector2 _PanPosition;
        private Matrix4x4 _Matrix;
        private Vector2 _MousePivot;
        private float _ZoomSpeed;
        private bool _IsMouseDown;
        private int _MouseButton;

        /// <summary> Zoom Factor </summary>
        public float ZoomFactor { get { return _ZoomFactor; } }

        /// <summary> Pan position </summary>
        public Vector2 PanPosition { get { return _PanPosition; } }

        /// <summary> Speed of zoom when user hold alt and drag with right mouse button (default : 0.001) </summary>
        public float ZoomSpeed { get { return _ZoomSpeed; } set { _ZoomSpeed = value; } }

        /// <summary> Transform matrix </summary>
        public Matrix4x4 Matrix { get { return _Matrix; } }

        /// <summary>
        /// Create a ZoomPanel
        /// </summary>
        public ZoomPanel()
        {
            _ZoomSpeed = 0.001f;
            _ZoomFactor = 1.0f;
            _PanPosition = Vector2.zero;
            UpdateMatrix();
            WantsMouseEvents = true;
            _MouseButton = -1;
        }

        private void UpdateMatrix()
        {
            _Matrix = Matrix4x4.TRS(new Vector3(_PanPosition.x, _PanPosition.y, 0), Quaternion.identity, new Vector3(_ZoomFactor, _ZoomFactor, 1));
            OnLayoutChanged();
        }


        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            Rect rect = RenderAreaShrinksByPadding;
            if (rect.xMax < rect.xMin) rect.xMax = rect.xMin;
            if (rect.yMax < rect.yMin) rect.yMax = rect.yMin;

            foreach (var c in Controls)
            {
                c.ScaleFactor = _ZoomFactor;

                Rect cRect = new Rect();

                cRect.x = rect.x + c.X + c.Margin.Left;
                cRect.y = rect.y + c.Y + c.Margin.Top;
                cRect.width = c.LayoutWidth;
                cRect.height = c.LayoutHeight;

                c.RenderArea = TransformRect(ref cRect, ref _Matrix);
            }
        }

        /// <summary> Begin Render control's content </summary>
        protected override void BeginRender()
        {
            base.BeginRender();
            Rect ra = RenderAreaShrinksByPadding;
            if (ra.xMax < ra.xMin) ra.xMax = ra.xMin;
            if (ra.yMax < ra.yMin) ra.yMax = ra.yMin;
            Rect viewRect = ra;
            viewRect.x = viewRect.y = 0;
            GUI.BeginScrollView(RenderArea, Vector3.zero, viewRect, false, false);
        }

        /// <summary> End Render control's content </summary>
        protected override void EndRender()
        {
            GUI.EndScrollView(false);
            base.EndRender();
        }

        private Rect TransformRect(ref Rect rect, ref Matrix4x4 matrix)
        {
            Vector3 topLeft = matrix.MultiplyPoint(new Vector3(rect.x, rect.y, 0));
            Vector3 bottomRight = matrix.MultiplyPoint(new Vector3(rect.x + rect.width, rect.y + rect.height, 0));
            return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
        }

        /// <summary>
        /// LayoutChanged
        /// </summary>
        protected override void OnLayoutChanged()
        {
            Frame frame = OwnerFrame;
            if (frame != null)
            {
                if (frame is Skill.Editor.UI.EditorFrame)
                    ((Skill.Editor.UI.EditorFrame)frame).Owner.Repaint();
            }
            base.OnLayoutChanged();
        }

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (_MouseButton == -1 && Parent != null && (args.Button == MouseButton.Right || args.Button == MouseButton.Middle) && args.Alt)
            {
                Frame of = OwnerFrame;
                if (of != null)
                {
                    _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
                    args.Handled = true;
                }
                _MousePivot = ConvertToLocal(args.MousePosition);
                _MouseButton = args.Button == MouseButton.Right ? 1 : 2;
            }
            base.OnMouseDown(args);
        }

        public override void HandleEvent(Event e)
        {
            if (_IsMouseDown && Parent != null && e != null)
            {
                if (e.type == EventType.MouseDrag)
                {
                    if (e.alt)
                    {
                        if (e.button == 2) // middle
                        {
                            _PanPosition += e.delta;
                            UpdateMatrix();
                            e.Use();
                        }
                        if (e.button == 1) // right
                        {
                            Zoom(e.delta.x * _ZoomSpeed, _MousePivot);
                            UpdateMatrix();
                            e.Use();
                        }
                    }
                }
                else if (e.type == EventType.MouseUp && e.button == _MouseButton)
                {
                    Frame of = OwnerFrame;
                    if (of != null)
                    {
                        of.UnregisterPrecedenceEvent(this);
                        _IsMouseDown = false;
                        _MouseButton = -1;
                        e.Use();
                    }
                }
            }
            else
                base.HandleEvent(e);
        }


        /// <summary>
        /// Zoom at specific position
        /// </summary>
        /// <param name="zoomDalta">zoom delta to add at currect ZoomFactor</param>
        /// <param name="pivotPoint">pivot point</param>
        public void Zoom(float zoomDalta, Vector2 pivotPoint)
        {
            float preZoomFactor = _ZoomFactor;
            _ZoomFactor = Mathf.Clamp(_ZoomFactor + zoomDalta, MinZoomFactor, MaxZoomFactor);
            zoomDalta = _ZoomFactor - preZoomFactor;

            UpdateMatrix();

            Vector3 pivot = _Matrix.MultiplyPoint(pivotPoint);
            Vector3 worldPanPosition = _Matrix.MultiplyPoint(_PanPosition);

            worldPanPosition = (TranslateMatrix(pivot) * ScaleMatrix(1.0f + zoomDalta) * TranslateMatrix(-pivot)).MultiplyPoint(worldPanPosition);
            _PanPosition = _Matrix.inverse.MultiplyPoint(worldPanPosition);

            UpdateMatrix();
        }

        private static Matrix4x4 TranslateMatrix(Vector2 translation)
        {
            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.m03 = translation.x;
            matrix.m13 = translation.y;
            matrix.m23 = 0;
            return matrix;
        }

        private static Matrix4x4 ScaleMatrix(float scale)
        {
            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.m00 = matrix.m11 = matrix.m22 = scale;
            return matrix;
        }
    }

}
