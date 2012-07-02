using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Defiens bass class that have children and manage weights of them
    /// </summary>
    public abstract class AnimNodeBlendBase : AnimNode
    {
        private float[] _BlendWeights;

        /// <summary>
        /// Retrieves lenght of active sub branch
        /// </summary>
        public override float Length
        {
            get
            {
                float maxW = 0;
                AnimNode maxChild = null;
                for (int i = 0; i < ChildCount; i++)
                {
                    AnimNode child = this[i];
                    if (child != null)
                    {
                        if (_BlendWeights[i] >= maxW)
                        {
                            maxW = _BlendWeights[i];
                            maxChild = child;
                        }
                    }
                }
                if (maxChild != null)
                    return maxChild.Length;
                return 0;
            }
        }        


        /// <summary>
        /// How long to take to get to the blend target.
        /// </summary>
        public virtual float BlendTime { get; set; }

        /// <summary>
        /// calculate blend base on deltatime
        /// </summary>
        protected float BlendRate { get { if (BlendTime > 0) return UnityEngine.Time.deltaTime / BlendTime; else return 1; } }

        /// <summary>
        /// Create new instance of AnimNodeBlendBase
        /// </summary>
        /// <param name="childCount">number of children</param>
        public AnimNodeBlendBase(int childCount)
            : base(childCount)
        {
            _BlendWeights = new float[childCount];
            BlendTime = 0.3f;
        }

        /// <summary>
        /// Blend between children
        /// </summary>
        protected override void Blend()
        {
            CalcBlendWeights(ref _BlendWeights);
            for (int i = 0; i < ChildCount; i++)
            {
                var child = this[i];
                if (child != null)
                {
                    child.Weight = _BlendWeights[i] * Weight;
                }
            }
        }



        /// <summary>
        /// subclasses should implement this and provide valid weight (0.0 - 0.1) for each child 
        /// </summary>
        /// <param name="blendWeights"></param>
        protected abstract void CalcBlendWeights(ref float[] blendWeights);
    }
}
