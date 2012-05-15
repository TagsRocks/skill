using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Contains AnimationLayers of an AnimationTree
    /// </summary>
    public class AnimationLayerManager
    {
        /// <summary>
        /// List of AnimationLayer. (do not modify manually)
        /// </summary>
        public List<AnimationLayer> Layers { get; private set; }

        /// <summary>
        /// Retrieves number of AnimNodeSequence that updated in current update(frame)
        /// </summary>
        public int EnableAnimNodeSequenceCount
        {
            get
            {
                int count = 0;
                foreach (var layer in Layers)
                {
                    count += layer.ActiveAnimNodes.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Create an instance of AnimationLayerManager
        /// </summary>
        public AnimationLayerManager()
        {
            Layers = new List<AnimationLayer>(10);
        }

        /// <summary>
        /// Create new layer with specified AnimationBlendMode
        /// </summary>
        /// <param name="blendMode">AnimationBlendMode</param>
        /// <returns>New create and regitered AnimationLayer</returns>
        public AnimationLayer Create(UnityEngine.AnimationBlendMode blendMode)
        {
            AnimationLayer layer = new AnimationLayer(Layers.Count, blendMode);
            Layers.Add(layer);
            return layer;
        }

        //public AnimationLayer First(UnityEngine.AnimationBlendMode blendMode)
        //{
        //    for (int i = 0; i < Layers.Count; i++)
        //    {
        //        var layer = Layers[i];
        //        if (layer.BlendMode == blendMode) return layer;
        //    }
        //    return Create(blendMode);
        //}
        //public AnimationLayer Last(UnityEngine.AnimationBlendMode blendMode)
        //{
        //    for (int i = Layers.Count - 1; i >= 0; i--)
        //    {
        //        var layer = Layers[i];
        //        if (layer.BlendMode == blendMode) return layer;
        //    }
        //    return Create(blendMode);
        //}
    }
}
