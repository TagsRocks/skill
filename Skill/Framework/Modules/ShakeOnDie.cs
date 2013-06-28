using System;
using UnityEngine;


namespace Skill.Framework.Modules
{
    /// <summary>
    /// Call Global.OnCameraShake event OnDie
    /// </summary>
    [AddComponentMenu("Skill/Modules/ShakeOnDie")]
    [RequireComponent(typeof(EventManager))]
    public class ShakeOnDie : StaticBehaviour
    {
        /// <summary> Shake parameter </summary>
        public CameraShakeInfo Shake;

        /// <summary>
        /// Hook required events
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if (Events != null)
                Events.Die += Events_Die;
        }

        /// <summary>
        /// Unhook hooked events
        /// </summary>
        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (Events != null)
                Events.Die -= Events_Die;
        }

        /// <summary>
        /// Hooked to Die event of EventManager
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e"> EventArgs </param>
        protected virtual void Events_Die(object sender, EventArgs e)
        {
            Global.OnCameraShake(this, Shake, _Transform.position);
        }
    }
}
