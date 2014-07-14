using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{
    public class FloatTrack : ContinuousTrack<float>
    {
        public override Type PropertyType { get { return typeof(float); } }
        public override TrackType Type { get { return TrackType.Float; } }


        public override float Length
        {
            get
            {
                if (Curve != null && Curve.length > 0)
                    return Curve[Curve.length - 1].time;
                else
                    return 0;
            }
        }


        [SerializeField]
        [HideInInspector]
        [CurveEditor(1, 0, 1, "Value")]
        public AnimationCurve Curve;


        protected override float EvaluateCurves(float time)
        {
            if (Curve != null)
                return Curve.Evaluate(time);
            return 0;
        }        
    }

}