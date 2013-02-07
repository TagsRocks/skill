using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Animation
{
    /// <summary>
    /// Defines keyframes for a 3 float component
    /// </summary>
    public class Vector3Keyframes
    {
        /// <summary> Keyframes for X component </summary>
        public Keyframe[] XKeys { get; private set; }
        /// <summary> Keyframes for Y component </summary>
        public Keyframe[] YKeys { get; private set; }
        /// <summary> Keyframes for Z component </summary>
        public Keyframe[] ZKeys { get; private set; }

        /// <summary>
        /// Create a Vector3Keyframes
        /// </summary>
        /// <param name="xKeys"> Keyframes for X component </param>
        /// <param name="yKeys"> Keyframes for Y component </param>
        /// <param name="zKeys"> Keyframes for Z component </param>
        public Vector3Keyframes(Keyframe[] xKeys, Keyframe[] yKeys, Keyframe[] zKeys)
        {
            this.XKeys = xKeys;
            this.YKeys = yKeys;
            this.ZKeys = zKeys;
        }
    }
}
