﻿using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
namespace Skill.Editor.UI.Extended
{
    public class TrackBar : Panel
    {
        public TrackBar()
        {
            Padding = new Thickness(0, 1);
            IsInScrollView = true;
        }

        protected override void UpdateLayout()
        {
            if (Parent != null && Parent is TrackBarView)
            {
                TrackBarView tw = (TrackBarView)Parent;
                Rect ra = RenderAreaShrinksByPadding;

                double deltaTime = (tw.TimeLine.MaxTime - tw.TimeLine.MinTime);

                foreach (var c in Controls)
                {
                    c.ScaleFactor = this.ScaleFactor;

                    Rect cRect = ra;
                    if (c is TimeLineEvent)
                    {
                        TimeLineEvent tle = (TimeLineEvent)c;
                        cRect.x = ra.x + ra.width * (float)((tle.FireTime - tw.TimeLine.MinTime) / deltaTime);
                        cRect.width = Mathf.Min(tle.MaxWidth, Mathf.Max(tle.MinWidth, ra.width * (float)(tle.Duration / deltaTime)));
                    }
                    else
                    {
                        cRect.x = ra.x + (c.X + c.Margin.Left) * this.ScaleFactor;
                        cRect.y = ra.y + (c.Y + c.Margin.Top) * this.ScaleFactor;
                        cRect.width = c.LayoutWidth * this.ScaleFactor;
                        cRect.height = c.LayoutHeight * this.ScaleFactor;
                    }
                    c.RenderArea = cRect;
                }
            }
        }

        public virtual double GetValidTime() { return 1; }
    }


    public abstract class TimeLineEvent : Grid
    {
        public abstract double FireTime { get; set; }
        public abstract double Duration { get; set; }

        private TrackBar _TrackBar;
        public TrackBar TrackBar { get { return _TrackBar; } }
        public virtual float MinWidth { get { return 0; } }

        public virtual float MaxWidth { get { return float.MaxValue; } }

        public TimeLineEvent(TrackBar trackBar)
        {
            if (trackBar == null) throw new System.ArgumentNullException("Invalid BaseTrackBar");
            this._TrackBar = trackBar;
            this.WantsMouseEvents = true;
        }
    }



    public class TimeLineEventDragDumb : Box
    {

        private ITimeLine _TimeLine;
        private float _DeltaX;
        private double _StartTime;

        private ITimeLine FindTimeLine()
        {
            IControl parent = this.Parent;
            while (parent != null)
            {
                if (parent is ITimeLine)
                {
                    return (ITimeLine)parent;
                }
                parent = parent.Parent;
            }
            return null;
        }

        public ITimeLine TimeLine
        {
            get
            {
                if (_TimeLine == null)
                    _TimeLine = FindTimeLine();
                return _TimeLine;
            }
        }
        /// <summary>
        /// Parent TimeLineEvent
        /// </summary>
        public TimeLineEvent OwnerEvent { get; private set; }

        /// <summary>
        /// Create a TimeLineEventDragDumb
        /// </summary>
        /// <param name="ownerEvent"></param>
        public TimeLineEventDragDumb(TimeLineEvent ownerEvent)
        {
            if (ownerEvent == null)
                throw new System.ArgumentNullException("Invalid TimeLineEvent");
            this.OwnerEvent = ownerEvent;
            this.WantsMouseEvents = true;
        }


        private bool _IsMouseDown;
        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left && args.Ctrl)
            {
                _IsMouseDown = true;
                _DeltaX = 0;
                _StartTime = OwnerEvent.FireTime;
            }
            base.OnMouseDown(args);
        }

        /// <summary>
        /// Occurs when mouse button was released.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseClickEventArgs </param>
        protected override void OnMouseUp(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                _IsMouseDown = false;

            }
            base.OnMouseUp(args);
        }

        /// <summary>
        /// OnMouseDrag
        /// </summary>
        /// <param name="args">Args</param>
        protected override void OnMouseDrag(MouseMoveEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                if (TimeLine != null)
                {
                    _DeltaX += args.Delta.x;
                    double time = _TimeLine.GetSnapedTime(_StartTime + _TimeLine.GetDeltaTime(_DeltaX));
                    if (time < _TimeLine.MinTime) time = _TimeLine.MinTime;
                    else if (time > _TimeLine.MaxTime) time = _TimeLine.MaxTime;

                    OwnerEvent.FireTime = time;
                    OnLayoutChanged();
                    EditorFrame.RepaintParentEditorWindow(this);
                    args.Handled = true;
                }
            }
            base.OnMouseDrag(args);
        }

        public override void HandleEvent(Event e)
        {
            if (e != null && e.type != EventType.Used)
            {
                if (_IsMouseDown)
                {
                    EventType type = e.type;
                    if (type == EventType.MouseDrag)
                    {
                        MouseButton mb = ConvertButton(e.button);
                        MouseMoveEventArgs args = new MouseMoveEventArgs(e.mousePosition, e.modifiers, mb, e.delta);
                        OnMouseDrag(args);
                        if (args.Handled)
                            e.Use();
                    }
                    else if (type == EventType.mouseDown)
                    {
                        if (_IsMouseDown)
                        {
                            if (!Contains(e.mousePosition))
                            {
                                _IsMouseDown = false;
                            }
                        }
                    }
                    else if (type == EventType.MouseUp || e.rawType == EventType.MouseUp)
                    {
                        MouseButton mb = ConvertButton(e.button);
                        MouseClickEventArgs args = new MouseClickEventArgs(e.mousePosition, e.modifiers, mb, e.clickCount);
                        OnMouseUp(args);
                        if (args.Handled)
                            e.Use();
                    }
                    else if (type != EventType.MouseDrag)
                        base.HandleEvent(e);
                }
                else if (e.type != EventType.MouseDrag)
                    base.HandleEvent(e);
            }
        }
    }
}