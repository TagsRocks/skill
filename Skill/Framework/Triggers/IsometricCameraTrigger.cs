using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Trigger to change IsometricCamera
    /// </summary> 
    public class IsometricCameraTrigger : Trigger
    {
        /// <summary> reference to IsometricCameraMotion attached to camera </summary>
        public IsometricCamera Camera;

        /// <summary> Minimum distance to target when PointOfIntrest is close to target</summary>
        public float ZoomIn = 8;
        /// <summary> Maximum distance to target when PointOfIntrest is far from target</summary>
        public float ZoomOut = 16;
        /// <summary> Maximum distance of PointOfIntrest from target</summary>
        public float MaxOffset = 6;
        /// <summary> If target can move very fast do not allow camera to loose it</summary>
        public bool FastTarget;
        /// <summary> Apply relative custom offset to position of camera </summary>
        public Vector3 CustomOffset = Vector3.zero;

        /// <summary>
        /// On enter trigger
        /// </summary>
        /// <param name="other"> other collider</param>
        /// <returns>true if trigger accepted, otherwise false</returns>
        protected override bool OnEnter(UnityEngine.Collider other)
        {
            if (Camera != null)
            {
                Camera.ZoomIn = this.ZoomIn;
                Camera.ZoomOut = this.ZoomOut;
                Camera.MaxOffset = this.MaxOffset;
                Camera.FastTarget = this.FastTarget;
                Camera.CustomOffset = this.CustomOffset;
                return true;
            }
            else
            {
                Debug.LogError("Miisin reference to IsometricCameraMotion");
            }
            return false;
        }
    }
}
