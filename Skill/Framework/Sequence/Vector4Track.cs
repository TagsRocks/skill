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

        [CurveEditor(1, 0, 0, "X")]
        public AnimationCurve CurveX;
        [CurveEditor(0, 1, 0, "Y")]
        public AnimationCurve CurveY;
        [CurveEditor(0, 0, 1, "Z")]
        public AnimationCurve CurveZ;
        [CurveEditor(0, 1, 1, "W")]
        public AnimationCurve CurveW;

        public Vector4 ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }


    public class Vector4Track : PropertyTrack<Vector4>
    {

        public override Type PropertyType { get { return typeof(Vector4); } }
        public override TrackType Type { get { return TrackType.Vector4; } }


        public override float Length
        {
            get
            {
                if (Key  != null )
                {
                    float maxLenght = Key.FireTime;

                    if (Key.CurveX != null && Key.CurveX.length > 0)
                        maxLenght = Mathf.Max(maxLenght, Key.CurveX.keys[Key.CurveX.length - 1].time);

                    if (Key.CurveY != null && Key.CurveY.length > 0)
                        maxLenght = Mathf.Max(maxLenght, Key.CurveY.keys[Key.CurveY.length - 1].time);

                    if (Key.CurveZ != null && Key.CurveZ.length > 0)
                        maxLenght = Mathf.Max(maxLenght, Key.CurveZ.keys[Key.CurveZ.length - 1].time);

                    if (Key.CurveW != null && Key.CurveW.length > 0)
                        maxLenght = Mathf.Max(maxLenght, Key.CurveW.keys[Key.CurveW.length - 1].time);

                    return maxLenght;
                }
                return 0;
            }
        }


        [SerializeField]
        [HideInInspector]
        public Vector4Key Key;

        public override IPropertyKey<Vector4>[] PropertyKeys
        {
            get
            {
                if (Key == null)
                    Key = new Vector4Key() { Time = 0, Value = Vector3.zero };
                return new IPropertyKey<Vector4>[] { Key };
            }
            set
            {
                Key = (Vector4Key)value[0];
            }
        }

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