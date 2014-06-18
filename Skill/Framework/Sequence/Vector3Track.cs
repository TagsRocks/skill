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

        [CurveEditor(1, 0, 0, "X")]
        public AnimationCurve CurveX;
        [CurveEditor(0, 1, 0, "Y")]
        public AnimationCurve CurveY;
        [CurveEditor(0, 0, 1, "Z")]
        public AnimationCurve CurveZ;

        public Vector3 ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }

    public class Vector3Track : PropertyTrack<Vector3>
    {
        public override Type PropertyType { get { return typeof(Vector3); } }
        public override TrackType Type { get { return TrackType.Vector3; } }


        public override float Length
        {
            get
            {
                if (Keys != null && Keys.Length > 0)
                {
                    if (!Application.isPlaying)
                        SortKeys();

                    Vector3Key v3k = Keys[Keys.Length - 1];
                    float maxLenght = v3k.FireTime;

                    if (v3k.CurveX != null && v3k.CurveX.length > 0)
                        maxLenght = Mathf.Max(maxLenght, v3k.CurveX.keys[v3k.CurveX.length - 1].time);

                    if (v3k.CurveY != null && v3k.CurveY.length > 0)
                        maxLenght = Mathf.Max(maxLenght, v3k.CurveY.keys[v3k.CurveY.length - 1].time);

                    if (v3k.CurveZ != null && v3k.CurveZ.length > 0)
                        maxLenght = Mathf.Max(maxLenght, v3k.CurveZ.keys[v3k.CurveZ.length - 1].time);                    

                    return maxLenght;
                }
                return 0;
            }
        }

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