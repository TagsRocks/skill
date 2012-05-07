using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Animation
{
    /// <summary>
    /// Represent data required for AnimNodeAimOffset profile 
    /// </summary>
    public class AnimNodeAimOffsetProfile
    {
        /// <summary> Name of Profile </summary>
        public string Name;

        /// <summary> Array of lenght 9 that contains AnimationNames </summary>
        public string[] Animations { get; private set; }

        /// <summary> AnimationName of CenterCenter child</summary>
        public string CenterCenter { get {return Animations[AnimNodeAimOffset.CenterCenterIndex -1]; } set { Animations[AnimNodeAimOffset.CenterCenterIndex -1] = value; } }
        /// <summary> AnimationName of CenterUp child</summary>
        public string CenterUp { get { return Animations[AnimNodeAimOffset.CenterUpIndex - 1]; } set { Animations[AnimNodeAimOffset.CenterUpIndex - 1] = value; } }
        /// <summary> AnimationName of CenterDown child</summary>
        public string CenterDown { get { return Animations[AnimNodeAimOffset.CenterDownIndex - 1]; } set { Animations[AnimNodeAimOffset.CenterDownIndex - 1] = value; } }
        /// <summary> AnimationName of LeftCenter child</summary>
        public string LeftCenter { get { return Animations[AnimNodeAimOffset.LeftCenterIndex - 1]; } set { Animations[AnimNodeAimOffset.LeftCenterIndex - 1] = value; } }
        /// <summary> AnimationName of LeftUp child</summary>
        public string LeftUp { get { return Animations[AnimNodeAimOffset.LeftUpIndex - 1]; } set { Animations[AnimNodeAimOffset.LeftUpIndex - 1] = value; } }
        /// <summary> AnimationName of LeftDown child</summary>
        public string LeftDown { get { return Animations[AnimNodeAimOffset.LeftDownIndex - 1]; } set { Animations[AnimNodeAimOffset.LeftDownIndex - 1] = value; } }
        /// <summary> AnimationName of RightCenter child</summary>
        public string RightCenter { get { return Animations[AnimNodeAimOffset.RightCenterIndex - 1]; } set { Animations[AnimNodeAimOffset.RightCenterIndex - 1] = value; } }
        /// <summary> AnimationName of RightUp child</summary>
        public string RightUp { get { return Animations[AnimNodeAimOffset.RightUpIndex - 1]; } set { Animations[AnimNodeAimOffset.RightUpIndex - 1] = value; } }
        /// <summary> AnimationName of RightDown child</summary>
        public string RightDown { get { return Animations[AnimNodeAimOffset.RightDownIndex - 1]; } set { Animations[AnimNodeAimOffset.RightDownIndex - 1] = value; } }

        /// <summary>
        /// Create an instance of AnimNodeAimOffsetProfile
        /// </summary>
        public AnimNodeAimOffsetProfile()
        {
            this.Animations = new string[9];
        }
    }
}
