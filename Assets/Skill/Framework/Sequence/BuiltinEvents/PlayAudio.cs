using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you want to play an Audio Clip
    /// </summary>
    [CustomEvent("Play", "Audio")]
    public class PlayAudio : EventKey
    {

        [SerializeField]
        private AudioSource _Source;

        [SerializeField]
        private AudioClip _Clip;

        [SerializeField]
        private bool _Loop;


        [ExposeProperty(101, "Source", "Audio Source")]
        public AudioSource Source { get { return _Source; } set { _Source = value; } }

        [ExposeProperty(102, "Clip", "The Audio Clip to play.")]
        public AudioClip Clip { get { return _Clip; } set { _Clip = value; } }

        [ExposeProperty(103, "Loop", "Should we loop this Audio Clip.")]
        public bool Loop { get { return _Loop; } set { _Loop = value; } }

        public override void FireEvent()
        {
            if (_Source != null && Clip != null)
            {
                if (_Source.isPlaying)
                    _Source.Stop();
                _Source.clip = _Clip;
                _Source.loop = _Loop;
                _Source.Play();
            }
            else
                Debug.LogWarning("Invalid AudioSource and AudioClip for PlayAudio event");
        }


    }
}