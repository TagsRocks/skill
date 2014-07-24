using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    public class QuaternionTrack : ContinuousTrack<Quaternion>
    {
        public override Type PropertyType { get { return typeof(Quaternion); } }
        public override TrackType Type { get { return TrackType.Quaternion; } }

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

        protected override Quaternion EvaluateCurves(float time)
        {
            Vector3 eulers = new Vector3();
            if (CurveX != null) eulers.x = CurveX.Evaluate(time);
            if (CurveY != null) eulers.y = CurveY.Evaluate(time);
            if (CurveZ != null) eulers.z = CurveZ.Evaluate(time);
            return Quaternion.Euler(eulers);
        }
    }
}