using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    public class AnimationBlendByPosture : AnimationSwitchBase
    {

        public AnimationNode Standing { get { return this[0]; } set { this[0] = value; } }
        public AnimationNode Crouched { get { return this[1]; } set { this[1] = value; } }
        public AnimationNode Prone { get { return this[2]; } set { this[2] = value; } }

        public Posture Posture { get; set; }

        public AnimationBlendByPosture()
            : base(2)
        {

        }

        protected override int SelectActiveChildIndex()
        {
            switch (Posture)
            {
                case Posture.Stand:
                    return 0;
                case Posture.Crouch:
                    return 1;
                case Posture.prone:
                    return 2;
            }
            return 0;
        }
    }
}
