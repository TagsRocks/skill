using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Try to keep single child active at time
    /// </summary>
    public class AnimNodeBlendByIndex : AnimNodeSingleLayer
    {

        private int _SelectedChildIndex;

        /// <summary>
        /// Get or set selected child by index
        /// </summary>
        public int SelectedChildIndex
        {
            get { return _SelectedChildIndex; }
            set
            {
                if (value < 0) value = 0;
                else if (value >= ChildCount) value = ChildCount - 1;
                _SelectedChildIndex = value;
            }
        }

        /// <summary>
        /// Get or set selected child by name
        /// </summary>
        public string SelectedChildName
        {
            get
            {
                if (_SelectedChildIndex < ChildCount)
                {
                    if (this[_SelectedChildIndex] != null)
                        return this[_SelectedChildIndex].Name;
                }
                return string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        AnimNode childNode = this[i];
                        if (childNode != null)
                        {
                            if (childNode.Name == value)
                            {
                                _SelectedChildIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrives selected child node
        /// </summary>
        public AnimNode SelectedChildNode { get { return this[_SelectedChildIndex]; } }

        /// <summary>
        /// Retrieves lenght of active sub branch
        /// </summary>
        public override float Length
        {
            get
            {
                if (SelectedChildNode != null)
                    return SelectedChildNode.Length;
                return 0;
            }
        }

        /// <summary>
        /// Create new instance of AnimNodeBlendByIndex
        /// </summary>
        /// <param name="childCount">number of children</param>
        public AnimNodeBlendByIndex(int childCount)
            : base(childCount)
        {

        }
        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>        
        protected override void CalcBlendWeights(ref BlendWeight[] blendWeights)
        {
            float blendRate = BlendRate;
            for (int i = 0; i < blendWeights.Length; i++)
            {
                float f = blendWeights[i].Weight;
                if (i == _SelectedChildIndex)
                {
                    f += blendRate;
                    if (f > 1)
                        f = 1;
                }
                else
                {
                    f -= blendRate;
                    if (f < 0)
                        f = 0;
                }

                blendWeights[i].SetBoth(f);
            }
        }
    }
}
