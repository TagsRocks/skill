using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Sound key data
    /// </summary>
    [System.Serializable]
    public class SoundKey : EventOrientedKey
    {
        /// <summary> Clip to play </summary>
        public AudioClip Clip;
        /// <summary> Volume factor </summary>   
        public float VolumeFactor = 1.0f;

        [ExposeProperty(0, "Play Time", "Time to play audio")]
        public override float FireTime { get { return base.FireTime; } set { base.FireTime = value; } }

        [ExposeProperty(1, "Audio Clip")]
        public AudioClip ExClip { get { return Clip; } set { Clip = value; } }

        [ExposeProperty(2, "Volume Factor")]
        public float ExVolumeFactor { get { return VolumeFactor; } set { VolumeFactor = value; } }

        public override float Length
        {
            get
            {
                if (Clip != null) return Clip.length;
                else return 0;
            }
        }


        public override void ExecuteEvent(EventOrientedTrack track)
        {
            SoundTrack st = (SoundTrack)track;
            if (st.Source != null && this.Clip != null)
            {
                float volume = 1.0f;
                if (Skill.Framework.Global.Instance != null)
                    volume = Skill.Framework.Global.Instance.Settings.Audio.GetVolume(st.Category);
                if (st.PlayOneShot)
                {
                    st.Source.PlayOneShot(this.Clip, volume * this.VolumeFactor);
                }
                else
                {
                    if (st.Source.isPlaying) st.Source.Stop();
                    st.Source.clip = this.Clip;
                    st.Source.Play();
                }
                if (Audio.AudioSubtitle.Instance != null)
                    Audio.AudioSubtitle.Instance.Show(this.Clip);
            }
        }
    }

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

        public override void Stop()
        {
            if (Source != null)
                Source.Stop();
            Source.clip = null;
        }
    }
}