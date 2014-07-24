using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    public class Vector4Track : ContinuousTrack<Vector4>
    {

        public override Type PropertyType { get { return typeof(Vector4); } }
        public override TrackType Type { get { return TrackType.Vector4; } }



        [SerializeField]
        [HideInInspector]
        [CurveEditor(1, 0, 0, "X")]
        public AnimationCurve CurveX;
        [SerializeField]
        [HideInInspector]
        [CurveEditor(0, 1, 0, "Y")]
        public AnimationCurve CurveY;
        [SerializeField]
        [HideInInspector]
        [CurveEditor(0, 0, 1, "Z")]
        public AnimationCurve CurveZ;
        [SerializeField]
        [HideInInspector]
        [CurveEditor(0, 1, 1, "W")]
        public AnimationCurve CurveW;


        protected override Vector4 EvaluateCurves(float time)
        {
            Vector4 result = new Vector4();
            if (CurveX != null) result.x = CurveX.Evaluate(time);
            if (CurveY != null) result.y = CurveY.Evaluate(time);
            if (CurveZ != null) result.z = CurveZ.Evaluate(time);
            if (CurveW != null) result.w = CurveW.Evaluate(time);
            return result;
        }


        public override int CurveCount { get { return 4; } }

        public override AnimationCurve GetCurve(int index)
        {
            if (index == 0) return CurveX;
            if (index == 1) return CurveY;
            if (index == 2) return CurveZ;
            return CurveW;
        }

    }
}