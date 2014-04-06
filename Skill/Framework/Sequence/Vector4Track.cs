using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    [System.Serializable]
    public class Vector4Key : IPropertyKey<Vector4>
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Value to set </summary>
        public Vector4 Value;

        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }

        public AnimationCurve CurveX;
        public AnimationCurve CurveY;
        public AnimationCurve CurveZ;
        public AnimationCurve CurveW;

        public Vector4 ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }


    public class Vector4Track : PropertyTrack<Vector4>
    {

        public override Type PropertyType { get { return typeof(Vector4); } }
        public override TrackType Type { get { return TrackType.Vector4; } }



        [SerializeField]
        [HideInInspector]
        public Vector4Key[] Keys = new Vector4Key[0];

        public override IPropertyKey<Vector4>[] PropertyKeys { get { return Keys; } set { Keys = (Vector4Key[])value; } }

        private Vector4Key _TempKey = new Vector4Key();
        protected override void Execute(IPropertyKey<Vector4> key)
        {
            Vector4Key fKey = (Vector4Key)key;
            if (fKey.CurveX != null && fKey.CurveY != null && fKey.CurveZ != null && fKey.CurveW != null)
            {
                float time = CurrecntTime - fKey.Time;
                float x = fKey.CurveX.Evaluate(time);
                float y = fKey.CurveY.Evaluate(time);
                float z = fKey.CurveZ.Evaluate(time);
                float w = fKey.CurveW.Evaluate(time);
                _TempKey.Value = new Vector4(x, y, z, w);
                base.Execute(_TempKey);
            }
            else
                base.Execute(key);
        }
    }
}