using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.Sequence
{

    public class AnimationTrack : EventOrientedTrack
    {
        [HideInInspector]
        [SerializeField]
        public UnityEngine.Animation _Animation;

        [HideInInspector]
        [SerializeField]
        public string _DefaultAnimation = "Idle";

        [HideInInspector]
        [SerializeField]
        public bool _Preview = true;


        [ExposeProperty(0, "Animation")]
        public UnityEngine.Animation Animation { get { return _Animation; } set { _Animation = value; } }

        [ExposeProperty(1, "Default Animation", "Name of default animation")]
        public string DefaultAnimation { get { return _DefaultAnimation; } set { _DefaultAnimation = value; } }

        [ExposeProperty(2, "Preview", "Preview animations in editor")]
        public bool Preview { get { return _Preview; } set { _Preview = value; } }

        public override TrackType Type { get { return TrackType.Animation; } }

        /// <summary>
        /// Get maximum time of track
        /// </summary>
        public override float MaxTime
        {
            get
            {
                if (Keys != null && Keys.Length > 0)
                {
                    CrossFadeAnimationKey k = Keys[Keys.Length - 1] as CrossFadeAnimationKey;
                    if (k != null)
                    {
                        if (_Animation != null && !string.IsNullOrEmpty(k._Animation))
                        {
                            var state = _Animation[k._Animation];
                            if (state != null)
                                return k.FireTime + state.length;
                        }
                        return k.FireTime;
                    }
                }
                return base.MaxTime;
            }
        }

        public override void Evaluate(float time)
        {
            base.Evaluate(time);
            if (_Animation != null && (!Application.isPlaying && Preview))
            {
                Seek(time);
            }
        }

        private CrossFadeAnimationKey[] _LayerKeys;
        public override void Seek(float time)
        {
            base.Seek(time);
            if (_LayerKeys == null) _LayerKeys = new CrossFadeAnimationKey[10]; // maximum 10 layer support
            for (int i = 0; i < _LayerKeys.Length; i++) _LayerKeys[i] = null;

            if (_Animation != null && _Animation.enabled)
            {
                ResetAnimations();// reset weight of all used animations

                if (Keys != null && Keys.Length > 0 && Index >= 0)
                {
                    for (int i = 0; i <= Index; i++)
                    {
                        CrossFadeAnimationKey cfak = Keys[i] as CrossFadeAnimationKey;
                        if (cfak != null)
                        {
                            if (!string.IsNullOrEmpty(cfak._Animation))
                            {
                                AnimationState newState = _Animation[cfak._Animation];
                                if (newState != null)
                                {
                                    float stateWeight = 1.0f;
                                    if (cfak._Layer < _LayerKeys.Length)
                                    {
                                        if (_LayerKeys[cfak._Layer] != null)
                                        {
                                            AnimationState preState = _Animation[_LayerKeys[cfak._Layer]._Animation];
                                            if (preState != null)
                                            {
                                                if (time >= cfak.FireTime + cfak.FadeLenght)
                                                {
                                                    preState.weight = 0.0f;
                                                    preState.time = 0.0f;
                                                }
                                                else if (time >= cfak.FireTime)
                                                {
                                                    stateWeight = (time - cfak.FireTime) / cfak.FadeLenght;
                                                    preState.weight = 1.0f - stateWeight;
                                                    preState.time = time - _LayerKeys[cfak._Layer].FireTime;
                                                }
                                            }
                                        }
                                        _LayerKeys[cfak._Layer] = cfak;
                                    }

                                    cfak.ApplyStateParameters(newState);
                                    _Animation.CrossFade(newState.name, 0, cfak.Mode);
                                    newState.weight = stateWeight;
                                    newState.time = time - cfak.FireTime;
                                }
                            }
                        }
                    }

                }
                else if (!string.IsNullOrEmpty(_DefaultAnimation))
                {
                    AnimationState state = _Animation[_DefaultAnimation];

                    if (state != null)
                    {
                        _Animation.CrossFade(state.name, 0, PlayMode.StopAll);
                        state.time = time;
                        state.weight = 1.0f;
                    }
                }

                _Animation.Sample();
                // this is a stupid code to make unity update animation completely in SceneView
                if (!Application.isPlaying)
                {
                    _Animation.enabled = false;
                    _Animation.enabled = true;
                }
            }
        }

        private void ResetAnimations()
        {
            if (_Animation != null && Keys != null)
            {
                AnimationState aState = null;
                if (!string.IsNullOrEmpty(_DefaultAnimation))
                {
                    aState = _Animation[_DefaultAnimation];
                    if (aState != null)
                    {
                        aState.time = 0;
                        aState.weight = 0;
                    }
                }

                foreach (var k in Keys)
                {
                    CrossFadeAnimationKey cfak = k as CrossFadeAnimationKey;
                    if (cfak != null && !string.IsNullOrEmpty(cfak._Animation))
                    {
                        aState = _Animation[cfak._Animation];
                        if (aState != null)
                        {
                            aState.time = 0;
                            aState.weight = 0;
                        }
                    }
                }
            }
        }

        protected override void InitializeEvent(EventOrientedKey key)
        {
            AnimationKey aKey = (AnimationKey)key;
            aKey.AnimationComponent = this._Animation;
        }
    }


    #region AnimationKeys
    [System.Serializable]
    public abstract class AnimationKey : EventOrientedKey
    {
        public override bool IsSingleExecution { get { return true; } }
        internal UnityEngine.Animation AnimationComponent { get; set; }
    }


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
    #endregion

}
