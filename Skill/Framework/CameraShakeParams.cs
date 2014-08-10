using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Defines basic parameters for shaking camera
    /// </summary>    
    [Serializable]
    public class CameraShakeParams
    {
        /// <summary> Is shake enable? </summary>
        public bool Enable = true;
        /// <summary> Intensity of shake
        /// x in left/right direction 
        /// y in up/down direction
        /// z in back/forward (zoom) direction
        /// </summary>
        public Vector3 Intensity = new Vector3(0.05f, 0.08f, 0.0f);

        /// <summary> Intensity of camera roll</summary>
        public float Roll = 0.5f;
        /// <summary> Duration of shake </summary>
        public float Duration = 1.0f;
        /// <summary> Max distance to camera. </summary>
        public float Range = 50;
        /// <summary> The shake will be more stronger near to camera </summary>
        public bool ByDistance = true;
        /// <summary> time between shake directions </summary>
        public float TickTime = 0.08f;

        /// <summary> Default constructor </summary>
        public CameraShakeParams() { }

        /// <summary>
        /// Create a copy of CameraShakeInfo
        /// </summary>
        /// <param name="other">Other CameraShakeInfo to copy</param>
        public CameraShakeParams(CameraShakeParams other)
        {
            CopyFrom(other);
        }

        /// <summary>
        /// Copy parameters from another object
        /// </summary>
        /// <param name="other">CameraShakeInfo to copy parameters</param>
        public void CopyFrom(CameraShakeParams other)
        {
            this.Intensity = other.Intensity;
            this.Roll = other.Roll;
            this.Duration = other.Duration;
            this.Range = other.Range;
            this.ByDistance = other.ByDistance;
            this.TickTime = other.TickTime;
        }
    }

    /// <summary>
    /// containing CameraShake event data.
    /// </summary>
    public class CameraShakeEventArgs : EventArgs
    {
        /// <summary> Shake information </summary>
        public CameraShakeParams Shake { get; private set; }

        /// <summary> Source position of shake </summary>
        public Vector3 Source { get; private set; }

        /// <summary>
        /// Create CameraShakeArgs
        /// </summary>
        /// <param name="shake"> Shake information </param>
        /// <param name="source">Source of shake</param>
        public CameraShakeEventArgs(CameraShakeParams shake, Vector3 source)
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

