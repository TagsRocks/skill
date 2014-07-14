using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{
    public class Vector2Track : ContinuousTrack<Vector2>
    {

        public override Type PropertyType { get { return typeof(Vector2); } }
        public override TrackType Type { get { return TrackType.Vector2; } }


        public override float Length
        {
            get
            {

                float maxLenght = 0;
                if (CurveX != null && CurveX.length > 0)
                    maxLenght = Mathf.Max(maxLenght, CurveX.keys[CurveX.length - 1].time);

                if (CurveY != null && CurveY.length > 0)
                    maxLenght = Mathf.Max(maxLenght, CurveY.keys[CurveY.length - 1].time);

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

        protected override Vector2 EvaluateCurves(float time)
        {
            Vector2 result = new Vector2();
            if (CurveX != null) result.x = CurveX.Evaluate(time);
            if (CurveY != null) result.y = CurveY.Evaluate(time);
            return result;
        }
    }

}