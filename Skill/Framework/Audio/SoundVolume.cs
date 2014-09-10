using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Audio
{
    /// <summary> Set volume of all audios on enable </summary>
    public class SoundVolume : StaticBehaviour
    {
        /// <summary> Category of sound </summary>
        public Skill.Framework.Audio.SoundCategory Category = Skill.Framework.Audio.SoundCategory.FX;
        /// <summary> Volume factor </summary>
        public float VolumeFactor = 1;

        private AudioSource[] _Audios;

        protected override void GetReferences()
        {
            base.GetReferences();
            _Audios = GetComponents<AudioSource>();
        }

        private void SetVolume()
        {
            float volume = 0;
            if (Global.Instance != null)
                volume = Global.Instance.Settings.Audio.GetVolume(Category) * VolumeFactor;
            foreach (var audio in _Audios)
            {
                if (audio != null)
                    audio.volume = volume;
            }

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            SetVolume();
        }
    }

    /// <summary>
    /// Manage volume of sounds every frame
    /// </summary>
    public class DynamicSoundVolume : DynamicBehaviour
    {
        public Skill.Framework.Audio.SoundCategory Category = Skill.Framework.Audio.SoundCategory.FX;
        public float VolumeFactor = 1;
        public bool FadingEnable = false;
        public Skill.Framework.SmoothingParameters Fading;

        private AudioSource[] _Audios;
        private Skill.Framework.Smoothing _Volume;

        protected override void Awake()
        {
            base.Awake();
            _Volume.Reset(0);
            _Audios = GetComponents<AudioSource>();
            foreach (var audio in _Audios)
            {
                if (audio != null)
                {
                    audio.volume = _Volume.Current;
                }
            }
        }

        protected override void Update()
        {
            if (!Global.IsGamePaused)
            {
                if (Global.Instance != null)
                {
                    float volume = Global.Instance.Settings.Audio.GetVolume(Category) * VolumeFactor;
                    if (_Volume.Current != volume)
                    {
                        if (FadingEnable)
                        {
                            _Volume.Target = volume;
                            _Volume.Update(Fading);
                        }
                        else
                            _Volume.Reset(volume);
                        foreach (var audio in _Audios)
                        {
                            if (audio != null)
                                audio.volume = _Volume.Current;

                        }
                    }
                }
            }
            base.Update();
        }
    }
}
