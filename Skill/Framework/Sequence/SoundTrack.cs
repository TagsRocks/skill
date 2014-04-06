using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Sound key data
    /// </summary>
    [System.Serializable]
    public class SoundKey : IPropertyKey<AudioClip>, ITrackKey
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Clip to play </summary>
        public AudioClip Clip;
        /// <summary> Volume factor </summary>   
        public float VolumeFactor = 1.0f;


        [ExposeProperty(0, "Time", "play time")]
        public float ExTime { get { return Time; } set { Time = value; } }
        [ExposeProperty(2, "Volume Factor")]
        public float ExVolumeFactor { get { return VolumeFactor; } set { VolumeFactor = value; } }
        [ExposeProperty(1, "Audio clip")]
        public AudioClip ExClip { get { return Clip; } set { Clip = value; } }


        /// <summary> Execution Time </summary>
        public float FireTime { get { return Time; } set { Time = value; if (Time < 0) Time = 0; } }

        public AudioClip ValueKey { get { return Clip; } set { Clip = value; } }
    }

    public class SoundTrack : PropertyTrack<AudioClip>
    {
        [HideInInspector]
        [SerializeField]
        public SoundKey[] Keys;

        [HideInInspector]
        [SerializeField]
        public AudioSource Source;

        [HideInInspector]
        [SerializeField]
        public Skill.Framework.Sounds.SoundCategory Category = Skill.Framework.Sounds.SoundCategory.Cinematic;

        [ExposeProperty(0, "Source", "Source AudioSource")]
        public AudioSource ExSource { get { return Source; } set { Source = value; } }

        [ExposeProperty(1, "Category", "Sound Category")]
        public Skill.Framework.Sounds.SoundCategory ExCategory { get { return Category; } set { Category = value; } }

        // to avoid show in PropertyGrid
        public new AudioClip ExDefaultValue { get; set; }


        public override TrackType Type { get { return TrackType.Sound; } }
        public override Type PropertyType { get { return typeof(AudioClip); } }
        public override IPropertyKey<AudioClip>[] PropertyKeys { get { return Keys; } set { Keys = (SoundKey[])value; } }


        protected override void Execute(IPropertyKey<AudioClip> key)
        {
            SoundKey sKey = (SoundKey)key;
            if (Source != null && sKey.ValueKey != null)
            {
                float volume = 1.0f;
                if (Skill.Framework.Global.Instance != null)
                    volume = Skill.Framework.Global.Instance.Settings.Audio.GetVolume(Category);
                Source.PlayOneShot(sKey.ValueKey, volume * sKey.VolumeFactor);
            }
        }

        protected override void Evaluate(IPropertyKey<AudioClip> key) { /* nothing */ }
    }
}