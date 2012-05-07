using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// This blend node allows the Anim Tree to combine additive animation, or a blend of additive animations.    
    /// </summary>
    public class AnimNodeAdditiveBlending : AnimNodeBlendBase
    {
        private float _AdditiveWeight;

        /// <summary>
        /// Weight of AdditiveLayer
        /// </summary>
        public float AdditiveWeight { get { return _AdditiveWeight; } private set { _AdditiveWeight = value; if (_AdditiveWeight < 0) _AdditiveWeight = 0; else if (_AdditiveWeight > 1) _AdditiveWeight = 1; } }

        /// <summary>
        /// Whether additive layer is enable?
        /// </summary>
        public bool IsAdditive { get; set; }

        /// <summary>
        /// The AnimNode that use input blendmode specified by parent
        /// </summary>
        public AnimNode NormalNode { get { return this[0]; } }

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
                if (IsAdditive)
                {
                    if (AdditiveNode != null) return AdditiveNode.Length;
                }
                else
                {
                    if (NormalNode != null) return NormalNode.Length;
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

        }

        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>
        protected override void CalcBlendWeights(ref float[] blendWeights)
        {
            if (IsAdditive) AdditiveWeight += BlendRate;
            else AdditiveWeight -= BlendRate;

            blendWeights[1] = _AdditiveWeight;
            blendWeights[0] = 1.0f;
        }

        /// <summary>
        /// Allow each node to get apropriate AnimationLayer
        /// </summary>
        /// <param name="manager">LayerManager to create layer</param>
        /// <param name="parentSuggestLayer">AnimationLayer suggested by parent. (layer of child at index 0)</param>
        public override void SelectLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            var child = this[0];
            if (child != null)
                child.SelectLayer(manager, parentSuggestLayer);

            child = this[1];
            if (child != null)
            {
                AnimationLayer layer = parentSuggestLayer;
                layer = manager.Create(UnityEngine.AnimationBlendMode.Additive);
                child.SelectLayer(manager, layer);
            }
        }
    }
}
