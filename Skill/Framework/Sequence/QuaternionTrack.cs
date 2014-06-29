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

        [CurveEditor(1, 0, 0, "X")]
        public AnimationCurve CurveX;
        [CurveEditor(0, 1, 0, "Y")]
        public AnimationCurve CurveY;
        [CurveEditor(0, 0, 1, "Z")]
        public AnimationCurve CurveZ;

        public Quaternion ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }
    public class QuaternionTrack : PropertyTrack<Quaternion>
    {
        public override Type PropertyType { get { return typeof(Quaternion); } }
        public override TrackType Type { get { return TrackType.Quaternion; } }

        public override float Length
        {
            get
            {
                if (Key != null)
                {

                    float maxLenght = Key.FireTime;

                    if (Key.CurveX != null && Key.CurveX.length > 0)
                        maxLenght = Mathf.Max(maxLenght, Key.CurveX.keys[Key.CurveX.length - 1].time);

                    if (Key.CurveY != null && Key.CurveY.length > 0)
                        maxLenght = Mathf.Max(maxLenght, Key.CurveY.keys[Key.CurveY.length - 1].time);

                    if (Key.CurveZ != null && Key.CurveZ.length > 0)
                        maxLenght = Mathf.Max(maxLenght, Key.CurveZ.keys[Key.CurveZ.length - 1].time);

                    return maxLenght;
                }
                return 0;
            }
        }

        [SerializeField]
        [HideInInspector]
        public QuaternionKey Key;

        public override IPropertyKey<Quaternion>[] PropertyKeys
        {
            get
            {
                if (Key == null)
                    Key = new QuaternionKey() { Time = 0, Value = Quaternion.identity };
                return new IPropertyKey<Quaternion>[] { Key };
            }
            set
            {
                Key = (QuaternionKey)value[0];
            }
        }

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