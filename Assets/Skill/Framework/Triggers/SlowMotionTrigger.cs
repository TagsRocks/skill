using UnityEngine;
using System.Collections;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Slow motion OnTriggerEnter 
    /// </summary>    
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
        /// <returns>True if event handled, otherwise false</returns>
        protected override bool OnEnter(Collider other)
        {
            if (Motion.Duration > 0)
                Skill.Framework.Global.RaiseSlowMotion(this, Motion);
            return true;
        }        
    }
}