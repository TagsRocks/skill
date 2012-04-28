using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public abstract class AnimationSwitchBase : AnimationNode
    {
        //private int _PreActiveIndex;
        private int _ActiveChildIndex;
        public int ActiveChildIndex { get { return _ActiveChildIndex; } private set { _ActiveChildIndex = value; } }

        public override float Length
        {
            get
            {
                if (_ActiveChildIndex >= 0 && _ActiveChildIndex < ChildCount)
                {
                    if (this[_ActiveChildIndex] != null)
                        return this[_ActiveChildIndex].Length;
                }
                return 0;
            }
        }

        public AnimationSwitchBase(int childCount)
            : base(childCount)
        {
            ActiveChildIndex = 0;
        }

        protected override void Updating()
        {
            ActiveChildIndex = SelectActiveChildIndex();
            for (int i = 0; i < ChildCount; i++)
            {
                var child = this[i];
                if (child != null)
                {
                    if (ActiveChildIndex == i)
                    {
                        //child.Weight += BlendRate;
                        //if (child.Weight > Weight) child.Weight = Weight;
                        child.Weight = Weight;
                    }
                    else
                    {
                        child.Weight = 0;
                        //child.Weight -= blendRate;
                    }
                }
            }
        }

        protected abstract int SelectActiveChildIndex();

        public override void SetLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            //AnimationLayer layer = manager.Last(LayerMode.CrossFade);
            AnimationLayer layer = parentSuggestLayer;
            foreach (var item in this)
            {
                if (item != null)
                {
                    item.SetLayer(manager, layer);
                }
            }
        }
    }
}
