using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Trigger to change IsometricCamera
    /// </summary> 
    public class CameraLimitPlaneTrigger : Trigger
    {
        public LimitPlane[] Limits;


        private static IsometricCameraTrigger _LastTrigger;
        /// <summary>
        /// On enter trigger
        /// </summary>
        /// <param name="other"> other collider</param>
        /// <returns>true if trigger accepted, otherwise false</returns>
        protected override bool OnEnter(UnityEngine.Collider other)
        {
            if (CameraLimitPlane.Instance != null)
            {
                CameraLimitPlane.Instance.Limits = this.Limits;
                return true;
            }
            return false;
        }

    }
}
