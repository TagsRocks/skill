using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// base class for AnimNodes that use single layer for children
    /// </summary>
    public abstract class AnimNodeSingleLayer : AnimNodeBlendBase
    {
        /// <summary>
        /// Create new instance of AnimNodeSingleLayer
        /// </summary>
        /// <param name="childCount">number of children</param>
        public AnimNodeSingleLayer(int childCount)
            : base(childCount)
        {

        }
        /// <summary>
        /// Allow each node to get apropriate AnimationLayer
        /// </summary>
        /// <param name="manager">LayerManager to create layer</param>
        /// <param name="parentSuggestLayer">AnimationLayer suggested by parent. (layer of child at index 0)</param>
        public override void SelectLayer(AnimationLayerManager manager, AnimationLayer parentSuggestLayer)
        {
            AnimationLayer layer = parentSuggestLayer;
            foreach (var item in this)
            {
                if (item != null)
                {
                    item.SelectLayer(manager, layer);
                }
            }
        }
    }
}
