using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

namespace Skill.Framework.Sequence
{
    public abstract class ContinuousTrack<V> : PropertyTrack<V>
    {
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
    }

}