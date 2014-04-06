using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{
    [System.Serializable]
    public class Vector2Key : IPropertyKey<Vector2>
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Value to set </summary>
        public Vector2 Value;

        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }

        public AnimationCurve CurveX;
        public AnimationCurve CurveY;

        public Vector2 ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }

    public class Vector2Track : PropertyTrack<Vector2>
    {

        public override Type PropertyType { get { return typeof(Vector2); } }
        public override TrackType Type { get { return TrackType.Vector2; } }


        [SerializeField]
        [HideInInspector]
        public Vector2Key[] Keys = new Vector2Key[0];

        public override IPropertyKey<Vector2>[] PropertyKeys { get { return Keys; } set { Keys = (Vector2Key[])value; } }

        private Vector2Key _TempKey = new Vector2Key();

        protected override void Execute(IPropertyKey<Vector2> key)
        {
            Vector2Key fKey = (Vector2Key)key;
            if (fKey.CurveX != null && fKey.CurveY != null)
            {
                float time = CurrecntTime - fKey.Time;
                float x = fKey.CurveX.Evaluate(time);
                float y = fKey.CurveY.Evaluate(time);
                _TempKey.Value = new Vector2(x, y);
                base.Execute(_TempKey);
            }
            else
                base.Execute(key);
        }
    }

}