using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationBlendByIdle : AnimationBlendBase
    {
        private float _IdleWeight;

        public AnimationNode Idle { get { return this[1]; } set { this[1] = value; } }
        public AnimationNode Moving { get { return this[0]; } set { this[0] = value; } }


        public float IdleWeight { get { return _IdleWeight; } private set { _IdleWeight = value; if (_IdleWeight < 0) _IdleWeight = 0; else if (_IdleWeight > 1) _IdleWeight = 1; } }

        public bool IsIdle { get; set; }
        
        protected override void CalcBlendWeights(ref float[] blendWeights)
        {
            //if (IsIdle) IdleWeight += BlendRate;
            //else IdleWeight -= BlendRate;

            if (IsIdle) _IdleWeight = 1;
            else _IdleWeight = 0;

            blendWeights[1] = _IdleWeight;
            blendWeights[0] = (1.0f - _IdleWeight);
        }

        public AnimationBlendByIdle()
            : base(2)
        {
        }        
    }
}
