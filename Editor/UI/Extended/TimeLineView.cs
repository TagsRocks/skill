using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    public abstract class TimeLineView : Panel
    {
        private ITimeLine _TimeLine;
        private bool _IsMouseDown;
        private int _MouseButton;
        private float _ZoomSpeed;
        private double _StartVisibleTime;
        private double _EndVisibleTime;
        private double _MinTime;
        private double _MaxTime;

        protected Rect _ViewRect;

        /// <summary> Speed of zoom when user hold alt and drag with right mouse button (default : 0.1) </summary>
        public float ZoomSpeed { get { return _ZoomSpeed; } set { _ZoomSpeed = value; } }

        /// <summary> Color of time position thumb </summary>
        public Color ThumbColor { get; set; }
        /// <summary> Show selection time of timeline </summary>
        public bool ShowSelectionTime { get; set; }
        /// <summary> Show position time of timeline </summary>
        public bool ShowTimePosition { get; set; }
        /// <summary> Color of selection time </summary>
        public Color SelectionTimeColor { get; set; }


        public Vector2 ScrollPosition { get { return new Vector2((float)((_TimeLine.StartVisible - _TimeLine.MinTime) / (_TimeLine.MaxTime - _TimeLine.MinTime) * _ViewRect.width), VerticalScroll); } }


        private float _VerticalScroll;
        public float VerticalScroll
        {
            get { return _VerticalScroll; }
            set
            {
                if (_VerticalScroll != value)
                {
                    _VerticalScroll = value;
                    OnVerticalScrollChanged();
                }
            }
        }

        public event System.EventHandler VerticalScrollChanged;
        private void OnVerticalScrollChanged()
        {
            if (VerticalScrollChanged != null) VerticalScrollChanged(this, System.EventArgs.Empty);
        }

        /// <summary> TimeLine </summary>
        public ITimeLine TimeLine { get { return _TimeLine; } internal set { _TimeLine = value; } }

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
        public TimeLineView()
        {
            this.WantsMouseEvents = true;
            this._MouseButton = -1;
            this._ZoomSpeed = 2.0f;
            this.ThumbColor = Resources.Colors.ThumbColor;
            this.ShowSelectionTime = true;
            this.ShowTimePosition = true;
            this.SelectionTimeColor = new Color(1.0f, 0.1f, 0.0f, 0.3f);
            this._StartVisibleTime = 0.0f;
            this._EndVisibleTime = 0.01f;
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
                Invalidate();
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

            base.Render();

            GUI.EndScrollView(false);

            float dx = newScrollPos.x - preScrollPos.x;
            double deltaScrollTime = dx / _ViewRect.width * (_MaxTime - _MinTime);
            _TimeLine.Scroll(deltaScrollTime);

            float dy = newScrollPos.y - preScrollPos.y;
            if (dy != 0)
                ScrollY(dy);

            double deltaTime = TimeLine.EndVisible - TimeLine.StartVisible;
            double dPx = ra.width / deltaTime; // number of pixel required for each unit of time

            float width2 = ra.width - ScrollbarThickness * 2;
            float x;

            Color savedColor = GUI.color;

            #region Draw Time position
            if (ShowTimePosition)
            {
                if (TimeLine.TimePosition >= TimeLine.StartVisible && TimeLine.TimePosition < TimeLine.EndVisible)
                {
                    x = ra.x + (float)((TimeLine.TimePosition - TimeLine.StartVisible) * dPx);
                    Skill.Editor.LineDrawer.DrawLineGL(new Vector2(x, ra.yMin), new Vector2(x, ra.yMax - ScrollbarThickness), ThumbColor);
                }

                x = ra.x + ScrollbarThickness + (float)(((TimeLine.TimePosition - TimeLine.MinTime) / (TimeLine.MaxTime - TimeLine.MinTime)) * width2);
                Skill.Editor.LineDrawer.DrawLineGL(new Vector2(x, ra.yMax - ScrollbarThickness + 2), new Vector2(x, ra.yMax - 1), ThumbColor);
            }
            #endregion

            #region Draw Selection time
            if (ShowSelectionTime && TimeLine.SelectionLenght > 0)
            {
                Rect rect = ra;
                if (TimeLine.StartVisible < TimeLine.EndSelection && TimeLine.EndVisible > TimeLine.StartSelection)
                {

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
            if (_MouseButton == -1 && Parent != null && (args.Button == MouseButton.Middle || (args.Button == MouseButton.Right && args.Alt)))
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
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.F)
                {
                    if (RenderArea.Contains(e.mousePosition))
                    {
                        FrameAll();
                        e.Use();
                    }
                }
            }
            else if (_IsMouseDown && Parent != null && e != null)
            {
                if (e.type == EventType.MouseDrag)
                {

                    if (e.button == 2) // middle
                    {
                        if (e.modifiers == EventModifiers.Control && e.delta.x != 0)
                            PanX(e.delta.x);
                        else if (e.modifiers == EventModifiers.Shift && e.delta.y != 0)
                            PanY(e.delta.y);
                        else
                        {
                            if (e.delta.x != 0)
                                PanX(e.delta.x);
                            if (e.delta.y != 0)
                                PanY(e.delta.y);
                        }
                        e.Use();
                    }
                    else if (e.alt && e.button == 1) // right
                    {
                        if (e.delta.x != 0)
                            ZoomX(e.delta.x);
                        if (e.delta.y != 0)
                            ZoomY(e.delta.y);
                        e.Use();
                    }

                }
                else if ((e.type == EventType.MouseUp || e.rawType == EventType.MouseUp) && e.button == _MouseButton)
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

        public virtual void FrameAll()
        {
            double minTime, maxTime;
            GetTimeBounds(out minTime, out maxTime);
            if (maxTime - minTime < 1.0f)
            {
                minTime -= 0.1f;
                maxTime = minTime + 1.0f;
            }
            TimeLine.MaxTime = maxTime;
            TimeLine.MinTime = minTime;
            TimeLine.EndVisible = maxTime;
            TimeLine.StartVisible = minTime;
            OnLayoutChanged();
        }

        protected virtual void ZoomX(float dx)
        {
            double deltaTime = (-dx / RenderAreaShrinksByPadding.width) * (_EndVisibleTime - _StartVisibleTime) * _ZoomSpeed;
            _TimeLine.Zoom(deltaTime);
        }
        protected virtual void ZoomY(float dy) { }

        protected virtual void PanX(float dx)
        {
            double deltaTime = (-dx / RenderAreaShrinksByPadding.width) * (_EndVisibleTime - _StartVisibleTime);
            _TimeLine.Scroll(deltaTime);
        }

        protected virtual void ScrollY(float dy)
        {
            if (dy != 0)
            {
                VerticalScroll += dy;
                OnLayoutChanged();
            }
        }
        protected virtual void PanY(float dy) { }

        protected abstract void GetTimeBounds(out double minTime, out double maxTime);

    }
}
