using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public abstract class AnimationBlendBase : AnimationNode
    {
        private float[] _PreBlendWeights;
        private float[] _BlendWeights;

        public override float Length
        {
            get
            {
                float maxW = 0;
                AnimationNode maxChild = null;
                for (int i = 0; i < ChildCount; i++)
                {
                    AnimationNode child = this[i];
                    if (child != null)
                    {
                        if (child.Weight >= maxW)
                        {
                            maxW = child.Weight;
                            maxChild = child;
                        }
                    }
                }
                if (maxChild != null)
                    return maxChild.Length;
                return 0;
            }
        }

        public AnimationBlendBase(int childCount)
            : base(childCount)
        {
            _BlendWeights = new float[childCount];
            _PreBlendWeights = new float[childCount];
        }



        protected override void Updating()
        {
            CalcBlendWeights(ref _BlendWeights);
            float blendRate = BlendRate;
            for (int i = 0; i < ChildCount; i++)
            {
                if (_PreBlendWeights[i] > _BlendWeights[i])
                {
                    _PreBlendWeights[i] -= blendRate;
                    if (_PreBlendWeights[i] < _BlendWeights[i])
                        _PreBlendWeights[i] = _BlendWeights[i];
                }
                else
                {
                    _PreBlendWeights[i] += blendRate;
                    if (_PreBlendWeights[i] > _BlendWeights[i])
                        _PreBlendWeights[i] = _BlendWeights[i];
                }

                var child = this[i];
                if (child != null)
                {
                    child.Weight = _PreBlendWeights[i] * Weight;
                }
            }
        }

        protected abstract void CalcBlendWeights(ref float[] blendWeights);

        public override void SetLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            AnimationLayer layer = parentSuggestLayer;
            if (ChildCount > 0)
            {
                if (this[0] != null)
                    this[0].SetLayer(manager, layer);
            }
            for (int i = 1; i < ChildCount; i++)
            {
                var child = this[i];
                if (child != null)
                {
                    layer = manager.NewLayer(LayerMode.BlendAll);
                    child.SetLayer(manager, layer);
                }
            }
        }
    }
}
