using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Audio
{
    /// <summary>
    /// Helper class to play background music
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : DynamicBehaviour
    {        
        private float _FadeStartTime;
        private float _DestVolume;
        private float _DestVolume2;
        private float _FadeTime;
        private float _FadeTime2;
        private float _StartVolume;
        private AudioClip _NextAudioClip;
        private bool _IsFading;

        private const float EpsilonTime = 0.01f;

        /// <summary> Audio Source </summary>
        public AudioSource Audio { get; private set; }

        /// <summary> Last valid played audio clip </summary>
        public AudioClip LastClip { get; private set; }

        /// <summary> Get compoenet references </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            Audio = audio;
        }


        /// <summary>
        /// Play AudioClip next to current playing audio
        /// </summary>
        /// <param name="clip">Next AudioClip to play (can be null to stop after fadeout)</param>
        /// <param name="volume">Volume of next AudioClip</param>
        /// <param name="fadeInTime">How long time to fade from 0 to volume</param>
        /// <param name="fadeOutTime"> How long time to fade from current playing audio volume to 0 </param>
        public void Play(AudioClip clip, float volume, float fadeInTime, float fadeOutTime)
        {
            _FadeTime2 = fadeInTime;
            _DestVolume2 = volume;
            _NextAudioClip = clip;
            if (Audio.isPlaying && Audio.clip != null)
                FadeTo(0, fadeOutTime);
            else
                FadeTo(0, 0);
        }

        /// <summary>
        /// Fade current volume to specified volume
        /// </summary>
        /// <param name="volume">destination volume</param>
        /// <param name="fadeTime"> How long time to fade from current playing audio volume to destination volume </param>
        public void FadeTo(float volume, float fadeTime)
        {
            if (fadeTime < EpsilonTime) fadeTime = 0;
            _FadeTime = fadeTime;
            _DestVolume = volume;
            _FadeStartTime = Time.time;
            _StartVolume = Audio.volume;
            _IsFading = true;
        }


        /// <summary> Update </summary>
        protected override void Update()
        {
            if (Skill.Framework.Global.IsGamePaused) return;
            if (_IsFading)
            {
                float deltaTime = Time.time - _FadeStartTime;
                float factor;
                if (_FadeTime <= 0)
                    factor = 1;
                else
                    factor = deltaTime / _FadeTime;

                if (factor < 1)
                {
                    Audio.volume = Mathf.Lerp(_StartVolume, _DestVolume, deltaTime / _FadeTime);
                }
                else
                {
                    Audio.volume = _DestVolume;
                    _IsFading = false;
                    if (_NextAudioClip != null)
                    {
                        if (LastClip != _NextAudioClip)
                        {
                            LastClip = _NextAudioClip;
                            if (Audio.isPlaying)
                                Audio.Stop();
                            Audio.clip = _NextAudioClip;
                            if (Audio.clip != null)
                                Audio.Stop();
                            Audio.Play();
                        }
                        FadeTo(_DestVolume2, _FadeTime2);
                        _NextAudioClip = null;
                    }
                    else
                    {
                        if (Audio.isPlaying && Audio.volume <= 0.01f)
                            Audio.Stop();
                    }
                }
            }
            base.Update();
        }
    }
}
