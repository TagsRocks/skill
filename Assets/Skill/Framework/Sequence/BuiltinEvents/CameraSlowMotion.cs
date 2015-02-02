using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    [CustomEventAttribute("SlowMotion", "Time")]
    public class CameraSlowMotion : EventKey
    {

        public float _Freez = 0.0f;
        public float _SlowMotion = 2.0f;
        public float _TimeScale = 0.2f;
        public float _Pitch = 0.5f;


        [Skill.Framework.ExposeProperty(101, "Freez", "Lenght of freez time at begining of slow motion.( Freez time is calculated as part of SlowMotion time )")]
        public float Freez { get { return _Freez; } set { _Freez = value; } }

        [Skill.Framework.ExposeProperty(102, "SlowMotion", "Lenght of slow motion")]
        public float SlowMotion { get { return _SlowMotion; } set { _SlowMotion = value; } }

        [Skill.Framework.ExposeProperty(103, "TimeScale", "Target TimeScale when slow motion")]
        public float TimeScale { get { return _TimeScale; } set { _TimeScale = value; } }

        [Skill.Framework.ExposeProperty(104, "Pitch", "Target sound pitch when slow motion")]
        public float Pitch { get { return _Pitch; } set { _Pitch = value; } }


        public override void FireEvent()
        {
            Skill.Framework.SlowMotionInfo info = new Skill.Framework.SlowMotionInfo();

            info.Freez = _Freez;
            info.SlowMotion = _SlowMotion;
            info.TimeScale = _TimeScale;
            info.Pitch = _Pitch;

            Skill.Framework.Global.RaiseSlowMotion(this, info);
        }
    }
}