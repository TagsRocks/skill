using UnityEngine;
using System.Collections;
using Skill.Editor.UI.Extended;
using Skill.Framework.UI;
using System.Collections.Generic;
using Skill.Editor;

namespace Skill.Editor.Curve
{
    public class TimeLineCurveView : TimeLineView
    {

        /// <summary>
        /// Minimum allowed value between MinValue and MaxValue
        /// </summary>
        public const double MinVisibleRange = 0.01;

        private double _MinVisibleValue;
        private double _MaxVisibleValue;
        private double _MaxValue;
        private double _MinValue;
        private CurveEditor.CurveKeyMultiSelector _MultiSelector;

        public double ZoomFactorY { get; private set; }

        public double MinVisibleValue
        {
            get { return _MinVisibleValue; }
            set
            {
                double preValue = _MinVisibleValue;
                _MinVisibleValue = value;
                if (_MinVisibleValue > _MaxVisibleValue - MinVisibleRange) _MinVisibleValue = _MaxVisibleValue - MinVisibleRange;
                if (_MinVisibleValue < _MinValue) _MinValue = _MinVisibleValue;
                if (preValue != _MinVisibleValue)
                    OnLayoutChanged();
            }
        }

        public double MaxVisibleValue
        {
            get { return _MaxVisibleValue; }
            set
            {
                double preValue = _MaxVisibleValue;
                _MaxVisibleValue = value;
                if (_MaxVisibleValue < _MinVisibleValue + MinVisibleRange)
                    _MaxVisibleValue = _MinVisibleValue + MinVisibleRange;
                if (_MaxVisibleValue > _MaxValue) _MaxValue = _MaxVisibleValue;
                if (preValue != _MaxVisibleValue)
                    OnLayoutChanged();
            }
        }

        public double MaxValue
        {
            get { return _MaxValue; }
            set
            {
                double preValue = _MaxValue;
                _MaxValue = value;
                if (_MaxValue < _MinValue + MinVisibleRange) _MaxValue = _MinValue + MinVisibleRange;
                if (_MaxValue < _MaxVisibleValue) _MaxVisibleValue = _MaxValue;
                if (preValue != _MaxValue)
                    OnLayoutChanged();
            }
        }

        public double MinValue
        {
            get { return _MinValue; }
            set
            {
                double preValue = _MinValue;
                _MinValue = value;
                if (_MinValue > _MaxValue - MinVisibleRange) _MinValue = _MaxValue - MinVisibleRange;
                if (_MinValue < _MinVisibleValue) _MinVisibleValue = _MinValue;
                if (preValue != _MinValue)
                    OnLayoutChanged();

            }
        }

        public CurveEditor Editor { get; private set; }

        internal TimeLineCurveView(CurveEditor editor)
        {
            this.Editor = editor;
            ShowTimePosition = false;
            ShowSelectionTime = false;

            ZoomFactorY = 1.0;
            _MinValue = -0.1f;
            _MaxValue = 1.1f;

            MinVisibleValue = MinValue;
            MaxVisibleValue = MaxValue;

            _MultiSelector = new CurveEditor.CurveKeyMultiSelector(this);
            Controls.Add(_MultiSelector);


        }

        protected override void GetTimeBounds(out double minTime, out double maxTime)
        {
            minTime = 0.0f;
            maxTime = 1.0f;
            if (Controls.Count > 1) // because of _MultiSelector
            {
                bool found = false;
                foreach (var c in Controls)
                {
                    if (c is CurveTrack && c.Visibility == Skill.Framework.UI.Visibility.Visible)
                    {
                        CurveTrack ct = (CurveTrack)c;
                        if (ct.Curve.length > 1)
                        {
                            if (!found)
                            {
                                minTime = float.MaxValue;
                                maxTime = float.MinValue;
                                found = true;
                            }

                            float ctMinTime, ctMaxTime;
                            ct.GetTimeBounds(out ctMinTime, out ctMaxTime);

                            if (maxTime < ctMaxTime) maxTime = ctMaxTime;
                            if (minTime > ctMinTime) minTime = ctMinTime;
                        }
                    }
                }
            }
            if (maxTime - minTime < 0.1f)
                maxTime = minTime + 0.1f;
        }
        public override void FrameAll()
        {
            _MinValue = -1.0f;
            _MaxValue = 1.0f;
            if (Controls.Count > 1) // because of _MultiSelector
            {
                bool found = false;

                foreach (var c in Controls)
                {
                    if (c is CurveTrack)
                    {
                        CurveTrack ct = (CurveTrack)c;
                        if (ct.Curve.length > 1)
                        {
                            if (!found)
                            {
                                found = true;
                                _MinValue = float.MaxValue;
                                _MaxValue = float.MinValue;
                            }

                            float ctMinValue, ctMaxValue;
                            ct.GetValueBounds(out ctMinValue, out ctMaxValue);

                            if (_MinValue > ctMinValue) _MinValue = ctMinValue;
                            if (_MaxValue < ctMaxValue) _MaxValue = ctMaxValue;
                        }
                    }
                }

                double delta = _MaxValue - _MinValue;
                _MinValue -= delta * 0.05f;
                _MaxValue += delta * 0.05f;
            }

            _MinVisibleValue = _MinValue;
            _MaxVisibleValue = _MaxValue;
            base.FrameAll();
        }

