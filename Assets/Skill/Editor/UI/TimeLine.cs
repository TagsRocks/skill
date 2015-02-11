using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Defines base behavior for a time line
    /// </summary>
    public interface ITimeLine
    {
        /// <summary> ZoomFactor </summary>
        double ZoomFactor { get; }
        /// <summary> Start time of visible area </summary>
        double StartVisible { get; set; }
        /// <summary> End time of visible area </summary>
        double EndVisible { get; set; }
        /// <summary> Maximum available time to scroll</summary>
        double MaxTime { get; set; }
        /// <summary> Minimum available time to scroll</summary>
        double MinTime { get; set; }
        /// <summary> Start time of selected time </summary>
        double StartSelection { get; }
        /// <summary> End time of selected time </summary>
        double EndSelection { get; }
        /// <summary> Lenght of selection time </summary>
        double SelectionLenght { get; }

        /// <summary> position of time </summary>
        double TimePosition { get; set; }


        /// <summary>
        /// Select section of time
        /// </summary>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        void SelectTime(double startTime, double endTime);

        /// <summary>
        /// Scroll time line
        /// </summary>
        /// <param name="deltaTime">delta time to scroll</param>
        void Scroll(double deltaTime);
        /// <summary>
        /// Zoom time line
        /// </summary>
        /// <param name="deltaTime">delta time to zoom ( negative for zoom out, positive to zoom in)</param>
        void Zoom(double deltaTime);

        /// <summary>
        /// Convert delta pixel to delta time base on current zoom
        /// </summary>
        /// <param name="deltaPixel">delta pixel</param>
        /// <returns></returns>
        double GetDeltaTime(float deltaPixel);

        /// <summary>
        /// Get time at mouse position
        /// </summary>
        /// <param name="mousePosX">MousePosition.x</param>
        /// <returns>time</returns>
        double GetTime(float mousePosX);


        /// <summary>
        /// Get closest snap time to specified time
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Snapped time</returns>
        double GetSnapedTime(double time);

    }


    public class TimeLine : Grid, ITimeLine
    {
        /// <summary>
        /// Minimum allowed time between MinTime and MaxTime
        /// </summary>
        public const double MinTimeLine = 0.01f;

        private double _StartVisibleTime;
        private double _EndVisibleTime;
        private double _MaxTime;
        private double _MinTime;
        private double _StartSelectionTime;
        private double _EndSelectionTime;
        private double _TimePosition;
        private bool _SelectionEnable;

        private TimeBar _Timebar;
        private TimeLineView _View;
        private Button _BtnTimeStyle;

        /// <summary> TimeBar </summary>
        public TimeBar TimeBar { get { return _Timebar; } }

        /// <summary> TimeLineView  </summary>
        public TimeLineView View { get { return _View; } }

        public Color Background { get; set; }

        public bool ExtendTime { get; set; }

        /// <summary> ZoomFactor </summary>
        public double ZoomFactor { get; private set; }

        /// <summary> Start time of visible area </summary>
        public double StartVisible
        {
            get { return _StartVisibleTime; }
            set
            {
                double preValue = _StartVisibleTime;
                _StartVisibleTime = value;
                if (_StartVisibleTime < _MinTime)
                {
                    if (ExtendTime)
                        _MinTime = _StartVisibleTime;
                    else
                        _StartVisibleTime = _MinTime;
                }
                if (_StartVisibleTime > _EndVisibleTime - MinTimeLine) _StartVisibleTime = _EndVisibleTime - MinTimeLine;
                if (preValue != _StartVisibleTime)
                    OnLayoutChanged();
            }
        }
        /// <summary> End time of visible area </summary>
        public double EndVisible
        {
            get { return _EndVisibleTime; }
            set
            {
                double preValue = _EndVisibleTime;
                _EndVisibleTime = value;
                if (_EndVisibleTime < _StartVisibleTime + MinTimeLine)
                    _EndVisibleTime = _StartVisibleTime + MinTimeLine;
                if (_EndVisibleTime > _MaxTime)
                {
                    if (ExtendTime)
                        _MaxTime = _EndVisibleTime;
                    else
                        _EndVisibleTime = _MaxTime;
                }
                if (preValue != _EndVisibleTime)
                    OnLayoutChanged();
            }
        }
        /// <summary> Maximum available time to scroll</summary>
        public double MaxTime
        {
            get { return _MaxTime; }
            set
            {
                double preValue = _MaxTime;
                _MaxTime = value;
                if (_MaxTime < _MinTime + MinTimeLine) _MaxTime = _MinTime + MinTimeLine;
                if (_MaxTime < _EndVisibleTime) _EndVisibleTime = _MaxTime;
                SelectTime(_StartSelectionTime, _EndSelectionTime);
                if (preValue != _MaxTime)
                    OnLayoutChanged();
            }
        }
        /// <summary> Minimum available time to scroll</summary>
        public double MinTime
        {
            get
            {
                return _MinTime;
            }
            set
            {
                double preValue = _MinTime;
                _MinTime = value;
                if (_MinTime > _MaxTime - MinTimeLine) _MinTime = _MaxTime - MinTimeLine;
                if (_MinTime < _StartVisibleTime) _StartVisibleTime = _MinTime;
                SelectTime(_StartSelectionTime, _EndSelectionTime);
                if (preValue != _MinTime)
                    OnLayoutChanged();
            }
        }

        /// <summary> Start time of selected time </summary>
        public double StartSelection { get { return _StartSelectionTime; } }
        /// <summary> End time of selected time </summary>
        public double EndSelection { get { return _EndSelectionTime; } }
        /// <summary> Lenght of selection time </summary>
        public double SelectionLenght { get { return _EndSelectionTime - _StartSelectionTime; } }
        public bool SelectionEnable
        {
            get { return _SelectionEnable; }
            set
            {
                _SelectionEnable = value;
                if (!_SelectionEnable)
                {
                    _StartSelectionTime = _EndSelectionTime = 0;
                }
            }
        }

        /// <summary> Selected time </summary>
        public double TimePosition
        {
            get { return _TimePosition; }
            set
            {
                if (value < _MinTime) value = _MinTime;
                else if (value > _MaxTime) value = _MaxTime;

                double delta = _TimePosition - value;
                if (delta > 0.00001 || delta < -0.00001)
                {
                    _TimePosition = value;
                    OnTimePositionChanged();
                }
            }
        }

        /// <summary> Occurs when PositionChanged changed </summary>
        public event System.EventHandler TimePositionChanged;
        /// <summary> Occurs when PositionChanged changed </summary>
        protected virtual void OnTimePositionChanged()
        {
            if (TimePositionChanged != null) TimePositionChanged(this, System.EventArgs.Empty);
        }


        /// <summary>
        /// Create a ZoomPanel
        /// </summary>
        /// <param name="view">instance of a TimeLineView implemented class</param>
        public TimeLine(TimeLineView view)
        {
            if (view == null) throw new System.InvalidOperationException("Invalid TimeLineView");
            _View = view;
            _View.TimeLine = this;

            if (UnityEditor.EditorGUIUtility.isProSkin)
                Background = new Color(0.15f, 0.15f, 0.15f, 1.0f);
            else
                Background = new Color(0.55f, 0.55f, 0.55f, 1.0f);

            this.ExtendTime = true;
            this._SelectionEnable = true;
            this._StartVisibleTime = 0;
            this._EndVisibleTime = 1.0;
            this._MaxTime = 1.0;

            //this.Padding = new Thickness(2);

            this.RowDefinitions.Add(24, GridUnitType.Pixel);
            this.RowDefinitions.Add(4, GridUnitType.Pixel);
            this.RowDefinitions.Add(1, GridUnitType.Star);

            _BtnTimeStyle = new Button() { Row = 0, Column = 0, HorizontalAlignment = Framework.UI.HorizontalAlignment.Right, Width = 16, Margin = new Thickness(0, 1) };
            _BtnTimeStyle.Style = new GUIStyle();
            _BtnTimeStyle.Style.padding = new RectOffset(1, 1, 2, 2);

            _BtnTimeStyle.Click += _BtnTimeStyle_Click;

            _Timebar = new TimeBar(this) { Row = 0, Column = 0, Margin = new Thickness(0, 0, 16, 0) };

            _View.Row = 2;
            _View.Column = 0;
            _View.ScrollbarThickness = 16;
            _View.Padding = new Thickness(0, 0, 16, 0);

            this.Controls.Add(_BtnTimeStyle);
            this.Controls.Add(_Timebar);
            this.Controls.Add(_View);
        }

        void _BtnTimeStyle_Click(object sender, System.EventArgs e)
        {
            _Timebar.TimeStyle = !_Timebar.TimeStyle;
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
        }

        protected override void Render()
        {
            RefreshStyles();
            UnityEditor.EditorGUI.DrawRect(RenderArea, Background);
            base.Render();
        }

        private void RefreshStyles()
        {
            _BtnTimeStyle.Content.image = _Timebar.TimeStyle ? Skill.Editor.Resources.UITextures.TimeFormat : Skill.Editor.Resources.UITextures.SequenceFormat;
        }

        /// <summary>
        /// Select section of time
        /// </summary>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        public void SelectTime(double startTime, double endTime)
        {
            if (!_SelectionEnable) return;
            _StartSelectionTime = startTime;
            if (_StartSelectionTime < _MinTime) _StartSelectionTime = _MinTime;
            else if (_StartSelectionTime > _MaxTime) _StartSelectionTime = _MaxTime;

            _EndSelectionTime = endTime;
            if (_EndSelectionTime < _MinTime) _EndSelectionTime = _MinTime;
            else if (_EndSelectionTime > _MaxTime) _EndSelectionTime = _MaxTime;

            if (_StartSelectionTime > _EndSelectionTime) _StartSelectionTime = _EndSelectionTime;
        }

        /// <summary>
        /// LayoutChanged
        /// </summary>
        protected override void OnLayoutChanged()
        {
            ZoomFactor = (this._MaxTime - this._MinTime) / (this._EndVisibleTime - this._StartVisibleTime);
            base.OnLayoutChanged();
        }

        /// <summary>
        /// Scroll time line
        /// </summary>
        /// <param name="deltaTime">delta time to scroll</param>
        public void Scroll(double deltaTime)
        {
            if (deltaTime < 0)
            {
                double preStartTime = StartVisible;
                StartVisible += deltaTime;
                EndVisible += StartVisible - preStartTime;
            }
            else
            {
                double preEndTime = EndVisible;
                EndVisible += deltaTime;
                StartVisible += EndVisible - preEndTime;
            }
            EditorFrame.RepaintParentEditorWindow(this);
        }

        /// <summary>
        /// Zoom time line
        /// </summary>
        /// <param name="deltaTime">delta time to zoom ( negative for zoom out, positive to zoom in)</param>
        public void Zoom(double deltaTime)
        {
            deltaTime *= 0.5f;
            if (deltaTime < 0)
            {
                StartVisible += deltaTime;
                EndVisible -= deltaTime;
            }
            else
            {
                EndVisible -= deltaTime;
                StartVisible += deltaTime;
            }
            EditorFrame.RepaintParentEditorWindow(this);
        }

        /// <summary>
        /// Convert delta pixel to delta time base on current zoom
        /// </summary>
        /// <param name="deltaPixel">delta pixel</param>
        /// <returns></returns>
        public double GetDeltaTime(float deltaPixel)
        {
            return TimeBar.GetDeltaTime(deltaPixel);
        }

        /// <summary>
        /// Get time at mouse position
        /// </summary>
        /// <param name="mousePosX">MousePosition.x</param>
        /// <returns>time</returns>
        public double GetTime(float mousePosX)
        {
            return TimeBar.GetTime(mousePosX);
        }

        /// <summary>
        /// Get closest snap time to specified time
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Snapped time</returns>
        public double GetSnapedTime(double time)
        {
            return TimeBar.GetSnapedTime(time);
        }

        /// <summary>
        /// Clear all controls from TrackView
        /// </summary>
        public void Clear()
        {
            View.Controls.Clear();
        }
    }
}