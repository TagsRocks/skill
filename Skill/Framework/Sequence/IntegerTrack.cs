using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    [System.Serializable]
    public class IntegerKey : IPropertyKey<int>
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Value to set </summary>
        public int Value;

        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }
        [ExposeProperty(1, "Value")]
        public int ExValue { get { return Value; } set { Value = value; } }



        public int ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }

    public class IntegerTrack : PropertyTrack<int>
    {
        public override Type PropertyType { get { return typeof(int); } }
        public override TrackType Type { get { return TrackType.Integer; } }

        [SerializeField]
        [HideInInspector]
        public IntegerKey[] Keys = new IntegerKey[0];

        public override IPropertyKey<int>[] PropertyKeys { get { return Keys; } set { Keys = (IntegerKey[])value; } }
    }
}