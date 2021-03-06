﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// base class for AnimNodes that use multiple layers for children
    /// </summary>
    public abstract class AnimNodeMultilayer : AnimNodeBlendBase
    {

        protected virtual UnityEngine.AnimationBlendMode BlendMode { get { return UnityEngine.AnimationBlendMode.Blend; } }

        /// <summary>
        /// Create new instance of AnimNodeMultilayer
        /// </summary>
        /// <param name="childCount">number of children</param>
        public AnimNodeMultilayer(int childCount)
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
            SyncLayer = false;
            if (NewLayer)
                layer = manager.Create(UnityEngine.AnimationBlendMode.Blend);

            if (ChildCount > 0)
            {
                if (this[0] != null)
                    this[0].SelectLayer(manager, layer);
            }
            for (int i = 1; i < ChildCount; i++)
            {
                var child = this[i];
                if (child != null)
                {
                    layer = manager.Create(BlendMode);
                    child.SelectLayer(manager, layer);
                }
            }
        }
    }
}
