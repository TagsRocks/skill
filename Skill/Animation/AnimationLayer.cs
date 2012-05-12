using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Manage animation blending of single layer
    /// </summary>
    public sealed class AnimationLayer
    {
        private AnimNodeSequence _MaxReleavant;

        /// <summary>
        /// Synchronize animations?
        /// </summary>
        public bool Synchronization { get; set; }

        /// <summary>
        /// include AnimNodes with weight > 0
        /// </summary>
        public List<AnimNodeSequence> ActiveAnimNodes { get; private set; }
        /// <summary>
        /// Index of layer. (begin by 0)
        /// </summary>
        public int LayerIndex { get; private set; }
        /// <summary>
        /// AnimationBlendMode. (Blend or Additive)
        /// </summary>
        public UnityEngine.AnimationBlendMode BlendMode { get; private set; }

        /// <summary>
        /// Create an instance of AnimationLayer
        /// </summary>
        /// <param name="layerIndex">Index of layer</param>
        /// <param name="blendMode">AnimationBlendMode</param>
        public AnimationLayer(int layerIndex, UnityEngine.AnimationBlendMode blendMode)
        {
            this.BlendMode = blendMode;
            this.LayerIndex = layerIndex;
            ActiveAnimNodes = new List<AnimNodeSequence>();
        }

        /// <summary>
        /// Update Layer
        /// </summary>
        internal void Update()
        {
            if (Synchronization)
            {
                _MaxReleavant = null;
                float maxW = 0;
                foreach (var node in ActiveAnimNodes)
                {
                    if (node.Weight > maxW)
                    {
                        _MaxReleavant = node;
                        maxW = node.Weight;
                    }
                }
            }
        }

        /// <summary>
        /// Make sure given AnimNodeSequence will update at next update
        /// </summary>
        /// <param name="anim">AnimNodeSequence to update</param>
        internal void UpdateAnimation(AnimNodeSequence anim)
        {
            AddToActiveList(anim);
        }

        /// <summary>
        /// Register given AnimNodeSequence to process next update
        /// </summary>
        /// <param name="anim"></param>
        private void AddToActiveList(AnimNodeSequence anim)
        {
            if (anim == null) return;
            foreach (var item in ActiveAnimNodes)
            {
                if (item == anim) return;
            }
            ActiveAnimNodes.Add(anim);
        }

        /// <summary>
        /// Remove AnimNodeSequences with weight == 0
        /// </summary>
        internal void CleanUpActiveList()
        {
            int i = 0;
            while (i < ActiveAnimNodes.Count)
            {
                if (ActiveAnimNodes[i].Weight == 0.0f)
                {
                    ActiveAnimNodes.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }


        //private void Blend()
        //{
        //    if (Current != null)
        //    {
        //        if (Previous != null)
        //        {
        //            float currLength = Current.Length;
        //            float preLength = Previous.Length;

        //            float totalW = Current.Weight + Previous.Weight + 1e-6f;
        //            float preP = Previous.Weight / totalW;
        //            float currP = Current.Weight / totalW;
        //            float ratio = currLength / preLength;
        //            Current.IntermediateSpeed = (1.0f - currP) * ratio + currP;
        //            Previous.IntermediateSpeed = (1.0f - preP) * (1.0f / ratio) + preP;

        //            if (Current.IntermediateSpeed > Current.Speed)
        //                Current.IntermediateSpeed = Current.Speed;

        //            if (Previous.Weight <= 0)
        //            {
        //                AddToActiveList(Previous);
        //                Previous = null;
        //            }
        //        }
        //        else
        //        {
        //            Current.IntermediateSpeed = Current.Speed;
        //        }
        //    }
        //}


        /// <summary>
        /// Apply changes to UnityEngine.Animation component 
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation to apply changes to</param>
        internal void Apply(UnityEngine.Animation animationComponent)
        {
            float normalizedTime = 0;

            if (Synchronization && _MaxReleavant != null)
            {
                UnityEngine.AnimationState state = animationComponent[_MaxReleavant.CurrentAnimation];
                if (state != null)
                    normalizedTime = state.normalizedTime;
            }


            foreach (var anim in ActiveAnimNodes)// iterate throw all active AnimNodeSequences
            {
                UnityEngine.AnimationState state = animationComponent[anim.CurrentAnimation];// access state
                if (state != null)
                {
                    if ((anim.WrapMode == UnityEngine.WrapMode.Once || anim.WrapMode == UnityEngine.WrapMode.ClampForever))
                    {
                        if (anim.IsJustBecameRelevant)// if AnimNodeSequence is already enable and was disabled previous frame
                            state.normalizedTime = 0;// force to start from beginning
                        if (anim.RelevantTime.Enabled && anim.RelevantTime.IsOver)// if reach end of animation : stop it
                        {
                            if (anim.WrapMode == UnityEngine.WrapMode.Once)
                                state.normalizedTime = 0; // stop at first frame
                            else
                                state.normalizedTime = 1; // stop at last frame
                            state.speed = 0;// freeze animation
                        }
                        else // else continue
                            state.speed = anim.Speed;
                    }
                    else // else continue
                    {
                        state.speed = anim.Speed;
                        if (anim.IsJustBecameRelevant)
                        {
                            if (Synchronization && anim.Synchronize && _MaxReleavant != null && _MaxReleavant != anim)
                            {
                                state.normalizedTime = normalizedTime;
                            }
                            else
                            {
                                state.normalizedTime = 0;
                            }
                        }
                    }

                    if (anim.WeightChange != WeightChangeMode.NoChange)
                    {
                        // set parameters
                        state.blendMode = BlendMode;
                        state.wrapMode = anim.WrapMode;
                        state.layer = LayerIndex;
                        state.weight = anim.Weight;

                        // disable or enable animation
                        if (anim.Weight == 0)
                            state.enabled = false;
                        else
                            state.enabled = true;
                    }
                }
                // if profile changed in previous frame                
                if (anim.UpdatePreviousAnimation)
                {
                    if (anim.PreviousAnimation != null)
                    {
                        UnityEngine.AnimationState preState = animationComponent[anim.PreviousAnimation];
                        if (preState != null)
                            animationComponent.Blend(anim.PreviousAnimation, 0, 0.3f);
                    }
                    anim.UpdatePreviousAnimation = false;
                }
            }
        }
    }
}
