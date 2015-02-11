using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{   
    public class SoundTrack : EventOrientedTrack
    {
        [HideInInspector]
        [SerializeField]
        public AudioSource Source;

        [HideInInspector]
        [SerializeField]
        public Skill.Framework.Audio.SoundCategory Category = Skill.Framework.Audio.SoundCategory.Cinematic;

        [HideInInspector]
        [SerializeField]
        public bool PlayOneShot = false;


        [ExposeProperty(0, "Source", "Source AudioSource")]
        public AudioSource ExSource { get { return Source; } set { Source = value; } }

        [ExposeProperty(1, "Category", "Sound Category")]
        public Skill.Framework.Audio.SoundCategory ExCategory { get { return Category; } set { Category = value; } }

        [ExposeProperty(2, "PlayOneShot", "user PlayOneShot method")]
        public bool ExPlayOneShot { get { return PlayOneShot; } set { PlayOneShot = value; } }

        public override TrackType Type { get { return TrackType.Sound; } }


        /// <summary>
        /// Get maximum time of track
        /// </summary>
        public override float MaxTime
        {
            get
            {
                if (Keys != null && Keys.Length > 0)
                {
                    SoundKey k = Keys[Keys.Length - 1] as SoundKey;
                    if (k != null)
                    {
                        if (k.Clip != null)
                            return k.FireTime + k.Clip.length;
                        else
                            return k.FireTime;
                    }
                }
                return base.MaxTime;
            }
        }

        public override void Stop()
        {
            if (Source != null)
                Source.Stop();
            Source.clip = null;
        }

        private bool _IsPaused;
        public override void Pause()
        {
            if (Source != null && Source.isPlaying)
            {
                _IsPaused = true;
                Source.Pause();
            }
            base.Pause();
        }

        public override void Resume()
        {
            if (_IsPaused)
            {
                _IsPaused = false;
                Source.Play();
            }
            base.Resume();
        }

        protected override void InitializeEvent(EventOrientedKey key)
        {
            SoundKey sKey = (SoundKey)key;
            sKey.Source = this.Source;
            sKey.PlayOneShot = this.PlayOneShot;
            sKey.Category = this.Category;
        }
    }
}