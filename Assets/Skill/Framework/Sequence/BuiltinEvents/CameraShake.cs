using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    [CustomEventAttribute("CameraShake", "Signal")]
    public class CameraShake : EventKey
    {
        public Transform _Source;
        public Vector3 _Intensity = new Vector3(0.05f, 0.1f, 0.0f);
        public float _Roll = 0.5f;
        public float _Duration = 1.0f;
        public float _Range = 50;
        public bool _ByDistance = true;
        public float _TickTime = 0.1f;
        public float _Smoothing = 50.0f;


        [Skill.Framework.ExposeProperty(101, "Source", "Source of shake")]
        public Transform Source { get { return _Source; } set { _Source = value; } }

        [Skill.Framework.ExposeProperty(102, "Intensity", "Intensity of shake (x in left/right direction), (y in up/down direction), (z in back/forward (zoom) direction)")]
        public Vector3 Intensity { get { return _Intensity; } set { _Intensity = value; } }

        [Skill.Framework.ExposeProperty(103, "Roll", "Intensity of camera roll")]
        public float Roll { get { return _Roll; } set { _Roll = value; } }

        [Skill.Framework.ExposeProperty(104, "Duration", "Duration of shake")]
        public float Duration { get { return _Duration; } set { _Duration = value; } }

        [Skill.Framework.ExposeProperty(105, "Range", "Max distance to camera.")]
        public float Range { get { return _Range; } set { _Range = value; } }

        [Skill.Framework.ExposeProperty(106, "ByDistance", "The shake will be more stronger near to camera")]
        public bool ByDistance { get { return _ByDistance; } set { _ByDistance = value; } }

        [Skill.Framework.ExposeProperty(107, "Smoothing", "Shake smoothing")]
        public float Smoothing { get { return _Smoothing; } set { _Smoothing = value; } }

        [Skill.Framework.ExposeProperty(108, "TickTime", "time between shake directions")]
        public float TickTime { get { return _TickTime; } set { _TickTime = value; } }


        public override void FireEvent()
        {
            Skill.Framework.CameraShakeParams info = new Skill.Framework.CameraShakeParams();

            info.Intensity = this._Intensity;
            info.Roll = this._Roll;
            info.Duration = this._Duration;
            info.Range = this._Range;
            info.ByDistance = this._ByDistance;
            info.TickTime = this._TickTime;
            info.Smoothing = this._Smoothing;
            info.Enable = true;

            if (_Source != null)
                Skill.Framework.Global.RaiseCameraShake(this, info, _Source.position);
            else
                Skill.Framework.Global.RaiseCameraShake(this, info, Vector3.zero);
        }
    }
}