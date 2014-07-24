using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{

    public class Vector3Track : ContinuousTrack<Vector3>
    {
        public override Type PropertyType { get { return typeof(Vector3); } }
        public override TrackType Type { get { return TrackType.Vector3; } }


        public override int CurveCount { get { return 3; } }

        public override AnimationCurve GetCurve(int index)
        {
            if (index == 0) return CurveX;
            if (index == 1) return CurveY;
            return CurveZ;
        }


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

        protected override Vector3 EvaluateCurves(float time)
        {
            Vector3 result = new Vector3();
            if (CurveX != null) result.x = CurveX.Evaluate(time);
            if (CurveY != null) result.y = CurveY.Evaluate(time);
            if (CurveZ != null) result.z = CurveZ.Evaluate(time);
            return result;
        }
    }
}