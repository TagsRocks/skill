using UnityEngine;
using System.Collections;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Shake camera on trigger enter
    /// </summary>    
    public class CameraShakeTrigger : Trigger
    {
        /// <summary>
        /// Camera shake config
        /// </summary>
        public CameraShakeParams Shake;

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        /// <returns>True if event handled, otherwise false</returns>
        protected override bool OnEnter(Collider other)
        {
            Skill.Framework.Global.RaiseCameraShake(this, Shake, transform.position);
            return true;
        }
        
    }
}