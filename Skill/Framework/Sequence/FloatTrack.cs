using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{
    public class FloatTrack : ContinuousTrack<float>
    {
        public override Type PropertyType { get { return typeof(float); } }
        public override TrackType Type { get { return TrackType.Float; } }


        public override int CurveCount { get { return 1; } }

        public override AnimationCurve GetCurve(int index)
        {
            return Curve;
        }


        [SerializeField]
        [HideInInspector]
        [CurveEditor(0, 1, 1, "Value")]
        public AnimationCurve Curve;


        [ExposeProperty(80, "Curve")]
        public AnimationCurve CurveP { get { return Curve; } set { Curve = value; } }


        protected override float EvaluateCurves(float time)
        {
            if (Curve != null)
                return Curve.Evaluate(time);
            return 0;
        }
    }

}