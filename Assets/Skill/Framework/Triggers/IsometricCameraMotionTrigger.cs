using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Trigger to change IsometricCamera
    /// </summary> 
    public class IsometricCameraMotionTrigger : Trigger
    {
        /// <summary> reference to IsometricCameraMotion attached to camera </summary>
        public IsometricCameraMotion Motion;

        /// <summary> Apply parameters relative to current parameters of camera or set parameters absolutely </summary>
        public bool Relative = false;
        /// <summary> faild of view </summary>
        public float Fov = 60;
        /// <summary> Rotation angle around target ( 0 - 360) </summary>
        public float AroundAngle = 0;
        /// <summary> Rotation angle behind target( 0 - 90). 0 is completely horizontal to target and 90 is completely vertical to target. </summary>
        public float LookAngle = 0;
        /// <summary> length of motion to reach this values</summary>
        public float MotionTime = 0.5f;

        private static IsometricCameraMotionTrigger _LastTrigger;

        /// <summary>
        /// On enter trigger
        /// </summary>
        /// <param name="other"> other collider</param>
        /// <returns>true if trigger accepted, otherwise false</returns>
        protected override bool OnEnter(UnityEngine.Collider other)
        {
            if (Motion != null)
            {
                if (_LastTrigger != this)
                {
                    _LastTrigger = this;
                    Apply();
                }                
                return true;
            }
            else
            {
                Debug.LogError("Miisin reference to IsometricCameraMotion");
            }
            return false;
        }

        public void Apply()
        {
            if (Relative)
                Motion.MotionDelta(Fov, AroundAngle, LookAngle, MotionTime);
            else
                Motion.Motion(Fov, AroundAngle, LookAngle, MotionTime);
        }
    }
}
