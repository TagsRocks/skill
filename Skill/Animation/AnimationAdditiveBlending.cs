using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationAdditiveBlending : AnimationBlendBase
    {
        //private float _AdditiveWeight;
        private float _TargetWeight;

        public AnimationNode Normal { get { return this[0]; } set { this[0] = value; } }
        public AnimationNode Additive { get { return this[1]; } set { this[1] = value; } }

        public float TargetWeight { get { return _TargetWeight; } set { _TargetWeight = value; if (_TargetWeight < 0) _TargetWeight = 0; else if (_TargetWeight > 1) _TargetWeight = 1; } }

        public AnimationAdditiveBlending()
            : base(2)
        {

        }

        //protected override void Updating()
        //{
        //    if (_AdditiveWeight != TargetWeight)
        //    {
        //        float blendRate = BlendRate;
        //        if (_AdditiveWeight > TargetWeight)
        //        {
        //            _AdditiveWeight -= blendRate;
        //            if (_AdditiveWeight < TargetWeight)
        //                _AdditiveWeight = TargetWeight;
        //        }
        //        else
        //        {
        //            _AdditiveWeight += blendRate;
        //            if (_AdditiveWeight > TargetWeight)
        //                _AdditiveWeight = TargetWeight;
        //        }
        //    }

        //    base.Updating();
        //}

        protected override void CalcBlendWeights(ref float[] blendWeights)
        {
            blendWeights[0] = 1;
            blendWeights[1] = _TargetWeight;
        }

        public override void SetLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            var child = this[0];
            if (child != null)
                child.SetLayer(manager, parentSuggestLayer);

            child = this[1];
            if (child != null)
            {
                AnimationLayer layer = parentSuggestLayer;
                layer = manager.NewLayer(LayerMode.Additive);
                child.SetLayer(manager, layer);
            }
        }

        public override void SetBlendTime(float blendTime, bool applyToChildren)
        {
            base.SetBlendTime(blendTime, applyToChildren);
            if (!applyToChildren) Additive.BlendTime = blendTime;
        }
    }
}
