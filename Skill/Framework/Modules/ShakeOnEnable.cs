using System;
using UnityEngine;


namespace Skill.Framework.Modules
{
    /// <summary>
    /// Call Global.OnCameraShake event OnEnable. this is usefull for explisions to shake camera
    /// </summary>
    [AddComponentMenu("Skill/Modules/ShakeOnEnable")]
    public class ShakeOnEnable : StaticBehaviour
    {
        /// <summary> Shake parameter </summary>
        public CameraShakeInfo Shake;

        /// <summary> Ignore first OnEnable </summary>
        /// <remarks>
        /// When a GameObject instantiated OnEnable method will called.
        /// if use this behavior with a cachable object, and cache objects instantiated before use,
        /// so it is better to ignore first enable
        /// </remarks>
        public bool IgnoreFirst = true;

        private int _EnableCount = 0;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (IgnoreFirst && _EnableCount < 1)
            {
                _EnableCount++;
            }
            else
                Global.OnCameraShake(this, Shake, _Transform.position);
        }
    }
}
