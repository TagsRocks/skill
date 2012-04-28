using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationBlendBySpeed : AnimationSwitchBase
    {
        private float _StartBlendUp;
        private float _StartBlendDown;
        //private float _BlendDownPercent;
        private int _PreActiveIndex;        

        public float Velocity { get; set; }

        // How fast to blend when going up an index.
        public float BlendUpTime { get; set; }
        // How fast to blend when going down an index.
        public float BlendDownTime { get; set; }
        // Where abouts in the constraint bounds should the blend start blending down.
        //public float BlendDownPercent { get { return _BlendDownPercent; } set { _BlendDownPercent = value; if (_BlendDownPercent < 0.0f)_BlendDownPercent = 0.0f; else if (_BlendDownPercent > 1.0f)_BlendDownPercent = 1.0f; } }
        // Time delay before blending up an index.
        public float BlendUpDelay { get; set; }
        // Time delay before blending down an index.
        public float BlendDownDelay { get; set; }

        /// <summary> Constraints </summary>
        public float[] Constraints { get; private set; }

        /// <summary> Number of constraints (ChildCount + 1)</summary>
        public float ConstraintCount { get { return Constraints.Length; } }                

        public AnimationBlendBySpeed(int childCoun)
            : base(childCoun)
        {
            Constraints = new float[childCoun + 1];
            BlendUpTime = BlendTime;
            BlendDownTime = BlendTime;
            //BlendDownPercent = 0.5f;
            _PreActiveIndex = 0;
            BlendUpDelay = 0;
            BlendDownDelay = 0;
        }

        protected override int SelectActiveChildIndex()
        {
            int index = _PreActiveIndex;
            if (Velocity < Constraints[0])
                return -1;
            else if (Velocity > Constraints[ChildCount])
                return ChildCount;
            else
            {
                for (int i = 0; i < ChildCount; i++)
                {
                    //float c1 = _Constraints[i];
                    //float c2 = _Constraints[i + 1];
                    if (Velocity >= Constraints[i] && Velocity <= Constraints[i + 1])
                    {
                        index = i;
                        break;
                    }
                    //{
                    //    float p = 1.0f - Velocity / (c1 + c2);
                    //    if (p < BlendDownPercent) { index = i; break; }
                    //}
                }
            }

            if (_PreActiveIndex != index)
            {
                if (_PreActiveIndex > index)
                {
                    if (_StartBlendDown < 0)
                        _StartBlendDown = UnityEngine.Time.time;
                    if (UnityEngine.Time.time >= BlendDownDelay + _StartBlendDown)
                    {
                        BlendTime = BlendDownTime;
                        _PreActiveIndex = index;
                        _StartBlendDown = -1;
                    }
                }
                else
                {
                    if (_StartBlendUp < 0)
                        _StartBlendUp = UnityEngine.Time.time;
                    if (UnityEngine.Time.time >= BlendUpDelay + _StartBlendUp)
                    {
                        BlendTime = BlendDownTime;
                        _PreActiveIndex = index;
                        _StartBlendUp = -1;
                    }
                }
            }
            return _PreActiveIndex;
        }
    }
}
