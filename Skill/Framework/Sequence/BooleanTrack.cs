using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    [System.Serializable]
    public class BooleanKey : IPropertyKey<bool>, ITrackKey
    {
        /// <summary> time to set </summary>
        public float Time;
        /// <summary> Value to set </summary>
        public bool Value;

        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }
        [ExposeProperty(1, "Value")]
        public bool ExValue { get { return Value; } set { Value = value; } }

        public bool ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0) this.Time = 0; } }
    }

    public class BooleanTrack : DiscreteTrack<bool>
    {
        [SerializeField]
        [HideInInspector]
        public BooleanKey[] Keys = new BooleanKey[0];
        public override Type PropertyType { get { return typeof(bool); } }
        public override TrackType Type { get { return TrackType.Bool; } }
        public override IPropertyKey<bool>[] PropertyKeys { get { return Keys; } set { Keys = (BooleanKey[])value; } }        
    }
}