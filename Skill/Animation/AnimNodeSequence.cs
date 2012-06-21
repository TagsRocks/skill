using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// This animation node outputs the animation data within an animation sequence.
    /// </summary>
    public class AnimNodeSequence : AnimNode
    {
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
            Synchronize = false;
            UseTreeProfile = true;
            Speed = 1;
            WrapMode = UnityEngine.WrapMode.Default;
            AnimationName = animationName;
        }

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnBecameRelevant(AnimationTreeState state)
        {
            RelevantTime.Begin(Length);
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
        /// Initialize and collect information from animationComponent
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation</param>
        public override void Initialize(UnityEngine.Animation animationComponent)
        {
            if (string.IsNullOrEmpty(AnimationName))
            {
                UnityEngine.Debug.LogWarning("Please set valid 'AnimationName' to  AnimNodeSequence : " + (string.IsNullOrEmpty(Name) ? "" : Name));
                return;
            }

            UnityEngine.AnimationState state = animationComponent[AnimationName];
            if (state != null)
            {
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
            else
                UnityEngine.Debug.LogWarning("Can not find AnimationClip : " + AnimationName);
        }
    }
}
