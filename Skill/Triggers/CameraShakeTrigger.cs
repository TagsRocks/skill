using UnityEngine;
using System.Collections;

namespace Skill.Triggers
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
        protected override void OnEnter(Collider other)
        {
            Skill.Global.OnCameraShake(this, new CameraShakeEventArgs(Shake, transform.position));
        }

        /// <summary>
        /// Name of file in Gizmos folder 
        /// </summary>
        protected override string GizmoFilename { get { return "CameraShake.png"; } }
    }
}