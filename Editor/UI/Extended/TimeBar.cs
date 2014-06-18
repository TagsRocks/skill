using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// Show time bar 
    /// </summary>
    public class TimeBar : Box
    {
        public const double MinSnap = 0.00001;

        private float _PixelRequiredForLabel; // number of pixel required to draw a time label

        private bool _IsChanged;
        private MouseButton _MouseButton;

        private double _StartSelectionTime;
        private double _StartTime;
        private double _EndTime;
        private double _DPx; // number of pixel required for each unit of time

        private long _Factor; // factor to scale numbers to long number for better divide precision
        private long _LongStartTime; // scaled start time
        private long _LongEndTime; // scaled end time
        private long _LongStep; // scaled step
        private long _LongMiniStep; // scaled mini step
        private long _LongFirstStep; // scaled first step in timebar

        private string _Format; // format of time label
        private string _TimeFormat; // format of time label

        private Color _LineColor; // color of lines          

        private ITimeLine _TimeLine;
        private GUIStyle _LabelStyle; // time label style
        private GUIContent _SampleText; // sample text to measure label size 

        private List<Vector2> _ThickLinePoints;
        private List<Vector2> _ThinLinePoints;
        private int _ThickLineCount;
        private int _ThinLineCount;
        private bool _TimeStyle;
        private double _BigStep;
        private double _SmallStep;


        /// <summary> TimeLine </summary>
        public ITimeLine TimeLine { get { return _TimeLine; } }

        /// <summary> Color of lines </summary>
        public Color LineColor { get { return _LineColor; } }

        /// <summary> Time label style </summary>
        public GUIStyle LabelStyle { get { return _LabelStyle; } }
        /// <summary> style used for a box to show selected time</summary>
        public GUIStyle ThumbStyle { get; set; }
        /// <summary> width of box to show selected time</summary>
        public float ThumbWidth { get; set; }
        /// <summary> Show selection time of timeline </summary>
        public bool ShowSelectionTime { get; set; }
        /// <summary> Color of selection time </summary>
        public Color SelectionTimeColor { get; set; }
        /// <summary> Color of time position thumb </summary>
        public Color ThumbColor { get; set; }
        /// <summary> Background color of time position label </summary>
        public Color TimePositionBackColor { get; set; }

        /// <summary> Show position time of TimeBar </summary>
        public bool ShowTimePosition { get; set; }

        /// <summary> Snap time </summary>
        public double SnapTime { get; set; }

        /// <summary> Big Step time </summary>
        public double BigStep { get { return _BigStep; } }

        /// <summary> Small Step time </summary>
        public double SmallStep { get { return _SmallStep; } }

        /// <summary>
        /// Show numbers in time style
        /// </summary>
        public bool TimeStyle
        {
            get { return _TimeStyle; }
            set
            {
                if (_TimeStyle != value)
                {
                    _TimeStyle = value;
                    _IsChanged = true;
                }
            }
        }


        /// <summary> RenderArea changed </summary>
        protected override void OnRenderAreaChanged()
        {
            base.OnRenderAreaChanged();
            _IsChanged = true;
        }

        /// <summary>
        /// Create a TimeBar
        /// </summary>
        /// <param name="timeLine">TimeLine</param>
        public TimeBar(ITimeLine timeLine)
        {
            if (timeLine == null)
                throw new System.ArgumentNullException("Invalid timeLine");
            this.TimeStyle = true;
            this._MouseButton = MouseButton.None;
            this._TimeLine = timeLine;
            this.WantsMouseEvents = true;
            this._StartTime = 0.0f;
            this._EndTime = 0.01f;
            this._IsChanged = true;
            this.SnapTime = 0;
            this.ShowTimePosition = true;

            this._LabelStyle = new GUIStyle();
            this._LabelStyle.alignment = TextAnchor.MiddleCenter;
            this._LabelStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            this._LabelStyle.fontSize = 10;

            this._SampleText = new GUIContent() { text = "00.000" };

            this._LineColor = this._LabelStyle.normal.textColor;
            this.ThumbWidth = 6.0f;
            this.ShowSelectionTime = true;
            this.SelectionTimeColor = new Color(1.0f, 0.1f, 0.0f, 0.3f);
            this.ThumbColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            this.TimePositionBackColor = new Color(0.9f, 0.9f, 0.9f, 0.8f);

            _ThickLinePoints = new List<Vector2>();
            _ThinLinePoints = new List<Vector2>();
            _ThickLineCount = 0;
            _ThinLineCount = 0;
        }
        private void UpdateView()
        {
            if (_StartTime != _TimeLine.StartVisible || _EndTime != _TimeLine.EndVisible)
            {
                _StartTime = _TimeLine.StartVisible;
                _EndTime = _TimeLine.EndVisible;
                _IsChanged = true;
            }

            if (_IsChanged)
            {
                _IsChanged = false;

                RebuildTimeFormat();

                float pixels = RenderArea.width; // number of available pixels to render Timebar                
                double deltaTime = _EndTime - _StartTime;

                _PixelRequiredForLabel = _LabelStyle.CalcSize(_SampleText).x + 5; // number of pixel required to draw a time label                

                int num = Mathf.FloorToInt(pixels / _PixelRequiredForLabel);// number of labels can we draw
                _BigStep = deltaTime / num;
                bool extraDecimal = NormalizeStep(ref _BigStep);// normalize step to first upper round value            

                if (_BigStep >= 0.1) { _Format = extraDecimal ? "{0:F2}" : "{0:F1}"; _Factor = 10; }
                else if (_BigStep >= 0.01) { _Format = extraDecimal ? "{0:F3}" : "{0:F2}"; _Factor = 100; }
                else if (_BigStep >= 0.001) { _Format = extraDecimal ? "{0:F4}" : "{0:F3}"; _Factor = 1000; }
                else { _Format = extraDecimal ? "{0:F5}" : "{0:F4}"; _Factor = 10000; }

                bool fiveSplit = (long)(_BigStep * _Factor) % 10 == 5;
                _SmallStep = _BigStep * (fiveSplit ? 0.2 : 0.5);

                _Factor = 1000000; // scale doubles and convert to longs because of better divide precision
                _LongStep = (long)(_BigStep * _Factor);
                _LongMiniStep = (long)(_SmallStep * _Factor);
                _LongStartTime = (long)(_StartTime * _Factor);
                _LongEndTime = (long)(_EndTime * _Factor);
                _LongFirstStep = (_LongStartTime / _LongMiniStep + 1) * _LongMiniStep;

                _DPx = pixels / deltaTime; // number of pixel required for each unit of time
            }
        }

        private void RebuildTimeFormat()
        {
            if (TimeStyle)
            {
                if (TimeLine.MaxTime >= 3600)
                {
                    if (_LongStep > 60 * _Factor)
                    {
                        _TimeFormat = "{0:D1}:{1:D2}";
                        _SampleText.text = " 0.00 ";
                    }
                    else if (_LongStep > _Factor)
                    {
                        _TimeFormat = "{0:D1}:{1:D2}:{2:D2}";
                        _SampleText.text = " 0.00.00 ";
                    }
                    else
                    {
                        _TimeFormat = "{0:D1}:{1:D2}:{2:D2}:{3:D3}";
                        _SampleText.text = " 0.00.00.000 ";
                    }
                }
                if (TimeLine.MaxTime >= 60)
                {
                    if (_LongStep > _Factor)
                    {
                        _TimeFormat = "{1:D2}:{2:D2}";
                        _SampleText.text = " 00.00 ";
                    }
                    else
                    {
                        _TimeFormat = "{1:D2}:{2:D2}:{3:D3}";
                        _SampleText.text = " 00.00.000 ";
                    }
                }

                if (_LongStep > _Factor)
                {
                    _TimeFormat = "{1:D2}:{2:D2}";
                    _SampleText.text = " 00.00 ";
                }
                else
                {
                    _TimeFormat = "{1:D2}:{2:D2}:{3:D3}";
                    _SampleText.text = " 00.00.000 ";
                }
            }
            else
            {
                _TimeFormat = null;
                _SampleText.text = " 00.000 ";
            }
        }

        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="x">position of line</param>
        /// <param name="thick">thickness of line</param>
        public delegate void DrawLineHandler(float x, bool thick);
        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="x">position of text</param>
        /// <param name="thickness">text of line</param>
        public delegate void DrawTextHandler(float x, string text);

        /// <summary>
        /// Draw grid
        /// </summary>
        /// <param name="drawLine">method to use for drawing lines</param>
        /// <param name="drawText">method to use for drawing labels</param>
        public void DrawGrid(DrawLineHandler drawLine, DrawTextHandler drawText = null)
        {
            Rect ra = RenderArea;
            long fs = _LongFirstStep; // start by first time
            while (fs < _LongEndTime)
            {
                float x = (float)((double)(fs - _LongStartTime) / _Factor * _DPx);
                if (x >= ra.xMax - 1) break;
                if (fs % _LongStep == 0)
                {
                    drawLine(x, true);
                    string text = GetFormattedTime(fs);
                    if (drawText != null)
                        drawText(x, text);
                }
                else
                {
                    drawLine(x, false);
                }
                fs += _LongMiniStep;
            }
        }

        protected override void Render()
        {
            if (this.ThumbStyle == null)
            {
                this.ThumbStyle = (GUIStyle)"ColorPickerHorizThumb";


                //this.ThumbStyle = (GUIStyle)"MeBlendPosition";
                //this.ThumbStyle.overflow = new RectOffset(0, 0, 0, 0);
                //this.ThumbStyle.fixedHeight = 28;

                //this.ThumbStyle = (GUIStyle)"Grad Down Swatch";
                //this.ThumbStyle.border = new RectOffset(0, 0, 16, 0); 
            }

            // update view if any change happened
            UpdateView();

            base.Render();

            #region Draw base time bar

            DrawGrid(this.DrawLine, this.DrawText);
            DrawLines();

            #endregion

            Color savedColor = GUI.color;
            Rect ra = RenderArea;

            #region Draw Time position
            if (ShowTimePosition && TimeLine.TimePosition >= TimeLine.StartVisible && TimeLine.TimePosition < TimeLine.EndVisible)
            {
                Rect rect = ra;
                rect.x += (float)((TimeLine.TimePosition - TimeLine.StartVisible) * _DPx) - ThumbWidth * 0.5f;
                rect.width = ThumbWidth;
                GUI.color = ThumbColor;
                if (ThumbStyle != null)
                    GUI.Box(rect, "", ThumbStyle);
                else
                    GUI.Box(rect, "");

                rect.x += ThumbWidth + 3;
                rect.width = _PixelRequiredForLabel * 1.6f;
                rect.y += ra.height * 0.5f;
                rect.height = ra.height * 0.5f;

                if (TimePositionBackColor.a > 0)
                {
                    GUI.color = TimePositionBackColor;
                    GUI.DrawTexture(rect, UnityEditor.EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
                    GUI.color = ThumbColor;
                }
                string label = string.Empty;
                if (TimeStyle)
                {
                    if (TimeLine.TimePosition > 3600)
                        label = GetTimeStyleFormat((long)(TimeLine.TimePosition * _Factor), "{0:D1}:{1:D2}:{2:D2}:{3:D3}");
                    else
                        label = GetTimeStyleFormat((long)(TimeLine.TimePosition * _Factor), "{1:D2}:{2:D2}:{3:D3}");
                }
                else
                {
                    label = string.Format("{0:F6}", TimeLine.TimePosition);
                }
                GUI.Label(rect, label, _LabelStyle);
            }
            #endregion

            #region Draw Selection time
            if (ShowSelectionTime && TimeLine.SelectionLenght > 0)
            {
                if (TimeLine.StartVisible < TimeLine.EndSelection && TimeLine.EndVisible > TimeLine.StartSelection)
                {
                    Rect rect = ra;
                    rect.x += (float)((TimeLine.StartSelection - TimeLine.StartVisible) * _DPx);
                    rect.width = (float)(TimeLine.SelectionLenght * _DPx);


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
            }
            #endregion

            GUI.color = savedColor;
        }
        /// <summary>
        /// normalize step to upper round step
        /// </summary>
        /// <param name="step">Step</param>
        /// <returns> return true if we need extra decimal to show times </returns>
        public static bool NormalizeStep(ref double step)
        {
            bool extraDecimal = false;
            double num = 0.0001;
            while (true)
            {
                if (step <= num)
                {
                    step = num;
                    extraDecimal = false;
                    break;
                }
                if (step <= num * 2)
                {
                    step = num * 2;
                    extraDecimal = false;
                    break;
                }
                if (step <= num * 2.5)
                {
                    step = num * 2.5;
                    extraDecimal = true;
                    break;
                }
                if (step <= num * 5)
                {
                    step = num * 5;
                    extraDecimal = false;
                    break;
                }
                num *= 10;
            }
            return extraDecimal;
        }

        private void DrawLine(float x, bool thick)
        {
            Rect ra = RenderArea;
            float scale = thick ? 2 : 1;
            float h = ra.height * 0.15f * scale;
            x += ra.x;
            float y = ra.y + ra.height - h;

            if (thick)
                AddLine(ref _ThickLinePoints, ref _ThickLineCount, new Vector2(x, y), new Vector2(x, y + h));
            else
                AddLine(ref _ThinLinePoints, ref _ThinLineCount, new Vector2(x, y), new Vector2(x, y + h));
        }

        private void AddLine(ref List<Vector2> linePoints, ref int lineCount, Vector2 lineStart, Vector2 lineEnd)
        {
            int index = lineCount * 2;
            if (linePoints.Count > index)
                linePoints[index] = lineStart;
            else
                linePoints.Add(lineStart);

            index++;
            if (linePoints.Count > index)
                linePoints[index] = lineEnd;
            else
                linePoints.Add(lineEnd);

            lineCount++;
        }

        private void DrawLines()
        {
            if (_ThickLineCount > 0)
            {
                Skill.Editor.LineDrawer.DrawLinesGL(_ThickLinePoints, _LineColor, _ThickLineCount);
                _ThickLineCount = 0;
            }
            if (_ThinLineCount > 0)
            {
                Color thinColor = _LineColor;
                thinColor.a *= 0.5f;
                Skill.Editor.LineDrawer.DrawLinesGL(_ThinLinePoints, thinColor, _ThinLineCount);
                _ThinLineCount = 0;
            }
        }

        private void DrawText(float x, string text)
        {
            Rect ra = RenderArea;
            float half = _PixelRequiredForLabel * 0.5f;
            if (x >= (ra.width - half)) return;
            if (x <= half) return;

            ra.x += x - half;
            ra.width = _PixelRequiredForLabel;
            ra.height *= 0.2f;
            ra.y += ra.height;
            GUI.Label(ra, text, _LabelStyle);
        }

        private string GetFormattedTime(long time)
        {
            if (_TimeFormat != null)
                return GetTimeStyleFormat(time, _TimeFormat);
            else
                return string.Format(_Format, (double)time / _Factor);

        }
        private string GetTimeStyleFormat(long time, string format)
        {
            long time2 = time / _Factor;

            long hour = time2 / 3600;
            time2 -= hour * 3600;

            long minute = time2 / 60;
            time2 -= minute * 60;

            long s = time2;

            long ms = (long)((((double)time / _Factor) - (hour * 3600 + minute * 60 + s)) * 1000);
            return string.Format(format, hour, minute, s, ms);

        }


        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnMouseDown(MouseClickEventArgs args)
        {
            if (args.Button == MouseButton.Left)
            {
                if (Contains(args.MousePosition))
                {
                    if (OwnerFrame.RegisterPrecedenceEvent(this))
                        _MouseButton = MouseButton.Left;
                    TimeLine.TimePosition = GetTime(args.MousePosition.x);
                    args.Handled = true;
                }
                else
                {
                    if (_MouseButton != MouseButton.None)
                        OwnerFrame.UnregisterPrecedenceEvent(this);
                    _MouseButton = MouseButton.None;
                }
            }
            else if (args.Button == MouseButton.Right)
            {
                if (Contains(args.MousePosition))
                {
                    double time = GetTime(args.MousePosition.x);
                    if (args.Shift && TimeLine.SelectionLenght > 0)
                    {
                        AddTime(time);
                    }
                    else
                    {
                        _StartSelectionTime = time;
                        TimeLine.SelectTime(time, time);
                    }
                    if (OwnerFrame.RegisterPrecedenceEvent(this))
                        _MouseButton = MouseButton.Right;
                    args.Handled = true;
                }
                else
                {
                    if (_MouseButton != MouseButton.None)
                        OwnerFrame.UnregisterPrecedenceEvent(this);
                    _MouseButton = MouseButton.None;
                }
            }

            base.OnMouseDown(args);
        }

        /// <summary>
        /// Occurs when mouse button was released.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseClickEventArgs </param>
        protected override void OnMouseUp(MouseClickEventArgs args)
        {
            if (args.Button == _MouseButton)
            {
                if (_MouseButton != MouseButton.None)
                    OwnerFrame.UnregisterPrecedenceEvent(this);
                _MouseButton = MouseButton.None;
                args.Handled = true;
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
                TimeLine.TimePosition = GetTime(args.MousePosition.x);
                args.Handled = true;
            }
            else if (args.Button == MouseButton.Right)
            {
                AddTime(GetTime(args.MousePosition.x));
                args.Handled = true;
            }
            base.OnMouseDrag(args);
        }

        public override void HandleEvent(Event e)
        {
            if (e != null && e.type != EventType.Used)
            {
                if (_MouseButton != MouseButton.None)
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
                        if (!Contains(e.mousePosition))
                        {
                            OwnerFrame.UnregisterPrecedenceEvent(this);
                            _MouseButton = MouseButton.None;
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
                    else
                        base.HandleEvent(e);
                }
                else
                    base.HandleEvent(e);
            }
        }


        public double GetTime(float mousePosX)
        {
            Rect ra = RenderArea;
            mousePosX = Mathf.Clamp(mousePosX - ra.x, 0, ra.width - 1);
            double time = TimeLine.StartVisible + (double)mousePosX / _DPx;
            return GetSnapedTime(time);
        }

        /// <summary>
        /// Get closest snap time to specified time
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Snapped time</returns>
        public double GetSnapedTime(double time)
        {
            if (SnapTime > MinSnap)
            {
                long longSnapTime = (long)(SnapTime * _Factor);
                if (longSnapTime <= 0) longSnapTime = _LongMiniStep;

                long longTime = (long)(time * _Factor);
                long lowerBound = (longTime / longSnapTime) * longSnapTime;
                long upperBound = lowerBound + longSnapTime;

                if (upperBound - longTime < longTime - lowerBound)
                    longTime = upperBound;
                else
                    longTime = lowerBound;

                time = (double)longTime / _Factor;
                if (time < TimeLine.MinTime) time = TimeLine.MinTime;
                else if (time > TimeLine.MaxTime) time = TimeLine.MaxTime;
            }
            return time;
        }

        /// <summary>
        /// Convert delta pixel to delta time base on current zoom
        /// </summary>
        /// <param name="deltaPixel">delta pixel</param>
        /// <returns></returns>
        public double GetDeltaTime(float deltaPixel)
        {
            float sign = Mathf.Sign(deltaPixel);
            double deltaTime = (double)Mathf.Abs(deltaPixel) / _DPx;
            return deltaTime * sign;
        }

        private void AddTime(double time)
        {
            if (time < _StartSelectionTime)
            {
                TimeLine.SelectTime(time, _StartSelectionTime);
            }
            else if (time > _StartSelectionTime)
            {
                TimeLine.SelectTime(_StartSelectionTime, time);
            }

        }
    }
}