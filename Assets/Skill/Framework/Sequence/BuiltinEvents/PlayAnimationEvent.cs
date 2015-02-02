using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Fades the animation with name animation in over a period of time seconds and fades other animations out.
    /// </summary>
    [CustomEvent("Play Animation", "Animation")]
    public class PlayAnimationEvent : EventKey
    {
        [SerializeField]
        private UnityEngine.Animation _Animation;
        [SerializeField]
        private string _AnimationName = "Idle";
        [SerializeField]
        private float _FadeLength = 0.3f;
        [SerializeField]
        private PlayMode _PlayMode = PlayMode.StopAll;

        [ExposeProperty(101, "Animation", "Animation component")]
        public UnityEngine.Animation Animation { get { return _Animation; } set { _Animation = value; } }
        [ExposeProperty(102, "Animation Name", "Animation clip name")]
        public string AnimationName { get { return _AnimationName; } set { _AnimationName = value; } }
        [ExposeProperty(103, "Fade Length", "")]
        public float FadeLength { get { return _FadeLength; } set { _FadeLength = value; } }
        [ExposeProperty(104, "PlayMode", "")]
        public PlayMode PlayMode { get { return _PlayMode; } set { _PlayMode = value; } }

        public override void FireEvent()
        {
            if (Animation != null && !string.IsNullOrEmpty(AnimationName))
                Animation.CrossFade(AnimationName, _FadeLength, _PlayMode);
            else
                Debug.LogWarning("Invalid PlayAnimationEvent data");
        }
    }
}