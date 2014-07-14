using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    public class Vector4Track : ContinuousTrack<Vector4>
    {

        public override Type PropertyType { get { return typeof(Vector4); } }
        public override TrackType Type { get { return TrackType.Vector4; } }


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

                if (CurveW != null && CurveW.length > 0)
                    maxLenght = Mathf.Max(maxLenght, CurveW.keys[CurveW.length - 1].time);

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
    }
}