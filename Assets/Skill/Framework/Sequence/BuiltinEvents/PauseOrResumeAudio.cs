using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// This event will allow you to Pause or Resume playback on an already playing AudioClip.
    /// </summary>
    [CustomEvent("PauseOrResume", "Audio")]
    public class PauseOrResumeAudio : EventKey
    {

        [SerializeField]
        private AudioSource _Source;

        [SerializeField]
        private bool _Pause;


        [ExposeProperty(101, "Source", "Audio Source")]
        public AudioSource Source { get { return _Source; } set { _Source = value; } }

        [ExposeProperty(102, "Pause", "Are we pausing or resuming this Audio Clip")]
        public bool Pause { get { return _Pause; } set { _Pause = value; } }

        public override void FireEvent()
        {
            if (_Source != null)
            {
                if (_Pause)
                    _Source.Pause();
                else
                    _Source.Play();
            }
            else
                Debug.LogWarning("Invalid AudioSource for PauseOrResume event");
        }


    }
}