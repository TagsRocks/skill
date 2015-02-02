using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// place children nodes in different layers.
    /// </summary>
    /// <remarks>
    /// for example :
    /// almost in reload AnimationClips only hand bones involved and other bones do not have keys.
    /// you can use this node to override hand animations so the lower body can play another animation
    /// be sure that lower body bones do not have any keys in AnimationClip or set MixingTransforms for bones
    /// Another usage of this nodeis when you want to play an IdleBreak animation sometimes
    /// </remarks>
    public class AnimNodeOverride : AnimNodeMultilayer
    {
        /// <summary>
        /// base node
        /// </summary>
        public AnimNode BaseNode { get { return this[0]; } }
        /// <summary>
        /// Override Node (higher layer)
        /// </summary>
        public AnimNode OverrideNode { get { return this[1]; } }


        private bool _IsChanged;
        private float _Value;

        /// <summary>
        /// Weight of override layer
        /// </summary>
        public float Value
        {
            get { return _Value; }
            set
            {
                value = UnityEngine.Mathf.Clamp01(value);
                if (_Value != value)
                {
                    _Value = value;
                    _IsChanged = true;
                }

            }
        }


        /// <summary>
        /// Lenght of active branch
        /// </summary>
        public override float Length
        {
            get
            {
                if (_Value > 0)
                {
                    if (OverrideNode != null) return OverrideNode.Length;
                }
                else
                {
                    if (BaseNode != null) return BaseNode.Length;
                }
                return 0;
            }
        }

        /// <summary>
        /// Create new instance of AnimNodeOverride
        /// </summary>        
        public AnimNodeOverride()
            : base(2)
        {
            _Value = 0;
            _IsChanged = true;
        }

        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>        
        protected override void CalcBlendWeights(ref BlendWeight[] blendWeights)
        {
            if (_IsChanged)
            {
                blendWeights[1].SetBoth(_Value);
                blendWeights[0].Weight = 1.0f;
                blendWeights[0].RootMotion = 1.0f - _Value;
                _IsChanged = false;
            }
        }
    }
}
