using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// This node automatically blends between idle and moving depending on the velocity.
    /// If the owner's velocity is zero (or a relatively small number) then the node blend to the 'Idle' branch.
    /// Otherwise the node blends to the 'Moving' branch.
    /// http://udn.epicgames.com/Three/AnimationNodes.html#UDKAnimBlendByIdle / _UTAnimBlendByIdle
    /// </summary>
    public class AnimNodeBlendByIdle : AnimNodeMultilayer
    {
        /// <summary>
        /// Idle branch
        /// </summary>
        public AnimNode IdleNode { get { return this[1]; } set { this[1] = value; } }
        /// <summary>
        /// Moving branch
        /// </summary>
        public AnimNode MovingNode { get { return this[0]; } set { this[0] = value; } }

        private float _IdleWeight;
        /// <summary>
        /// Weight of idle branch
        /// </summary>
        public float IdleWeight { get { return _IdleWeight; } private set { _IdleWeight = value; if (_IdleWeight < 0) _IdleWeight = 0; else if (_IdleWeight > 1) _IdleWeight = 1; } }

        /// <summary>
        /// Whether actor is idle?
        /// </summary>
        public bool IsIdle { get; set; }


        /// <summary>
        /// Retrieves lenght of active sub branch
        /// </summary>
        public override float Length
        {
            get
            {
                if (IsIdle)
                {
                    if (IdleNode != null) return IdleNode.Length;
                }
                else
                {
                    if (MovingNode != null) return MovingNode.Length;
                }
                return 0;
            }
        }
        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>        
        protected override void CalcBlendWeights(ref BlendWeight[] blendWeights)
        {
            if (IsIdle) IdleWeight += BlendRate;
            else IdleWeight -= BlendRate;

            blendWeights[1].SetBoth(_IdleWeight);
            blendWeights[0].Weight = 1.0f;
            blendWeights[0].RootMotion = 1.0f - _IdleWeight;
        }

        /// <summary>
        /// Create new instance of AnimNodeBlendByIdle
        /// </summary>
        public AnimNodeBlendByIdle()
            : base(2)
        {
        }
    }
}
