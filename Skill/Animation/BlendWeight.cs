using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Weight information
    /// </summary>
    public class BlendWeight
    {        
        private float _Weight; // last setted amount of weight        
        private float _RootMotion;

        /// <summary>
        /// Get or set weight of node (0.0f - 1.0f)
        /// </summary>
        public float Weight
        {
            get { return _Weight; }
            internal set
            {
                _Weight = value;
                if (_Weight < 0) _Weight = 0; else if (_Weight > 1) _Weight = 1;
            }
        }

        /// <summary>
        /// Get or set root motion weight of node (0.0f - 1.0f)
        /// </summary>
        public virtual float RootMotion
        {
            get { return _RootMotion; }
            internal set
            {
                _RootMotion = value;
                if (_RootMotion < 0) _RootMotion = 0; else if (_RootMotion > 1) _RootMotion = 1;
            }
        }

        /// <summary>
        /// Set both Weight and RootMotionWeight in one call
        /// </summary>
        /// <param name="value">weight</param>
        public void SetBoth(float value)
        {
            Weight = value;
            RootMotion = _Weight;
        }
    }
}
