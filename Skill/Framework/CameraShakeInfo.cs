using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Defines basic parameters for shaking camera
    /// </summary>    
    [Serializable]
    public class CameraShakeInfo
    {
        /// <summary> Intensity of shake
        /// x in left/right direction 
        /// y in up/down direction
        /// z in back/forward (zoom) direction
        /// </summary>
        public Vector3 Intensity = new Vector3(0.1f, 0.1f, 0.0f);

        /// <summary> Intensity of camera roll</summary>
        public float Roll = 0.1f;
        /// <summary> Duration of shake </summary>
        public float Duration = 0.1f;
        /// <summary> Max distance to camera. the shake will be more stronger by distance to camera </summary>
        public float Range = 50;

        /// <summary> Default constructor </summary>
        public CameraShakeInfo() { }

        /// <summary>
        /// Create a copy of CameraShakeInfo
        /// </summary>
        /// <param name="other">Other CameraShakeInfo to copy</param>
        public CameraShakeInfo(CameraShakeInfo other)
        {
            this.Intensity = other.Intensity;            
            this.Roll = other.Roll;
            this.Duration = other.Duration;
            this.Range = other.Range;
        }
    }

    /// <summary>
    /// containing CameraShake event data.
    /// </summary>
    public class CameraShakeEventArgs : EventArgs
    {
        /// <summary> Shake information </summary>
        public CameraShakeInfo Shake { get; private set; }

        /// <summary> Source position of shake </summary>
        public Vector3 Source { get; private set; }

        /// <summary>
        /// Create CameraShakeArgs
        /// </summary>
        /// <param name="shake"> Shake information </param>
        /// <param name="source">Source of shake</param>
        public CameraShakeEventArgs(CameraShakeInfo shake, Vector3 source)
        {
            this.Shake = shake;
            this.Source = source;
        }
    }

    /// <summary>
    /// Handle CameraShake
    /// </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="args"> a CameraShakeArgs containing shake information </param>
    public delegate void CameraShakeEventHandler(object sender, CameraShakeEventArgs args);
}

