using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationSequence : AnimationNode
    {        
        internal bool UpdatePreviousAnimation;
        internal string PreviousAnimation { get; private set; }

        private string _AnimationName;
        public string AnimationName { get { return _AnimationName; } set { _AnimationName = value; UpdateAnimation(); } }

        private string _Format;
        public string Format { get { return _Format; } set { _Format = value; UpdateAnimation(); } }

        public string CurrentAnimation { get; private set; }

        public bool UseProfile { get; set; }

        private void UpdateAnimation()
        {
            if (UseProfile)
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

        // Speed at which the animation will be played back.Default is 1.0
        public float Speed { get; set; }

        public UnityEngine.WrapMode WrapMode { get; set; }

        public AnimationLayer Layer { get; private set; }

        private float _Length;
        public override float Length { get { return _Length; } }

        internal AnimationSequence()
            : this("")
        {            
        }
        public AnimationSequence(string animationName)
            : base(0)
        {
            UseProfile = true;
            Speed = 1;
            WrapMode = UnityEngine.WrapMode.Default;
            AnimationName = animationName;            
        }

        protected override void OnBecameRelevant()
        {
            base.OnBecameRelevant();
        }

        protected override void Updating()
        {
            if (!string.IsNullOrEmpty(AnimationName)) Layer.UpdateAnimation(this);
        }

        public override void SetLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            this.Layer = parentSuggestLayer;
        }

        public override void CollectInfo(UnityEngine.Animation animationComponent)
        {
            if (string.IsNullOrEmpty(AnimationName)) return;
            UnityEngine.AnimationState state = animationComponent[AnimationName];
            if (state != null)
            {
                this._Length = state.length;
                state.layer = Layer.LayerIndex;
            }
        }
    }
}
