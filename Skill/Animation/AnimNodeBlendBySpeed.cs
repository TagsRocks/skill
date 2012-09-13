using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// This blend node allows the Anim Tree to automatically blend between inputs between the constraints based on the size of the Velocity or Acceleration vector within the owning actor.
    /// The constraints define the bounds between each input, 
    /// for example, Constraints[0] and Constraints[1] define the lower and upper bound for index 0;
    /// Constraints[1] and Constraints[2] define the lower and upper bound for index 1; and so forth.
    /// These bounds are modified by the Blend Down Perc value, set Blend Down Perc to zero if you wish to keep the bounds strict.
    /// http://udn.epicgames.com/Three/AnimationNodes.html#AnimNodeBlendBySpeed
    /// </summary>
    public class AnimNodeBlendBySpeed : AnimNodeSingleLayer
    {
        private Skill.TimeWatch _DelayTW;
        private int _PreActiveIndex;
        private float[] _TargetBlendWeights;

        /// <summary>
        /// Get or set velocity of actor
        /// </summary>
        public float Velocity { get; set; }

        /// <summary>
        /// How fast to blend when going up an index.
        /// </summary>
        public float BlendUpTime { get; set; }
        /// <summary>
        /// How fast to blend when going down an index.
        /// </summary>
        public float BlendDownTime { get; set; }

        private float _BlendDownPercent;
        /// <summary>
        /// Where abouts in the constraint bounds should the blend start blending down.
        /// </summary>
        public float BlendDownPercent { get { return _BlendDownPercent; } set { _BlendDownPercent = value; if (_BlendDownPercent < 0.0f)_BlendDownPercent = 0.0f; else if (_BlendDownPercent > 1.0f)_BlendDownPercent = 1.0f; } }
        /// <summary>
        /// Time delay before blending up an index.
        /// </summary> 
        public float BlendUpDelay { get; set; }
        /// <summary>
        /// Time delay before blending down an index.
        /// </summary>
        public float BlendDownDelay { get; set; }

        /// <summary> Constraints </summary>
        public float[] Constraints { get; private set; }

        /// <summary> Number of constraints (ChildCount + 1)</summary>
        public float ConstraintCount { get { return Constraints.Length; } }

        /// <summary>
        /// Create new instance of AnimNodeBlendBySpeed
        /// </summary>
        /// <param name="childCoun">number of children</param>
        public AnimNodeBlendBySpeed(int childCoun)
            : base(childCoun)
        {
            _TargetBlendWeights = new float[childCoun];
            Constraints = new float[childCoun + 1];
            BlendUpTime = BlendTime;
            BlendDownTime = BlendTime;
            BlendDownPercent = 0.0f;
            BlendUpDelay = 0;
            BlendDownDelay = 0;
            _PreActiveIndex = 0;
        }
                
        private void MoveTowardsTarget(ref float[] blendWeights)
        {
            float blendRate = BlendRate;
            for (int i = 0; i < blendWeights.Length; i++)
            {
                float f = blendWeights[i];
                float t = _TargetBlendWeights[i];

                if (f < t)
                {
                    f += blendRate;
                    if (f > t)
                        f = t;
                }
                else if (f > t)
                {
                    f -= blendRate;
                    if (f < t)
                        f = t;
                }
                blendWeights[i] = f;
            }            
        }

        private void CalcTargetBlendWeights(int index)
        {
            float max, min;
            // we must reach velocity from lower index
            float delta = (Constraints[index + 1] - Constraints[index]) * BlendDownPercent;
            max = Constraints[index] + delta;

            if (Velocity <= max)
            {
                if (index > 0)
                {
                    delta = Constraints[index] - Constraints[index - 1];
                    min = Constraints[index] - (delta * BlendDownPercent);
                }
                else
                    min = Constraints[index];

                float percent = (Velocity - min) / (max - min);
                _TargetBlendWeights[index] = percent;
                if (index > 1)
                    _TargetBlendWeights[index - 1] = 1.0f - percent;
            }
            else
            {
                min = Constraints[index + 1] - delta;

                if (Velocity > min)
                {
                    if (index < ChildCount)
                    {
                        delta = (Constraints[index + 2] - Constraints[index + 1]) * BlendDownPercent;
                        max = Constraints[index + 1] + delta;
                    }
                    else
                        max = Constraints[index + 1];

                    float percent = (Velocity - min) / (max - min);
                    _TargetBlendWeights[index] = percent;
                    if (index < ChildCount)
                        _TargetBlendWeights[index + 1] = 1.0f - percent;
                }
                else
                    _TargetBlendWeights[index] = 1.0f;
            }
        }

        /// <summary>
        /// Calculate weight of children between 0.0f - 1.0f
        /// </summary>
        /// <param name="blendWeights">previous weight of children</param>
        protected override void CalcBlendWeights(ref float[] blendWeights)
        {
            for (int i = 0; i < _TargetBlendWeights.Length; i++)
                _TargetBlendWeights[i] = 0;

            int index = 0;
            if (Velocity < Constraints[0] || Velocity > Constraints[ChildCount])
            {
                BlendTime = BlendDownTime;
            }
            else
            {
                for (int i = 0; i < ChildCount; i++)
                {
                    if (Velocity >= Constraints[i] && Velocity < Constraints[i + 1])
                    {
                        index = i;
                        break;
                    }
                }

                CalcTargetBlendWeights(index);

                if (_PreActiveIndex != index)
                {
                    if (_PreActiveIndex > index)
                    {
                        if (!_DelayTW.Enabled)
                            _DelayTW.Begin(BlendDownDelay);
                        if (_DelayTW.Enabled)
                        {
                            if (_DelayTW.IsOver)
                            {
                                BlendTime = BlendDownTime;
                                _PreActiveIndex = index;
                                _DelayTW.End();
                            }
                        }
                    }
                    else
                    {
                        if (!_DelayTW.Enabled)
                            _DelayTW.Begin(BlendUpDelay);
                        if (_DelayTW.Enabled)
                        {
                            if (_DelayTW.IsOver)
                            {
                                BlendTime = BlendUpTime;
                                _PreActiveIndex = index;
                                _DelayTW.End();
                            }
                        }
                    }
                }
                MoveTowardsTarget(ref blendWeights);
            }
        }
    }
}
