using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    ///  Add this Event when you would like to stop the playback of audio.
    /// </summary>
    [CustomEvent("Stop", "Audio")]
    public class StopAudio : EventKey
    {
        [SerializeField]
        private AudioSource _Source;


        [ExposeProperty(101, "Source", "Audio Source")]
        public AudioSource Source { get { return _Source; } set { _Source = value; } }

        public override void FireEvent()
        {
            if (_Source != null)
                _Source.Stop();
            else
                Debug.LogWarning("Invalid AudioSource for StopAudio event");
        }

    }
}