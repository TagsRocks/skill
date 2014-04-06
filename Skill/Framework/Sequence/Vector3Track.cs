using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    [System.Serializable]
    public class Vector3Key : IPropertyKey<Vector3>
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Value to set </summary>
        public Vector3 Value;

        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }

        public AnimationCurve CurveX;
        public AnimationCurve CurveY;
        public AnimationCurve CurveZ;

        public Vector3 ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }

    public class Vector3Track : PropertyTrack<Vector3>
    {
        public override Type PropertyType { get { return typeof(Vector3); } }
        public override TrackType Type { get { return TrackType.Vector3; } }


        [SerializeField]
        [HideInInspector]
        public Vector3Key[] Keys = new Vector3Key[0];

        public override IPropertyKey<Vector3>[] PropertyKeys { get { return Keys; } set { Keys = (Vector3Key[])value; } }

        private Vector3Key _TempKey = new Vector3Key();
        protected override void Execute(IPropertyKey<Vector3> key)
        {
            Vector3Key fKey = (Vector3Key)key;
            if (fKey.CurveX != null && fKey.CurveY != null && fKey.CurveZ != null)
            {
                float time = CurrecntTime - fKey.Time;
                float x = fKey.CurveX.Evaluate(time);
                float y = fKey.CurveY.Evaluate(time);
                float z = fKey.CurveZ.Evaluate(time);
                _TempKey.Value = new Vector3(x, y, z);
                base.Execute(_TempKey);
            }
            else
                base.Execute(key);
        }
    }
}