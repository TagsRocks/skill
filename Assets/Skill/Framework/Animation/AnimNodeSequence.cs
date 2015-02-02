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

        /// <summary>
        /// whether animation layer needs to update PreviousAnimation. when AnimationTree profile changed
        /// </summary>
        internal bool UpdatePreviousAnimation;
        /// <summary>
        /// Previous AnimationName
        /// </summary>
        internal string PreviousAnimation { get; private set; }

        private UnityEngine.Animation _Animation;
        private UnityEngine.AnimationState _State;

        public UnityEngine.AnimationState State { get { return _State; } }

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

            if (_Animation != null)
            {
                _State = _Animation[CurrentAnimation];
                if (_State != null)
                {
                    _Length = _State.length;
                    _State.layer = Layer.LayerIndex;
                }
                else
                {
                    if (!IgnoreMissAnimationWarning)
                        UnityEngine.Debug.LogWarning("Please set valid 'AnimationName' to  AnimNodeSequence : " + (string.IsNullOrEmpty(Name) ? "" : CurrentAnimation));
                }
            }
        }

        /// <summary>
        /// Speed at which the animation will be played back. Default is 1.0
        /// </summary>
        public float Speed { get; set; }

        /// <summary> Sync animation with parent blendnode relevant time </summary>
        public bool Sync { get; set; }

        /// <summary>
        /// WrapMode
        /// </summary>
        public UnityEngine.WrapMode WrapMode { get; set; }

        /// <summary>Layer </summary>
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
            this.UseTreeProfile = true;
            this.Speed = 1;
            this.WrapMode = UnityEngine.WrapMode.Default;
            this.AnimationName = animationName;
            this.Sync = false;
        }

        /// <summary>
        /// call BecameRelevant event
        /// </summary>
        /// <param name="state">State of AnimationTree</param>
        protected override void OnBecameRelevant()
        {
            base.OnBecameRelevant();
            if (Sync && Parent != null)
                _State.time = UnityEngine.Mathf.Repeat((UnityEngine.Time.time - Parent.RelevantTime), _State.length);
            else
                _State.time = 0;
        }

        /// <summary>
        /// update weight
        /// </summary>
        protected override void Blend()
        {
            if (this.RootMotion.HasPosition || this.RootMotion.HasRotation)
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


        /// <summary>
        /// Initialize and collect information from animationComponent
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation</param>
        public override void Initialize(UnityEngine.Animation animationComponent)
        {
            _Animation = animationComponent;
            _State = _Animation[CurrentAnimation];
            if (_State != null)
            {
                _Length = _State.length;
                _State.layer = Layer.LayerIndex;
            }
            else
            {
                if (!IgnoreMissAnimationWarning)
                    UnityEngine.Debug.LogWarning("Please set valid 'AnimationName' to  AnimNodeSequence : " + (string.IsNullOrEmpty(Name) ? "" : CurrentAnimation));
            }
        }
    }
}
