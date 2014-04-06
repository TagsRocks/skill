using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{
    [System.Serializable]
    public class FloatKey : IPropertyKey<float>
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Value to set </summary>
        public float Value;
        /// <summary> Value curve </summary>
        [SerializeField]
        public AnimationCurve Curve;


        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }


        public float ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }

    public class FloatTrack : PropertyTrack<float>
    {
        public override Type PropertyType { get { return typeof(float); } }
        public override TrackType Type { get { return TrackType.Float; } }


        [SerializeField]
        [HideInInspector]
        public FloatKey[] Keys = new FloatKey[0];

        public override IPropertyKey<float>[] PropertyKeys { get { return Keys; } set { Keys = (FloatKey[])value; } }

        private FloatKey _TempKey = new FloatKey();
        protected override void Execute(IPropertyKey<float> key)
        {
            FloatKey fKey = (FloatKey)key;
            if (fKey.Curve != null)
            {
                _TempKey.Value = fKey.Curve.Evaluate(CurrecntTime - fKey.Time);
                base.Execute(_TempKey);
            }
            else
                base.Execute(key);
        }
    }

}