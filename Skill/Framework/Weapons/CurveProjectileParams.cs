using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// Defines rotation of curve projectile when spawned by weapon
    /// </summary>
    public enum InitialCurveProjectileRotation
    { 
        Identity,
        Forward,
        AbsoluteCustom,
        RelativeCustom,
        
    }

    /// <summary>
    /// Parameters needed by weapon when shooting Curve projectiles
    /// </summary>
    public class CurveProjectileParams
    {
        /// <summary> Angle relative to direction to throw bullets. </summary>
        public float ThrowAngle = 45;

        /// <summary> Rotation of bulet when spawn</summary>
        public InitialCurveProjectileRotation InitialRotation;

        /// <summary> Custom Rotation of bulet when InitialRotation is Custom</summary>
        public Quaternion Rotation;
    }   
}
