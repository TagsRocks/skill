using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// This blend node allows the Anim Tree to automatically blend between three inputs;
    /// Standing, Crouched and Prone.
    /// </summary>
    public class AnimNodeBlendByPosture : AnimNodeBlendByIndex
    {
        /// <summary> Standing branch </summary>
        public AnimNode StandingNode { get { return this[0]; } set { this[0] = value; } }
        /// <summary> Crouched branch </summary>
        public AnimNode CrouchedNode { get { return this[1]; } set { this[1] = value; } }
        /// <summary> Prone branch </summary>
        public AnimNode ProneNode { get { return this[2]; } set { this[2] = value; } }


        private AnimPosture _Posture;

        /// <summary>
        /// Get or set posture of actor
        /// </summary>
        public AnimPosture Posture
        {
            get
            {
                return _Posture;
            }
            set
            {
                _Posture = value;
                SelectedChildIndex = (int)_Posture;
            }
        }

        /// <summary>
        /// Create new instance of AnimNodeBlendByPosture
        /// </summary>
        public AnimNodeBlendByPosture()
            : base(3)
        {

        }
    }
}
