using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

namespace Skill.Framework.Sequence
{
    public abstract class ContinuousTrack<V> : PropertyTrack<V>
    {
        private float _MinTime;
        private float _MaxTime;

        public abstract int CurveCount { get; }
        public abstract AnimationCurve GetCurve(int index);

        private bool HasKey()
        {
            for (int i = 0; i < CurveCount; i++)
            {
                if (GetCurve(i).length > 0)
                    return true;
            }
            return false;
        }

        protected abstract V EvaluateCurves(float time);

        public override void Evaluate(float time)
        {
            if (HasKey() && time >= _MinTime && time <= _MaxTime)
                SetValue(EvaluateCurves(time));
        }
        public override void Seek(float time)
        {
            if (HasKey() && time >= _MinTime && time <= _MaxTime)
                SetValue(EvaluateCurves(time));
        }

        public override void SortKeys()
        {
            _MinTime = 0;
            _MaxTime = 0;

            bool found = false;
            for (int i = 0; i < CurveCount; i++)
            {
                var curve = GetCurve(i);
                if (curve != null)
                {
                    if (curve.length > 1)
                    {
                        if (!found)
                        {
                            found = true;
                            _MinTime = float.MaxValue;
                            _MaxTime = float.MinValue;
                        }

                        _MinTime = Mathf.Min(_MinTime, curve[0].time);
                        _MaxTime = Mathf.Max(_MaxTime, curve[curve.length - 1].time);
                    }
                }
            }

        }

        public override void GetTimeBounds(out float minTime, out float maxTime)
        {
            minTime = 0.0f;
            maxTime = 0.0f;
            bool found = false;

            if (CurveCount > 0)
            {
                for (int i = 0; i < CurveCount; i++)
                {
                    AnimationCurve curve = GetCurve(i);
                    if (curve != null && curve.length > 0)
                    {
                        if (!found)
                        {
                            found = true;
                            minTime = float.MaxValue;
                            maxTime = float.MinValue;
                        }

                        minTime = Mathf.Min(minTime, curve.keys[0].time);
                        maxTime = Mathf.Max(maxTime, curve.keys[curve.length - 1].time);
                    }
                }
            }
            if (maxTime - minTime < 0.1f)
                maxTime = minTime + 0.1f;
        }
    }

}