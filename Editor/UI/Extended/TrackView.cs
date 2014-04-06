using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// Track view area of TimeLine
    /// </summary>
    public class TrackView : Panel
    {
        private ITimeLine _TimeLine;
        private Rect _ViewRect;
        private bool _IsMouseDown;
        private int _MouseButton;
        private float _ZoomSpeed;
        private double _StartVisibleTime;
        private double _EndVisibleTime;
        private double _MinTime;
        private double _MaxTime;
        private float _ScrollY;

        /// <summary> Speed of zoom when user hold alt and drag with right mouse button (default : 0.1) </summary>
        public float ZoomSpeed { get { return _ZoomSpeed; } set { _ZoomSpeed = value; } }

        /// <summary> Color of time position thumb </summary>
        public Color ThumbColor { get; set; }
        /// <summary> Show selection time of timeline </summary>
        public bool ShowSelectionTime { get; set; }
        /// <summary> Color of selection time </summary>
        public Color SelectionTimeColor { get; set; }


        public Vector2 ScrollPosition { get { return new Vector2((float)((_TimeLine.StartVisible - _TimeLine.MinTime) / (_TimeLine.MaxTime - _TimeLine.MinTime) * _ViewRect.width), _ScrollY); } }

        /// <summary> TimeLine </summary>
        public ITimeLine TimeLine { get { return _TimeLine; } }

        /// <summary>
        /// Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.
        /// </summary>
        public GUIStyle HorizontalScrollbarStyle { get; set; }
        /// <summary>
        /// Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.
        /// </summary>
        public GUIStyle VerticalScrollbarStyle { get; set; }


        /// <summary>
        /// Gets or sets thikness of vertical scrollbar to consider when calculating scrollview area (default is 16)
        /// </summary>
        public float ScrollbarThickness { get; set; }


        /// <summary>
        /// Create a ZoomPanel
        /// </summary>
        public TrackView(ITimeLine timeLine)
        {
            this.WantsMouseEvents = true;
            this._TimeLine = timeLine;
            this._MouseButton = -1;
            this._ZoomSpeed = 0.1f;
            this.ThumbColor = new Color(1.0f, 0.0f, 0.0f, 0.8f);
            this.ShowSelectionTime = true;
            this.SelectionTimeColor = new Color(1.0f, 0.1f, 0.0f, 0.3f);
            this._StartVisibleTime = 0.0f;
            this._EndVisibleTime = 0.01f;
        }

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        protected override void UpdateLayout()
        {
            Rect ra = RenderAreaShrinksByPadding;
            double zoomFactor = TimeLine.ZoomFactor;

            _ViewRect = ra;
            _ViewRect.x = (float)(ra.x + _TimeLine.MinTime * zoomFactor);
            _ViewRect.width = (float)(ra.width * zoomFactor);

            float y = _ViewRect.y;
            foreach (var c in Controls)
            {
                Rect cRect = _ViewRect;

                cRect.y = y;
                cRect.height = c.LayoutHeight - c.Margin.Vertical;
                y += c.LayoutHeight;

                c.RenderArea = cRect;
            }

            _ViewRect.yMax = y;
            _ViewRect.height = Mathf.Max(ra.height, _ViewRect.height);

        }

        /// <summary> Begin Render control's content </summary>
        protected override void BeginRender()
        {
            if (_StartVisibleTime != _TimeLine.StartVisible || _EndVisibleTime != _TimeLine.EndVisible ||
                _MinTime != _TimeLine.MinTime || _MaxTime != _TimeLine.MaxTime)
            {
                _StartVisibleTime = _TimeLine.StartVisible;
                _EndVisibleTime = _TimeLine.EndVisible;
                _MinTime = _TimeLine.MinTime;
                _MaxTime = _TimeLine.MaxTime;
                RequestUpdateLayout();
            }
            base.BeginRender();
        }

        protected override void Render()
        {
            Rect ra = RenderAreaShrinksByPadding;
            double zoomFactor = TimeLine.ZoomFactor;

            Vector2 preScrollPos = ScrollPosition;
            Vector2 newScrollPos;
            if (HorizontalScrollbarStyle != null && VerticalScrollbarStyle != null)
                newScrollPos = GUI.BeginScrollView(RenderArea, preScrollPos, _ViewRect, true, true, HorizontalScrollbarStyle, VerticalScrollbarStyle);
            else
                newScrollPos = GUI.BeginScrollView(RenderArea, preScrollPos, _ViewRect, true, true);
            _ScrollY = newScrollPos.y;

            base.Render();

            GUI.EndScrollView(false);

            double deltaScrollTime = (newScrollPos.x - preScrollPos.x) / _ViewRect.width * zoomFactor;
            _TimeLine.Scroll(deltaScrollTime);

            double deltaTime = TimeLine.EndVisible - TimeLine.StartVisible;
            double dPx = ra.width / deltaTime; // number of pixel required for each unit of time

            float width2 = ra.width - ScrollbarThickness * 2;
            float x;

            Color savedColor = GUI.color;

            #region Draw Time position
            if (TimeLine.TimePosition >= TimeLine.StartVisible && TimeLine.TimePosition < TimeLine.EndVisible)
            {
                x = ra.x + (float)((TimeLine.TimePosition - TimeLine.StartVisible) * dPx);
                Skill.Editor.LineDrawer.DrawLine(new Vector2(x, ra.yMin), new Vector2(x, ra.yMax - ScrollbarThickness), ThumbColor, 1.0f, UnityEditor.EditorGUIUtility.whiteTexture);
            }

            x = ra.x + ScrollbarThickness + (float)(((TimeLine.TimePosition - TimeLine.MinTime) / (TimeLine.MaxTime - TimeLine.MinTime)) * width2);
            Skill.Editor.LineDrawer.DrawLine(new Vector2(x, ra.yMax - ScrollbarThickness + 2), new Vector2(x, ra.yMax - 1), ThumbColor, 1.0f, UnityEditor.EditorGUIUtility.whiteTexture);
            #endregion

            #region Draw Selection time
            if (ShowSelectionTime && TimeLine.SelectionLenght > 0)
            {
                Rect rect = ra;
                rect.x += (float)((TimeLine.StartSelection - TimeLine.StartVisible) * dPx);
                rect.width = (float)(TimeLine.SelectionLenght * dPx);
                rect.height -= ScrollbarThickness;
                if (!(rect.xMax < 0 || rect.xMin > ra.xMax))
                {
                    if (rect.x < ra.x)
                    {
                        float delta = ra.x - rect.x;
                        rect.x += delta;
                        rect.width -= delta;
                    }
                    if (rect.xMax > ra.xMax)
                    {
                        rect.xMax = ra.xMax;
                    }

                    GUI.color = SelectionTimeColor;
                    GUI.DrawTexture(rect, UnityEditor.EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                }

                rect.xMin = ra.x + ScrollbarThickness + (float)(((TimeLine.StartSelection - TimeLine.MinTime) / (TimeLine.MaxTime - TimeLine.MinTime)) * width2);
                rect.xMax = ra.x + ScrollbarThickness + (float)(((TimeLine.EndSelection - TimeLine.MinTime) / (TimeLine.MaxTime - TimeLine.MinTime)) * width2);
                rect.yMin = ra.yMax - ScrollbarThickness + 2;
                rect.yMax = ra.yMax - 1;

                GUI.color = SelectionTimeColor;
                GUI.DrawTexture(rect, UnityEditor.EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
            }
            #endregion

            GUI.color = savedColor;
        }

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (_MouseButton == -1 && Parent != null && (args.Button == MouseButton.Middle || args.Button == MouseButton.Right) && args.Alt)
            {
                Frame of = OwnerFrame;
                if (of != null)
                {
                    _IsMouseDown = OwnerFrame.RegisterPrecedenceEvent(this);
                    args.Handled = true;
                }
                _MouseButton = args.Button == MouseButton.Right ? 1 : 2;
            }
            base.OnMouseDown(args);
        }

        /// <summary>
        /// Handle event 
        /// </summary>
        /// <param name="e"> Event to handle</param>
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
                            double deltaTime = (-e.delta.x * _TimeLine.ZoomFactor) / _ViewRect.width;
                            _TimeLine.Scroll(deltaTime);
                            e.Use();
                        }
                        else if (e.button == 1) // right
                        {
                            double deltaTime = e.delta.x / _TimeLine.ZoomFactor * _ZoomSpeed;
                            _TimeLine.Zoom(deltaTime);
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
                else
                    base.HandleEvent(e);
            }
            else
                base.HandleEvent(e);
        }

    }

}