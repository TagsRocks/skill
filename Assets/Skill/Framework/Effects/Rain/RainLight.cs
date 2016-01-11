using UnityEngine;
using System.Collections;
using Skill.Framework;
namespace Skill.Framework.Effects
{
    [RequireComponent(typeof(AudioSource))]
    public class RainLight : DynamicBehaviour
    {
        public Light SceneLight;
        public Color LightColor;
        public AnimationCurve[] Curves;
        public AudioClip[] Sounds;
        public ParticleSystem LightingParticle;

        public float ExtraIntensity = 3;
        public float Interval = 30;
        public float CurveSpeed = 1;

        private float _PreIntensity;
        private Color _PreLightColor;
        private TimeWatch _IntervalTW;
        private TimeWatch _SoundTW;
        private AnimationCurve _ActiveCurve;
        private float _CurveTime;
        private float _MaxCurveTime;
        private AudioSource _Audio;


        protected override void GetReferences()
        {
            base.GetReferences();
            _Audio = GetComponent<AudioSource>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Reschedule();
            if (LightingParticle != null)
            {
                var emission = LightingParticle.emission;
                emission.enabled = false;
            }
        }

        void OnDisable()
        {
            if (_ActiveCurve != null)
            {
                _ActiveCurve = null;
                if (SceneLight)
                {
                    SceneLight.intensity = _PreIntensity;
                    SceneLight.color = _PreLightColor;
                    _SoundTW.End();
                }
            }
            if (_Audio.isPlaying)
                _Audio.Stop();
            _CurveTime = _MaxCurveTime;
        }

        private void Reschedule()
        {
            _IntervalTW.Begin(Random.Range(Interval * 0.6f, Interval));
        }

        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (_SoundTW.IsEnabledAndOver)
            {
                _SoundTW.End();
                Global.Instance.PlayOneShot(_Audio, Sounds[Random.Range(0, Sounds.Length)], Skill.Framework.Audio.SoundCategory.FX);
            }

            if (_IntervalTW.IsEnabledAndOver)
            {
                _ActiveCurve = Curves[Random.Range(0, Curves.Length)];
                _CurveTime = 0;
                _MaxCurveTime = _ActiveCurve[_ActiveCurve.length - 1].time;
                _SoundTW.Begin(_MaxCurveTime + 1);
                _PreIntensity = SceneLight.intensity;
                _PreLightColor = SceneLight.color;
                Reschedule();
                if (LightingParticle != null)
                    LightingParticle.Emit(1);
            }
            else if (_ActiveCurve != null)
            {
                _CurveTime += Time.deltaTime * CurveSpeed;
                if (_CurveTime >= _MaxCurveTime)
                {
                    _ActiveCurve = null;
                    SceneLight.intensity = _PreIntensity;
                    SceneLight.color = _PreLightColor;
                }
                else
                {
                    float factor = _ActiveCurve.Evaluate(_CurveTime);
                    SceneLight.intensity = _PreIntensity + (factor * ExtraIntensity);
                    SceneLight.color = Color.Lerp(_PreLightColor, LightColor, factor);
                }
            }

            base.Update();
        }
    }
}