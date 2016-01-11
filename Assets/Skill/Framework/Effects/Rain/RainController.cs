using UnityEngine;
using System.Collections;
using Skill.Framework;
namespace Skill.Framework.Effects
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Fading))]
    public class RainController : DynamicBehaviour
    {
        public static RainController Instance { get; private set; }

        public Light SceneLight;
        public RainLight RainLight;
        public ParticleEmitter Particle;
        public SpecularController Specular;

        public float RainVolume = 0.5f;
        public Color RainSpecular;
        
        private float _MinEmission;
        private float _MaxEmission;
        private TimeWatch _FadeTW;
        private bool _Starting;
        private bool _Stopping;
        private AudioSource _RainAudio;
        private Fading _Fading;


        protected override void GetReferences()
        {
            base.GetReferences();
            _RainAudio = GetComponent<AudioSource>();
            _Fading = GetComponent<Fading>();
        }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
            if (RainLight.SceneLight == null)
                RainLight.SceneLight = this.SceneLight;
            RainLight.enabled = false;
            _MinEmission = Particle.minEmission;
            _MaxEmission = Particle.maxEmission;
            Particle.gameObject.SetActive(false);
            _Fading.Alpha = 1.0f;
            _RainAudio.volume = 0;
            _RainAudio.Stop();
        }

        private bool _IsRaining;
        public bool IsRaining { get { return _IsRaining; } }

        public void StartRain()
        {
            if (_IsRaining) return;
            _IsRaining = true;
            _RainAudio.Play();
            _Fading.FadeToZero();
            _Starting = true;
            _Stopping = false;
            RainLight.enabled = true;
            Particle.minEmission = 0;
            Particle.maxEmission = 0;
            Particle.gameObject.SetActive(true);
            Specular.SpecularColor = Specular.DefaultColor;
            _FadeTW.Begin(_Fading.FadeInTime + 0.1f);
        }

        public void StopRain()
        {
            if (!_IsRaining) return;
            _IsRaining = false;

            RainLight.enabled = false;
            _Fading.FadeToOne();
            _Starting = false;
            _Stopping = true;
            _FadeTW.Begin(_Fading.FadeOutTime + 0.1f);


            Particle.minEmission = _MinEmission;
            Particle.maxEmission = _MaxEmission;
            Specular.SpecularColor = RainSpecular;
        }


        protected override void Update()
        {
            base.Update();
            if (Global.Instance != null)
                _RainAudio.volume = (1.0f - _Fading.Alpha) * (Global.Instance.Settings.Audio.FxVolume * RainVolume);
            else
                _RainAudio.volume = (1.0f - _Fading.Alpha) * RainVolume;

            if (_FadeTW.IsEnabled)
            {
                if (_FadeTW.IsOver)
                {
                    _FadeTW.End();
                    if (_Starting)
                    {
                        Particle.minEmission = _MinEmission;
                        Particle.maxEmission = _MaxEmission;
                        Specular.SpecularColor = RainSpecular;
                    }
                    if (_Stopping)
                    {
                        Particle.minEmission = 0;
                        Particle.maxEmission = 0;
                        Particle.gameObject.SetActive(false);
                        Specular.SpecularColor = Specular.DefaultColor;
                        _RainAudio.Stop();
                    }

                    _Starting = false;
                    _Stopping = false;
                }
                else
                {
                    float percent = _FadeTW.Percent;
                    if (_Stopping)
                        percent = 1.0f - percent;

                    Particle.minEmission = Mathf.Lerp(0, _MinEmission, percent * 3);
                    Particle.maxEmission = Mathf.Lerp(0, _MaxEmission, percent * 3);
                    Specular.SpecularColor = Color.Lerp(Specular.DefaultColor, RainSpecular, percent);
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Toggle")]
        public void ToggleRain()
        {
            if (Application.isPlaying)
            {
                if (_IsRaining)
                    StopRain();
                else
                    StartRain();
            }
            else
            {
                Debug.LogError("Toggle rain works only in play mode");
            }
        }

#endif
    }
}