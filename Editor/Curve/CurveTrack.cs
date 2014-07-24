using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;

namespace Skill.Editor.Curve
{
    public class CurveTrack : Canvas
    {
        private class CurvePart
        {
            public int StartIndex;
            public int Count;
        }

        public AnimationCurve Curve { get; private set; }
        public TimeLineCurveView View { get; private set; }
        public Color Color { get; set; }

        private List<Vector2> _Samples;
        private List<CurvePart> _Parts;
        private Vector3[] _Points;
        private bool _Resample;
        private bool _UpdatePoints;
        private float _MinValue;
        private float _MaxValue;
        public override void Invalidate()
        {
            _Resample = true;
            base.Invalidate();
        }
        protected override void OnLayoutChanged()
        {
            _Resample = true;
            base.OnLayoutChanged();
        }

        public event System.EventHandler Changed;
        private void OnChanged()
        {
            if (Changed != null) Changed(this, System.EventArgs.Empty);
        }

        public void GetTimeBounds(out float minTime, out float maxTime)
        {
            if (Curve.length > 1)
            {
                minTime = Curve[0].time;
                maxTime = Curve[Curve.length - 1].time;
                if (maxTime - minTime < 0.1f)
                    maxTime = minTime + 0.1f;
            }
            else
            {
                minTime = 0;
                maxTime = 0.1f;
            }
        }
        public void GetValueBounds(out float minValue, out float maxValue)
        {
            if (Curve.length > 1)
            {
                Resample();
                minValue = _MinValue;
                maxValue = _MaxValue;
            }
            else
            {
                minValue = -0.1f;
                maxValue = 0.1f;
            }
            if (maxValue - minValue < 0.1f)
                maxValue = minValue + 0.1f;
        }


