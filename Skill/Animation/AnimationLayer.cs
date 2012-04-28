using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public enum LayerMode
    {
        CrossFade,
        BlendAll,
        Additive
    }

    public sealed class AnimationLayer
    {
        private bool _CrossFade;
        public List<AnimationSequence> ActiveAnimations { get; private set; }
        public int LayerIndex { get; private set; }
        public LayerMode Mode { get; private set; }

        public AnimationLayer(int layerIndex, LayerMode mode)
        {
            this.Mode = mode;
            this.LayerIndex = layerIndex;
            ActiveAnimations = new List<AnimationSequence>();
        }

        public void Update()
        {
        }

        internal void UpdateAnimation(AnimationSequence anim)
        {
            AddToActiveList(anim);
        }

        private void AddToActiveList(AnimationSequence anim)
        {
            if (anim == null) return;
            foreach (var item in ActiveAnimations)
            {
                if (item == anim) return;
            }
            ActiveAnimations.Add(anim);
        }

        internal void CleanUpActiveList()
        {
            int i = 0;
            while (i < ActiveAnimations.Count)
            {
                if (ActiveAnimations[i].Weight == 0.0f)
                {
                    ActiveAnimations.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }


        internal void Apply(UnityEngine.Animation animationComponent)
        {

            foreach (var anim in ActiveAnimations)
            {
                UnityEngine.AnimationState state = animationComponent[anim.CurrentAnimation];
                if (state != null)
                {
                    if (Mode == LayerMode.Additive)
                        state.blendMode = UnityEngine.AnimationBlendMode.Additive;
                    state.wrapMode = anim.WrapMode;
                    state.layer = LayerIndex;
                    state.speed = anim.Speed;
                    if (anim.WrapMode == UnityEngine.WrapMode.Once || anim.WrapMode == UnityEngine.WrapMode.Clamp || anim.WrapMode == UnityEngine.WrapMode.ClampForever)
                    {
                        if (anim.IsJustBecameRelevant)
                            state.normalizedTime = 0;
                        if (anim.Timer.IsOver)
                        {
                            continue;
                        }
                    }

                    if (anim.WeightChange != WeightChangeMode.NoChange)
                        animationComponent.Blend(anim.CurrentAnimation, anim.Weight, anim.BlendTime);

                    if (anim.UpdatePreviousAnimation)
                    {
                        if (anim.PreviousAnimation != null)
                            animationComponent.Blend(anim.PreviousAnimation, 0, anim.BlendTime);
                        anim.UpdatePreviousAnimation = false;
                    }
                }
            }
        }
    }
}
