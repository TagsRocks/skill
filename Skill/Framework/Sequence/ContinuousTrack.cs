using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

namespace Skill.Framework.Sequence
{
    public abstract class ContinuousTrack<V> : PropertyTrack<V>
    {
        public abstract int CurveCount { get; }
        public abstract AnimationCurve GetCurve(int index);

        protected abstract V EvaluateCurves(float time);

        public override void Evaluate(float time)
        {
            SetValue(EvaluateCurves(time));
        }
        public override void Seek(float time)
        {
            SetValue(EvaluateCurves(time));
        }

        public override void SortKeys()
        {
        }

        public override void GetTimeBounds(out float minTime, out float maxTime)
        {
            minTime = 0.0f;
            maxTime = 0.0f;

            if (CurveCount > 0)
            {
                for (int i = 0; i < CurveCount; i++)
                {
                    AnimationCurve curve = GetCurve(i);
                    if (curve != null && curve.length > 0)
                    {
                        maxTime = Mathf.Max(maxTime, curve.keys[curve.length - 1].time);
                        minTime = Mathf.Min(minTime, curve.keys[0].time);
                    }
                }
            }
            if (maxTime - minTime < 0.1f)
                maxTime = minTime + 0.1f;
        }
    }

}