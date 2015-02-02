using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Limit camera (exp: to go throw objects)
    /// </summary>
    public abstract class CameraLimit : StaticBehaviour
    {
        /// <summary>
        /// Apply limit to position of camera after camera calculation
        /// </summary>
        /// <param name="cameraPosition">position of camera after camera calculation</param>
        /// <param name="preCameraPosition">position of camera in previouse camera calculation</param>
        public abstract void ApplyLimit(ref Vector3 cameraPosition, Vector3 preCameraPosition);
    }
}
