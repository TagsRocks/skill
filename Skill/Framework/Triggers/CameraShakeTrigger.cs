using UnityEngine;
using System.Collections;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Shake camera on trigger enter
    /// </summary>
    [AddComponentMenu("Game/Triggers/CameraShake")]
    public class CameraShakeTrigger : Trigger
    {
        /// <summary>
        /// Camera shake config
        /// </summary>
        public CameraShakeInfo Shake;

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        /// <returns>True if event handled, otherwise false</returns>
        protected override bool OnEnter(Collider other)
        {
            Skill.Framework.Global.OnCameraShake(this, Shake, transform.position);
            return true;
        }

        /// <summary>
        /// Name of file in Gizmos folder 
        /// </summary>
        protected override string GizmoFilename { get { return "CameraShake.png"; } }
    }
}