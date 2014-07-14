using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    public class QuaternionTrack : ContinuousTrack<Quaternion>
    {
        public override Type PropertyType { get { return typeof(Quaternion); } }
        public override TrackType Type { get { return TrackType.Quaternion; } }

        public override float Length
        {
            get
            {

                float maxLenght = 0;
                if (CurveX != null && CurveX.length > 0)
                    maxLenght = Mathf.Max(maxLenght, CurveX.keys[CurveX.length - 1].time);

                if (CurveY != null && CurveY.length > 0)
                    maxLenght = Mathf.Max(maxLenght, CurveY.keys[CurveY.length - 1].time);

                if (CurveZ != null && CurveZ.length > 0)
                    maxLenght = Mathf.Max(maxLenght, CurveZ.keys[CurveZ.length - 1].time);

                return maxLenght;

            }
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