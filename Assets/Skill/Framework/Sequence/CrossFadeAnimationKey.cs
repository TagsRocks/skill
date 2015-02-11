using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{

    /// <summary>
    /// Fades the animation with name animation in over a period of time seconds and fades other animations out.
    /// </summary>
    [System.Serializable]
    public class CrossFadeAnimationKey : AnimationKey
    {
        public string _Animation = "Idle";
        public int _Layer = 0;
        public float _FadeLenght = 0.3f;
        public PlayMode _Mode = PlayMode.StopAll;
        public float _Speed = 1.0f;
        public AnimationBlendMode _BlendMode = AnimationBlendMode.Blend;
        public WrapMode _WrapMode = WrapMode.Once;


        [ExposeProperty(2, "Animation", "Animation to crossfade")]
        public string Animation { get { return this._Animation; } set { this._Animation = value; } }
        [ExposeProperty(3, "Layer", "Layer of animation")]
        public int Layer { get { return this._Layer; } set { this._Layer = value; } }
        [ExposeProperty(4, "Fade Lenght", "")]
        public float FadeLenght { get { return this._FadeLenght; } set { this._FadeLenght = value; } }
        [ExposeProperty(5, "Play Mode", "")]
        public PlayMode Mode { get { return this._Mode; } set { this._Mode = value; } }
        [ExposeProperty(6, "Speed", "The playback speed of the animation. 1 is normal playback speed.")]
        public float Speed { get { return this._Speed; } set { this._Speed = value; } }
        [ExposeProperty(7, "BlendMode", "Which blend mode should be used?")]
        public AnimationBlendMode BlendMode { get { return this._BlendMode; } set { this._BlendMode = value; } }
        [ExposeProperty(8, "WrapMode", "Wrapping mode of the animation.")]
        public WrapMode WrapMode { get { return this._WrapMode; } set { this._WrapMode = value; } }

        public override void FireEvent()
        {
            if (AnimationComponent != null && !string.IsNullOrEmpty(_Animation))
            {
                var state = AnimationComponent[_Animation];
                if (state != null)
                {
                    ApplyStateParameters(state);
                    AnimationComponent.CrossFade(_Animation, _FadeLenght, _Mode);
                }
            }
        }

        public void ApplyStateParameters(UnityEngine.AnimationState state)
        {
            if (state != null)
            {
                state.layer = _Layer;
                state.speed = _Speed;
                state.blendMode = _BlendMode;
                state.wrapMode = _WrapMode;
            }
        }
    }

}