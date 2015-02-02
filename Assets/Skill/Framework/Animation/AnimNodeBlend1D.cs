using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// This blend node allows the Anim Tree to automatically blend between inputs between the constraints based on the size of the Velocity or Acceleration vector within the owning actor.
    /// The constraints define the bounds between each input, 
    /// for example, Constraints[0] and Constraints[1] define the lower and upper bound for index 0;
    /// Constraints[1] and Constraints[2] define the lower and upper bound for index 1; and so forth.
    /// These bounds are modified by the Blend Down Perc value, set Blend Down Perc to zero if you wish to keep the bounds strict.
    /// http://udn.epicgames.com/Three/AnimationNodes.html#AnimNodeBlendBySpeed
    /// </summary>
    public class AnimNodeBlend1D : AnimNodeSingleLayer
    {

        private bool _IsChanged;
        private float _Value;
        /// <summary>
        /// Get or set value of actor
        /// </summary>
        public float Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    _IsChanged = true;
                }
            }
        }

        /// <summary> Thresholds </summary>
        public float[] Thresholds { get; private set; }

        /// <summary>
        /// Create new instance of AnimNodeBlendBySpeed
        /// </summary>
        /// <param name="childCoun">number of children</param>
        public AnimNodeBlend1D(int childCoun)
            : base(childCoun)
        {
            Thresholds = new float[childCoun];
            _IsChanged = true;
        }

        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>        
        protected override void CalcBlendWeights(ref BlendWeight[] blendWeights)
        {
            if (_IsChanged)
            {
                _IsChanged = false;
                for (int i = 0; i < blendWeights.Length; i++)
                    blendWeights[i].SetBoth(0);

                int index = -1;
                for (int i = 0; i < ChildCount - 1; i++)
                {
                    if (Value >= Thresholds[i] && Value < Thresholds[i + 1])
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    if (Value <= Thresholds[0])
                        blendWeights[0].SetBoth(1.0f);
                    else
                        if (Value >= Thresholds[Thresholds.Length - 1])
                            blendWeights[Thresholds.Length - 1].SetBoth(1.0f);
                }
                else
                {
                    float percent = UnityEngine.Mathf.Clamp01((Value - Thresholds[index]) / (Thresholds[index + 1] - Thresholds[index]));
                    blendWeights[index].SetBoth((1.0f - percent));
                    blendWeights[index + 1].SetBoth(percent);
                }
            }
        }
    }
}
