using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// This blend node allows the Anim Tree to combine additive animation, or a blend of additive animations.    
    /// </summary>
    public class AnimNodeAdditiveBlending : AnimNodeMultilayer
    {
        private bool _IsChanged;
        private float _Value;

        /// <summary>
        /// Weight of AdditiveLayer
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

        protected override UnityEngine.AnimationBlendMode BlendMode { get { return UnityEngine.AnimationBlendMode.Additive; } }

        /// <summary>
        /// The AnimNode that use input blendmode specified by parent
        /// </summary>
        public AnimNode BaseNode { get { return this[0]; } }

        /// <summary>
        /// The AnimNode that use additive blendmode
        /// </summary>
        public AnimNode AdditiveNode { get { return this[1]; } }

        /// <summary>
        /// Retrives lenght of active sub branch.
        /// </summary>
        public override float Length
        {
            get
            {
                if (_Value > 0)
                {
                    if (AdditiveNode != null) return AdditiveNode.Length;
                }
                else
                {
                    if (BaseNode != null) return BaseNode.Length;
                }
                return 0;
            }
        }

        /// <summary>
        /// Create an insatance of AnimNodeAdditiveBlending
        /// </summary>
        public AnimNodeAdditiveBlending()
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
                _IsChanged = false;
                blendWeights[1].SetBoth(_Value);
                blendWeights[0].Weight = 1.0f;
                blendWeights[0].RootMotion = 1.0f - _Value;
            }
        }

        /// <summary>
        /// Allow each node to get apropriate AnimationLayer
        /// </summary>
        /// <param name="manager">LayerManager to create layer</param>
        /// <param name="parentSuggestLayer">AnimationLayer suggested by parent. (layer of child at index 0)</param>
        //public override void SelectLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        //{
        //    var child = this[0];
        //    if (child != null)
        //        child.SelectLayer(manager, parentSuggestLayer);

        //    child = this[1];
        //    if (child != null)
        //    {
        //        AnimationLayer layer = parentSuggestLayer;
        //        layer = manager.Create(UnityEngine.AnimationBlendMode.Additive);
        //        child.SelectLayer(manager, layer);
        //    }
        //}
    }
}