        /// <summary>
        /// LayoutChanged
        /// </summary>
        protected override void OnLayoutChanged()
        {
            ZoomFactorY = (this._MaxValue - this._MinValue) / (this._MaxVisibleValue - this._MinVisibleValue);
            base.OnLayoutChanged();
        }
        protected override void PanY(float dy)
        {
            double deltaValue = ((double)dy / RenderAreaShrinksByPadding.height) * (_MaxVisibleValue - _MinVisibleValue);
            MinVisibleValue += deltaValue;
            MaxVisibleValue += deltaValue;
        }
        protected override void ScrollY(float dy)
        {
            if (Mathf.Approximately(dy, 0)) return;

            double deltaValue = (-dy / _ViewRect.height) * (_MaxValue - _MinValue);

            if (deltaValue < 0)
            {
                double min = _MinVisibleValue + deltaValue;
                if (min < _MinValue)
                {
                    min = _MinValue;
                    deltaValue = min - _MinVisibleValue;
                }
                MinVisibleValue = min;
                MaxVisibleValue = _MaxVisibleValue + deltaValue;
            }
            else
            {
                double max = _MaxVisibleValue + deltaValue;
                if (max > _MaxValue)
                {
                    max = _MaxValue;
                    deltaValue = max - _MaxVisibleValue;
                }
                MaxVisibleValue = max;
                MinVisibleValue = _MinVisibleValue + deltaValue;
            }
            //OnLayoutChanged();        

        }
        protected override void ZoomY(float dy)
        {
            double deltaValue = (-(double)dy / RenderAreaShrinksByPadding.height) * (_MaxVisibleValue - _MinVisibleValue);

            deltaValue *= 0.5;
            if (deltaValue < 0)
            {
                MinVisibleValue += deltaValue;
                MaxVisibleValue -= deltaValue;
            }
            else
            {
                MaxVisibleValue -= deltaValue;
                MinVisibleValue += deltaValue;
            }
        }
        protected override void UpdateLayout()
        {
            Rect ra = RenderAreaShrinksByPadding;
            double zoomFactor = TimeLine.ZoomFactor;

            _ViewRect = ra;
            _ViewRect.x = (float)(ra.x + TimeLine.MinTime * zoomFactor);
            _ViewRect.width = (float)(ra.width * zoomFactor);
            _ViewRect.height = (float)(ra.height * ZoomFactorY) - ScrollbarThickness;
            double deltaValue = _MaxValue - _MinValue;
            if (deltaValue < MinVisibleRange)
                deltaValue = MinVisibleRange;

            VerticalScroll = (float)(((_MaxValue - _MaxVisibleValue) / (deltaValue)) * _ViewRect.height);

            foreach (var c in Controls)
            {
                c.RenderArea = _ViewRect;
                if (c is Panel) ((Panel)c).Invalidate();
            }
        }

        #region Add/Remove Curve
        public CurveTrack AddCurve(AnimationCurve curve, Color color)
        {
            CurveTrack t = new CurveTrack(this, curve) { Color = color };
            Controls.Add(t);
            _MultiSelector.BringToFront();
            return t;
        }
        public void RemoveCurve(AnimationCurve curve)
        {
            foreach (var c in Controls)
            {
                if (c is CurveTrack)
                {
                    if (((CurveTrack)c).Curve == curve)
                    {
                        Controls.Remove(c);
                        break;
                    }
                }
            }
        }

        internal void RemoveAllCurves()
        {
            Controls.Clear();
            Controls.Add(_MultiSelector);
        }
        #endregion




    }
}