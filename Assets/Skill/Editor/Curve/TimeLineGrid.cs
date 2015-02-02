using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;
using Skill.Editor.UI.Extended;

namespace Skill.Editor.Curve
{
    public class TimeLineGrid : BaseControl
    {
        public override ControlType ControlType { get { return Skill.Framework.UI.ControlType.Control; } }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }

        /// <summary> Big Step value </summary>
        public double BigStep { get { return _BigStep; } }

        /// <summary> Small Step value </summary>
        public double SmallStep { get { return _SmallStep; } }

        private TimeBar _TimeBar;
        private List<Vector2> _ThickLinePoints = new List<Vector2>();
        private List<Vector2> _ThinLinePoints = new List<Vector2>();
        private int _ThickLineCount = 0;
        private int _ThinLineCount = 0;
        private double _BigStep;
        private double _SmallStep;

        public TimeLineGrid(TimeBar timeBar)
        {
            _TimeBar = timeBar;
        }

        protected override void Render()
        {
            Color savedColor = GUI.color;

            _TimeBar.DrawGrid(this.DrawVerticalLine);
            DrawValueGrid();
            DrawLines();

            GUI.color = savedColor;
        }
        private void DrawLines()
        {
            Color lineColor = _TimeBar.LineColor;
            lineColor.a = 0.1f;
            if (_ThickLineCount > 0)
            {
                Skill.Editor.LineDrawer.DrawLinesGL(_ThickLinePoints, lineColor, _ThickLineCount);
                _ThickLineCount = 0;
            }
            if (_ThinLineCount > 0)
            {
                Color thinColor = lineColor;
                thinColor.a *= 0.5f;
                Skill.Editor.LineDrawer.DrawLinesGL(_ThinLinePoints, thinColor, _ThinLineCount);
                _ThinLineCount = 0;
            }
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
        private void DrawVerticalLine(float x, bool thick)
        {
            Rect ra = RenderArea;
            float h = ra.height;
            x += ra.x;
            if (x >= ra.xMax - 1) return;
            float y = ra.y - 3;

            if (thick)
                AddLine(ref _ThickLinePoints, ref _ThickLineCount, new Vector2(x, y), new Vector2(x, y + h));
            else
                AddLine(ref _ThinLinePoints, ref _ThinLineCount, new Vector2(x, y), new Vector2(x, y + h));
        }

        private float _PixelRequiredForLabel = 20;
        private GUIContent _SampleText = new GUIContent() { text = "9" };

        private long _Factor; // factor to scale numbers to long number for better divide precision
        private long _LongMinValue; // scaled min value
        private long _LongMaxValue; // scaled max value
        private long _LongStep; // scaled step
        private long _LongMiniStep; // scaled mini step
        private long _LongFirstStep; // scaled first step
        private double _DPy; // number of pixel required for each unit of value
        private string _Format; // format of time label
        private GUIStyle _LabelStyle;

        private void DrawValueGrid()
        {
            float pixels = RenderArea.height; // number of available pixels to render value grid
            double deltaValue = MaxValue - MinValue;

            if (_LabelStyle != null)
                _PixelRequiredForLabel = _LabelStyle.CalcSize(_SampleText).y; // number of pixel required to draw a value label                
            else
                _PixelRequiredForLabel = 10;

            int num = Mathf.FloorToInt(pixels / (_PixelRequiredForLabel * 3));// number of labels can we draw
            _BigStep = deltaValue / num;
            bool extraDecimal = TimeBar.NormalizeStep(ref _BigStep);// normalize step to first upper round value            

            if (_BigStep >= 0.1) { _Format = extraDecimal ? "{0:F2}" : "{0:F1}"; _Factor = 10; }
            else if (_BigStep >= 0.01) { _Format = extraDecimal ? "{0:F3}" : "{0:F2}"; _Factor = 100; }
            else if (_BigStep >= 0.001) { _Format = extraDecimal ? "{0:F4}" : "{0:F3}"; _Factor = 1000; }
            else { _Format = extraDecimal ? "{0:F5}" : "{0:F4}"; _Factor = 10000; }

            bool fiveSplit = (long)(_BigStep * _Factor) % 10 == 5;
            _SmallStep = _BigStep * (fiveSplit ? 0.2 : 0.5);

            _Factor = 1000000; // scale doubles and convert to longs because of better divide precision
            _LongStep = (long)(_BigStep * _Factor);
            _LongMiniStep = (long)(_SmallStep * _Factor);
            _LongMinValue = (long)(MinValue * _Factor);
            _LongMaxValue = (long)(MaxValue * _Factor);
            _LongFirstStep = (_LongMinValue / _LongMiniStep + 1) * _LongMiniStep;

            _DPy = pixels / deltaValue; // number of pixel required for each unit of time        

            long fs = _LongFirstStep; // start by first time
            while (fs < _LongMaxValue)
            {
                float y = (float)((double)(fs - _LongMinValue) / _Factor * _DPy);
                if (fs % _LongStep == 0)
                {
                    DrawHorizontalLine(y, true);
                    string text = GetFormattedTime(fs);
                    DrawText(y, text);
                }
                else
                {
                    DrawHorizontalLine(y, false);
                }
                fs += _LongMiniStep;
            }

        }

        private void DrawHorizontalLine(float y, bool thick)
        {
            Rect ra = RenderArea;
            float w = ra.width;
            y = ra.yMax - y;
            if (y < ra.yMin + 1) return;
            float x = ra.x;
            if (thick)
                AddLine(ref _ThickLinePoints, ref _ThickLineCount, new Vector2(x, y), new Vector2(x + w, y));
            else
                AddLine(ref _ThinLinePoints, ref _ThinLineCount, new Vector2(x, y), new Vector2(x + w, y));
        }

        private void DrawText(float y, string text)
        {
            Color textColor = _TimeBar.LineColor;

            if (_LabelStyle == null)
            {
                this._LabelStyle = new GUIStyle();
                this._LabelStyle.normal.textColor = UnityEditor.EditorStyles.label.normal.textColor;
                this._LabelStyle.alignment = TextAnchor.MiddleLeft;
                this._LabelStyle.fontSize = 10;

            }

            if (_LabelStyle != null)
                textColor = _LabelStyle.normal.textColor;
            //textColor.a = 0.4f;
            GUI.color = textColor;

            Rect ra = RenderArea;
            float half = _PixelRequiredForLabel * 0.5f;
            if (y >= (ra.height - half)) return;
            if (y <= half) return;

            ra.y = ra.yMax - y - half;
            ra.x += 5;
            ra.width = 100;
            ra.height = _PixelRequiredForLabel;
            GUI.Label(ra, text, _LabelStyle);
        }

        private string GetFormattedTime(long value)
        {
            return string.Format(_Format, (double)value / _Factor);
        }
    }
}
