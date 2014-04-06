using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    [System.Serializable]
    public class ColorKey : IPropertyKey<Color>
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Value to set </summary>
        public Color Value;


        [ExposeProperty(0, "Time", "Event time")]
        public float ExTime { get { return Time; } set { Time = value; } }
        [ExposeProperty(1, "Color")]
        public Color ExValue { get { return Value; } set { Value = value; } }






        public Color ValueKey { get { return this.Value; } set { this.Value = value; } }
        public float FireTime { get { return this.Time; } set { this.Time = value; if (this.Time < 0)this.Time = 0; } }
    }

    public class ColorTrack : PropertyTrack<Color>
    {
        [SerializeField]
        [HideInInspector]
        public ColorKey[] Keys;

        public override Type PropertyType { get { return typeof(Color); } }
        public override TrackType Type { get { return TrackType.Color; } }
        public override IPropertyKey<Color>[] PropertyKeys { get { return Keys; } set { Keys = (ColorKey[])value; } }


        private ColorKey _TempKey = new ColorKey();

        protected override void Execute(IPropertyKey<Color> key)
        {
            IPropertyKey<Color> nextKey = GetNextKey();
            if (nextKey != null)
            {
                _TempKey.Value = Color.Lerp(key.ValueKey, nextKey.ValueKey, (CurrecntTime - key.FireTime) / (nextKey.FireTime - key.FireTime));
                base.Execute(_TempKey);
            }
            else
                base.Execute(key);
        }
    }
}