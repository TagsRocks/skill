using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// Manage animation blending of single layer
    /// </summary>
    public sealed class AnimationLayer
    {
        private UnityEngine.Vector3 _DeltaPosition;
        private UnityEngine.Vector3 _DeltaRotation;

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

        /// <summary> delta position  of Sequences in this layer </summary>
        public UnityEngine.Vector3 DeltaPosition
        {
            get
            {
                if (float.IsNaN(_DeltaPosition.x)) _DeltaPosition.x = 0;
                if (float.IsNaN(_DeltaPosition.y)) _DeltaPosition.y = 0;
                if (float.IsNaN(_DeltaPosition.z)) _DeltaPosition.z = 0;
                return _DeltaPosition;
            }
        }

        /// <summary> delta rotation result of Sequences in this layer </summary>
        public UnityEngine.Vector3 DeltaRotation
        {
            get
            {
                if (float.IsNaN(_DeltaRotation.x)) _DeltaRotation.x = 0;
                if (float.IsNaN(_DeltaRotation.y)) _DeltaRotation.y = 0;
                if (float.IsNaN(_DeltaRotation.z)) _DeltaRotation.z = 0;
                return _DeltaRotation;
            }
        }

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
        /// Prepare for update
        /// </summary>
        internal void BeginUpdate()
        {
            CleanUpActiveList();
        }

        /// <summary>
        /// Update Layer
        /// </summary>
        internal void Update()
        {
            _DeltaPosition = _DeltaRotation = UnityEngine.Vector3.zero;
            foreach (var node in ActiveAnimNodes)
            {
                if (node.RootMotion.HasPosition)
                    _DeltaPosition += node.RootMotion.DeltaPosition * node.BlendWeight.RootMotion;
                if (node.RootMotion.HasRotation)
                    _DeltaRotation += node.RootMotion.DeltaRotation * node.BlendWeight.RootMotion;
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
        private void CleanUpActiveList()
        {
            int i = 0;
            while (i < ActiveAnimNodes.Count)
            {
                if (ActiveAnimNodes[i].BlendWeight.Weight == 0.0f)
                {
                    ActiveAnimNodes.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }


        /// <summary>
        /// Apply changes to UnityEngine.Animation component 
        /// </summary>
        /// <param name="animationComponent">UnityEngine.Animation to apply changes to</param>
        internal void Apply(UnityEngine.Animation animationComponent)
        {
            foreach (var anim in ActiveAnimNodes)// iterate throw all active AnimNodeSequences
            {
                UnityEngine.AnimationState state = animationComponent[anim.CurrentAnimation];// access state
                if (state != null)
                {
                    state.speed = anim.Speed;
                    // set parameters
                    state.blendMode = BlendMode;
                    state.wrapMode = anim.WrapMode;
                    state.layer = LayerIndex;
                    state.weight = anim.BlendWeight.Weight;

                    // disable or enable animation
                    if (anim.BlendWeight.Weight == 0)
                        state.enabled = false;
                    else
                        state.enabled = true;
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