        public CurveTrack(TimeLineCurveView view, AnimationCurve curve)
        {
            if (view == null) throw new System.ArgumentNullException("Invalid TimeLineCurveView");
            if (curve == null) throw new System.ArgumentNullException("Invalid AnimationCurve");

            this.View = view;
            this.Curve = curve;
            this.Color = Color.green;

            this._Samples = new List<Vector2>(2000);
            this._Parts = new List<CurvePart>(5);
            this.IsInScrollView = true;
            RebuildKeys();
        }
        public void RebuildKeys()
        {
            Controls.Clear();
            Keyframe[] keys = Curve.keys;
            if (keys != null && keys.Length > 0)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    var k = new CurveKey(this, i) { ContextMenu = CurveKeyContextMenu.Instance };
                    Controls.Add(k);
                }
            }
        }
        private CurvePart CreatePart(int startIndex)
        {
            CurvePart part = new CurvePart();
            part.StartIndex = startIndex;
            part.Count = 0;
            _Parts.Add(part);
            return part;
        }
        private void Resample()
        {
            if (_Resample)
            {
                _UpdatePoints = true;
                _Resample = false;
                _Parts.Clear();
                if (Curve.length > 1)
                {
                    _MinValue = float.MaxValue;
                    _MaxValue = float.MinValue;
                    float minTime, maxTime;
                    GetTimeBounds(out minTime, out maxTime);

                    int index = -1;
                    CurvePart part = null;

                    double startTime = View.TimeLine.StartVisible;
                    if (startTime < minTime) startTime = minTime;

                    double endTime = View.TimeLine.EndVisible;
                    if (endTime > maxTime) endTime = maxTime;

                    double stepTime = (endTime - startTime) / View.RenderArea.width;

                    if (stepTime <= 0.0)
                    {
                        _MinValue = 0;
                        _MaxValue = 1;
                        return;
                    }

                    double time = startTime;
                    while (time <= endTime)
                    {
                        float fTime = (float)time;
                        float value = Curve.Evaluate(fTime);

                        _MinValue = Mathf.Min(_MinValue, value);
                        _MaxValue = Mathf.Max(_MaxValue, value);

                        if (value >= View.MinVisibleValue && value <= View.MaxVisibleValue)
                        {
                            index++;
                            Vector2 sample = GetPoint(fTime, value, false);
                            if (part == null)
                                part = CreatePart(index);

                            if (_Samples.Count > index)
                                _Samples[index] = sample;
                            else
                                _Samples.Add(sample);

                            part.Count++;
                        }
                        else if (part != null)
                        {
                            part = null;
                            index++;
                            if (_Samples.Count > index)
                                _Samples[index] = _Samples[index - 1];
                            else
                                _Samples.Add(_Samples[index - 1]);
                        }

                        time += stepTime;
                    }
                }
                else
                {
                    _MinValue = 0;
                    _MaxValue = 1;
                }
            }
        }

        protected override void BeginRender()
        {
            if (Controls.Count != Curve.length)
                RebuildKeys();
            base.BeginRender();
        }

        protected override void Render()
        {
            Resample();
            if (_Parts.Count > 0)
            {
                foreach (var p in _Parts)
                {
                    if (p.Count > 1)
                        Skill.Editor.LineDrawer.DrawPolyLineGL(_Samples, Color, p.StartIndex, p.Count);
                }
            }
            base.Render();
        }

        public void MoveKey(int index, Keyframe key)
        {
            Curve.MoveKey(index, key);
            _Resample = true;
            OnChanged();
        }
        public void AddKey(float time)
        {
            float value = Curve.Evaluate(time);
            int keyIndex = Curve.AddKey(time, value);

            Skill.Editor.CurveUtility.SetKeyModeFromContext(Curve, keyIndex);
            Skill.Editor.CurveUtility.UpdateTangentsFromModeSurrounding(Curve, keyIndex);

            RebuildKeys();
            View.Editor.Select((CurveKey)Controls[keyIndex]);
            _Resample = true;
            OnChanged();
        }

        private List<int> _RemovingKeys = new List<int>();
        public void RegisterForRemove(int index)
        {
            _RemovingKeys.Add(index);
        }

        public void RemoveKeys()
        {
            if (_RemovingKeys.Count > 0)
            {
                _RemovingKeys.Sort();
                for (int i = _RemovingKeys.Count - 1; i >= 0; i--)
                    Curve.RemoveKey(_RemovingKeys[i]);
                RebuildKeys();
                _Resample = true;
                _RemovingKeys.Clear();
                OnChanged();

            }
        }

        public Vector2 GetPoint(float time, float value, bool relative)
        {
            Rect ra = RenderArea;
            return GetPoint(time, value, relative, ref ra);
        }
        private Vector2 GetPoint(float time, float value, bool relative, ref Rect renderArea)
        {
            return new Vector2(GetX(time, relative, ref renderArea), GetY(value, relative, ref renderArea));
        }

        public float GetX(float time, bool relative)
        {
            Rect ra = RenderArea;
            return GetX(time, relative, ref ra);
        }
        private float GetX(float time, bool relative, ref Rect renderArea)
        {
            float x;
            if (renderArea.width > Mathf.Epsilon)
                x = renderArea.x + (float)((time - View.TimeLine.MinTime) / (View.TimeLine.MaxTime - View.TimeLine.MinTime)) * renderArea.width;
            else
                x = renderArea.x;
            if (relative)
                x -= renderArea.x;
            return x;
        }

        public float GetY(float value, bool relative)
        {
            Rect ra = RenderArea;
            return GetY(value, relative, ref ra);
        }
        private float GetY(float value, bool relative, ref Rect renderArea)
        {
            float y = (float)(((value - View.MinValue) / (View.MaxValue - View.MinValue)) * renderArea.height);
            y = renderArea.yMax - y;

            if (relative)
                y -= renderArea.yMin;
            return y;
        }


        public float GetTime(float x, bool relative)
        {
            Rect renderArea = RenderArea;
            return GetTime(x, relative, ref renderArea);
        }
        private float GetTime(float x, bool relative, ref Rect renderArea)
        {
            if (relative) x += renderArea.xMin;
            if (renderArea.width > Mathf.Epsilon)
                return (float)(View.TimeLine.MinTime + ((x - renderArea.xMin) / renderArea.width) * (View.TimeLine.MaxTime - View.TimeLine.MinTime));
            else
                return 0;
        }


        public float GetValue(float y, bool relative)
        {
            Rect renderArea = RenderArea;
            return GetValue(y, relative, ref renderArea);
        }
        private float GetValue(float y, bool relative, ref Rect renderArea)
        {
            if (relative) y += renderArea.yMin;
            return (float)(View.MaxValue - (((y - renderArea.yMin) / renderArea.height) * (View.MaxValue - View.MinValue)));
        }


        private void UpdatePoints()
        {
            if (_UpdatePoints)
            {
                _UpdatePoints = false;
                if (_Samples != null)
                    _Points = new Vector3[_Samples.Count];
                for (int i = 0; i < _Samples.Count; i++)
                    _Points[i] = _Samples[i];
            }
        }
        public override void HandleEvent(Event e)
        {
            if (e != null && e.type != EventType.Used && e.type == EventType.ContextClick)
            {
                ContextMenu = null;
                Resample();
                UpdatePoints();
                if (_Points != null && _Points.Length > 1)
                {
                    if (UnityEditor.HandleUtility.DistanceToPolyLine(_Points) < 3.0f)
                        ContextMenu = CurveTrackContextMenu.Instance;
                }
            }
            base.HandleEvent(e);
        }
    }
}
