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

        /// <summary> Loop sound </summary>   
        public bool Loop = false;

        /// <summary> Visualize sound data in Matinee Editor </summary>   
        public bool Visualize = false;

        [ExposeProperty(0, "Play Time", "Time to play audio")]
        public override float FireTime { get { return base.FireTime; } set { base.FireTime = value; } }

        [ExposeProperty(1, "Audio Clip")]
        public AudioClip ExClip { get { return Clip; } set { Clip = value; } }

        [ExposeProperty(2, "Volume Factor")]
        public float ExVolumeFactor { get { return VolumeFactor; } set { VolumeFactor = value; } }

        [ExposeProperty(3, "Loop")]
        public bool ExLoop { get { return Loop; } set { Loop = value; } }

        [ExposeProperty(4, "Visualize", "Visualize sound data")]
        public bool ExVisualize { get { return Visualize; } set { Visualize = value; } }

        public override float Length
        {
            get
            {
                if (Clip != null) return Clip.length;
                else return 0;
            }
        }

        public override bool IsSingleExecution { get { return true; } }

        public override void FireEvent()
        {
            if (Source != null)
            {

                float volume = 1.0f;
                if (Skill.Framework.Global.Instance != null)
                    volume = Skill.Framework.Global.Instance.Settings.Audio.GetVolume(Category);
                if (PlayOneShot)
                {
                    if (this.Clip != null)
                        Source.PlayOneShot(this.Clip, volume * this.VolumeFactor);
                }
                else
                {
                    if (Source.isPlaying)
                        Source.Stop();
                    Source.volume = volume * this.VolumeFactor;
                    Source.clip = this.Clip;
                    if (this.Clip != null)
                        Source.Play();
                }
                Source.loop = this.Loop;
                if (this.Clip != null)
                {
                    if (Audio.AudioSubtitle.Instance != null)
                        Audio.AudioSubtitle.Instance.Show(this.Clip);
                }
            }
        }

        internal AudioSource Source { get; set; }
        internal bool PlayOneShot { get; set; }
        internal Skill.Framework.Audio.SoundCategory Category { get; set; }
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

        protected override void InitializeEvent(EventOrientedKey key)
        {
            SoundKey sKey = (SoundKey)key;
            sKey.Source = this.Source;
            sKey.PlayOneShot = this.PlayOneShot;
            sKey.Category = this.Category;
        }
    }
}