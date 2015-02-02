using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// Defines keyframes for a 3 float component
    /// </summary>
    public class RootMotionKeyframes
    {
        /// <summary> Keyframes for X component </summary>
        public Keyframe[] XKeys { get; private set; }
        /// <summary> Keyframes for Y component </summary>
        public Keyframe[] YKeys { get; private set; }
        /// <summary> Keyframes for Z component </summary>
        public Keyframe[] ZKeys { get; private set; }


        /// <summary> Keyframes for X component </summary>
        public Keyframe[] RXKeys { get; private set; }
        /// <summary> Keyframes for Y component </summary>
        public Keyframe[] RYKeys { get; private set; }
        /// <summary> Keyframes for Z component </summary>
        public Keyframe[] RZKeys { get; private set; }


        /// <summary>
        /// Create a RootMotionKeyframes
        /// </summary>
        /// <param name="xKeys"> Keyframes for X component </param>
        /// <param name="yKeys"> Keyframes for Y component </param>
        /// <param name="zKeys"> Keyframes for Z component </param>
        public RootMotionKeyframes(Keyframe[] xKeys, Keyframe[] yKeys, Keyframe[] zKeys, Keyframe[] rxKeys, Keyframe[] ryKeys, Keyframe[] rzKeys)
        {
            this.XKeys = xKeys;
            this.YKeys = yKeys;
            this.ZKeys = zKeys;

            this.RXKeys = rxKeys;
            this.RYKeys = ryKeys;
            this.RZKeys = rzKeys;
        }
    }
}
