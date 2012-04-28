using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationLayerManager
    {
        private List<AnimationLayer> _Layers;

        internal List<AnimationLayer> Layers { get { return _Layers; } }

        public AnimationLayer DefaultCrossFade { get; private set; }        

        public AnimationLayerManager()
        {
            _Layers = new List<AnimationLayer>(10);
            DefaultCrossFade = NewLayer(LayerMode.CrossFade);            
        }

        public AnimationLayer NewLayer(LayerMode mode)
        {
            AnimationLayer layer = new AnimationLayer(_Layers.Count, mode);
            _Layers.Add(layer);
            return layer;
        }
        public AnimationLayer First(LayerMode mode)
        {
            for (int i = 0; i < _Layers.Count; i++)
            {
                var layer = _Layers[i];
                if (layer.Mode == mode) return layer;
            }
            return NewLayer(mode);
        }
        public AnimationLayer Last(LayerMode mode)
        {
            for (int i = _Layers.Count - 1; i >= 0; i--)
            {
                var layer = _Layers[i];
                if (layer.Mode == mode) return layer;
            }
            return NewLayer(mode);
        }
    }
}
