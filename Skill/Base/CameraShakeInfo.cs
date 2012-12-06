using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// Defines basic parameters for shaking camera
    /// </summary>    
    [Serializable]
    public class CameraShakeInfo
    {
        /// <summary> Intensity of shake in left and right direction </summary>
        public float SideIntensity = 0.1f;
        /// <summary> Intensity of shake in up and Down direction </summary>
        public float UpDownIntensity = 0.0f;
        /// <summary> Intensity of camera roll</summary>
        public float Roll = 0.1f;
        /// <summary> Duration of shake </summary>
        public float Duration = 0.1f;
        /// <summary> Max distance to camera. the shake will be more stronger by distance to camera </summary>
        public float Range = 20;        
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
        public CameraShakeEventArgs(CameraShakeInfo shake , Vector3 source)
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

