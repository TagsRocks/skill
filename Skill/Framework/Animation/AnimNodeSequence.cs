using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// This animation node outputs the animation data within an animation sequence.
    /// </summary>
    public class AnimNodeSequence : AnimNode
    {
        /// <summary>
        /// by default skill generates warnings for miss animations. set it true to disable warnings.
        /// </summary>
        public static bool IgnoreMissAnimationWarning { get; set; }

        private class AnimationInfo
        {
            public string Name;
            public float Length;
        }

        /// <summary>
        /// whether animation layer needs to update PreviousAnimation. when AnimationTree profile changed
        /// </summary>
        internal bool UpdatePreviousAnimation;
        /// <summary>
        /// Previous AnimationName
        /// </summary>
        internal string PreviousAnimation { get; private set; }

        private string _AnimationName;
        /// <summary>
        /// Name of AnimationClip
        /// </summary>
        public string AnimationName { get { return _AnimationName; } set { _AnimationName = value; UpdateAnimation(); } }

        private string _Format;
        /// <summary>
        /// Format of AnimationName. used in AnimationTree profile
        /// </summary>
        public string Format { get { return _Format; } set { _Format = value; UpdateAnimation(); } }

        /// <summary>
        /// Current Animation Name
        /// </summary>
        public string CurrentAnimation { get; private set; }

        /// <summary>
        /// Whether use AnimationTree profiling method? (default is true)
        /// </summary>
        public bool UseTreeProfile { get; set; }

        /// <summary> MixingTransforms </summary>
        public string[] MixingTransforms { get; set; }

        /// <summary>
        /// Synchronize animations with other animations in same Layer?
        /// </summary>
        /// <remarks>
        /// it can be used for Direction AnimationClips that has same length.
        /// </remarks>
        public bool Synchronize { get; set; }

        /// <summary>
        /// Specify when this animation bacame relevant, and when needs to finish
        /// </summary>
        public TimeWatch RelevantTime;

        /// <summary>
        /// Update AnimationName and Format and check whether it needs to update previous animation
        /// </summary>
        private void UpdateAnimation()
        {
            if (UseTreeProfile)
            {
                PreviousAnimation = CurrentAnimation;

                if (!string.IsNullOrEmpty(_Format) && !string.IsNullOrEmpty(_AnimationName))
                {
                    CurrentAnimation = string.Format(_Format, _AnimationName);
                }
                else if (!string.IsNullOrEmpty(_AnimationName))
                {
                    CurrentAnimation = _AnimationName;
                }
                else
                    CurrentAnimation = "";

                if (PreviousAnimation != CurrentAnimation)
                    UpdatePreviousAnimation = true;
                else
                    UpdatePreviousAnimation = false;

                var info = FindInfo(CurrentAnimation);
                if (info != null) _Length = info.Length;
            }
            else
            {
                CurrentAnimation = _AnimationName;
                UpdatePreviousAnimation = false;
            }
        }

        /// <summary>
        /// Speed at which the animation will be played back. Default is 1.0
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// WrapMode
        /// </summary>
        public UnityEngine.WrapMode WrapMode { get; set; }

        /// <summary>
        /// Layer
        /// </summary>
        public AnimationLayer Layer { get; private set; }

        private float _Length;
        /// <summary>
        /// Lenght of AnimationClip based on speed
        /// </summary>
        public override float Length
        {
            get
            {
                if (Speed > 0)
                    return _Length / Speed;
                return 0;
            }
        }

        /// <summary> RootMotion data </summary>
        public RootMotion RootMotion { get; private set; }

        /// <summary>
        /// Create an instance of AnimNodeSequence
        /// </summary>
        internal AnimNodeSequence()
            : this("")
        {
        }

        /// <summary>
        /// Create an instance of AnimNodeSequence
        /// </summary>
        /// <param name="animationName">Name of AnimationClip</param>
        public AnimNodeSequence(string animationName)
            : base(0)
        {
            this.RootMotion = new Animation.RootMotion(this);
            this._InitializedAnimations = new List<AnimationInfo>();
            this.Synchronize = false;
            this.UseTreeProfile = true;
            this.Speed = 1;
            this.WrapMode = UnityEngine.WrapMode.Default;
            this.AnimationName = animationName;
        }

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnBecameRelevant(AnimationTreeState state)
        {
            RelevantTime.Begin(Length);
            RootMotion.Begin();
            base.OnBecameRelevant(state);
        }
        /// <summary>
        /// call CeaseRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnCeaseRelevant(AnimationTreeState state)
        {
            RelevantTime.End();
            base.OnCeaseRelevant(state);
        }

        /// <summary>
        /// update weight
        /// </summary>
        protected override void Blend()
        {
            if (this.RootMotion.State.IsEnable)
                this.RootMotion.Evaluate();
            if (!string.IsNullOrEmpty(AnimationName)) Layer.UpdateAnimation(this);
        }
        /// <summary>
        /// Allow each node to get apropriate AnimationLayer
        /// </summary>
        /// <param name="manager">LayerManager to create layer</param>
        /// <param name="parentSuggestLayer">AnimationLayer suggested by parent. (layer of child at index 0)</param>
        public override void SelectLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            this.Layer = parentSuggestLayer;
        }

        /// <summary>
        /// Set format of aimation name
        /// </summary>
        /// <param name="format"></param>
        internal override void SetFormat(string format)
        {
            this.Format = format;
        }

        private List<AnimationInfo> _InitializedAnimations;
        private AnimationInfo FindInfo(string name)
        {
            foreach (var item in _InitializedAnimations)
            {
                if (item.Name == name) return item;
            }
            return null;
        }

        /// <summary>
        /// Initialize and collect information from animationComponent
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation</param>
        public override void Initialize(UnityEngine.Animation animationComponent)
        {
            if (string.IsNullOrEmpty(CurrentAnimation))
            {
                UnityEngine.Debug.LogWarning("Please set valid 'AnimationName' to  AnimNodeSequence : " + (string.IsNullOrEmpty(Name) ? "" : CurrentAnimation));
                return;
            }

            if (FindInfo(CurrentAnimation) != null) return;

            UnityEngine.AnimationState state = animationComponent[CurrentAnimation];
            if (state != null)
            {
                _InitializedAnimations.Add(new AnimationInfo() { Name = CurrentAnimation, Length = state.length });
                this._Length = state.length;
                state.layer = Layer.LayerIndex;

                if (MixingTransforms != null)
                {
                    foreach (var item in MixingTransforms)
                    {
                        if (string.IsNullOrEmpty(item))
                            UnityEngine.Debug.LogWarning("Empty MixingTransform");
                        else
                        {
                            UnityEngine.Transform tr = animationComponent.transform.Find(item);
                            if (tr != null)
                                state.AddMixingTransform(tr);
                            else
                                UnityEngine.Debug.LogWarning("Invalid MixingTransform : " + item);
                        }
                    }
                }

            }
            else if (!IgnoreMissAnimationWarning)
                UnityEngine.Debug.LogWarning("Can not find AnimationClip : " + CurrentAnimation);
        }
    }
}
