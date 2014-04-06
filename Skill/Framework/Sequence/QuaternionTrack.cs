using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{

    [System.Serializable]
    public class QuaternionKey : IPropertyKey<Quaternion>
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Value to set </summary>
        public Quaternion Value;

        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }

        public AnimationCurve CurveX;
        public AnimationCurve CurveY;
        public AnimationCurve CurveZ;

        public Quaternion ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }
    public class QuaternionTrack : PropertyTrack<Quaternion>
    {
        public override Type PropertyType { get { return typeof(Quaternion); } }
        public override TrackType Type { get { return TrackType.Quaternion; } }

        [SerializeField]
        [HideInInspector]
        public QuaternionKey[] Keys = new QuaternionKey[0];

        public override IPropertyKey<Quaternion>[] PropertyKeys { get { return Keys; } set { Keys = (QuaternionKey[])value; } }

        private QuaternionKey _TempKey = new QuaternionKey();

        protected override void Execute(IPropertyKey<Quaternion> key)
        {
            QuaternionKey fKey = (QuaternionKey)key;
            if (fKey.CurveX != null && fKey.CurveY != null && fKey.CurveZ != null)
            {
                float time = CurrecntTime - fKey.Time;
                float x = fKey.CurveX.Evaluate(time);
                float y = fKey.CurveY.Evaluate(time);
                float z = fKey.CurveZ.Evaluate(time);
                _TempKey.Value = Quaternion.Euler(x, y, z);
                base.Execute(_TempKey);
            }
            else
                base.Execute(key);


        }
    }
}