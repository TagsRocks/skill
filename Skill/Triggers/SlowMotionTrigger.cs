using UnityEngine;
using System.Collections;

namespace Skill.Triggers
{
    [AddComponentMenu("Game/Triggers/SlowMotion")]
    public class SlowMotionTrigger : Trigger
    {
        /// <summary>
        /// Slow motion config
        /// </summary>
        public SlowMotionInfo Motion;

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnEnter(Collider other)
        {
            if (Motion.SlowMotion > 0)
               Skill.Global.OnSlowMotion(this, new SlowMotionEventArgs(Motion));
        }

        /// <summary>
        /// Name of file in Gizmos folder 
        /// </summary>
        protected override string GizmoFilename { get { return "SlowMotion.png"; } }
    }
}