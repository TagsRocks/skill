using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{
    public class Vector2Track : ContinuousTrack<Vector2>
    {

        public override Type PropertyType { get { return typeof(Vector2); } }
        public override TrackType Type { get { return TrackType.Vector2; } }


        public override int CurveCount { get { return 2; } }

        public override AnimationCurve GetCurve(int index)
        {
            if (index == 0) return CurveX;
            return CurveY;
        }


        [SerializeField]
        [HideInInspector]
        [CurveEditor(1, 0, 0, "X")]
        public AnimationCurve CurveX;
        [SerializeField]
        [HideInInspector]
        [CurveEditor(0, 1, 0, "Y")]
        public AnimationCurve CurveY;


        [ExposeProperty(80, "CurveX")]
        public AnimationCurve CurveXP { get { return CurveX; } set { CurveX = value; } }

        [ExposeProperty(81, "CurveY")]
        public AnimationCurve CurveYP { get { return CurveY; } set { CurveY = value; } }
        

        protected override Vector2 EvaluateCurves(float time)
        {
            Vector2 result = new Vector2();
            if (CurveX != null) result.x = CurveX.Evaluate(time);
            if (CurveY != null) result.y = CurveY.Evaluate(time);
            return result;
        }
    }

}