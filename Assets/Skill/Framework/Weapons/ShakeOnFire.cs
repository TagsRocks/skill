using System;
using UnityEngine;


namespace Skill.Framework.Weapons
{
    /// <summary>
    /// Call Global.OnCameraShake event OnEnable. this is usefull for explisions to shake camera
    /// </summary>
    ///<remarks>
    /// When a GameObject instantiated OnEnable method will called.
    /// if use this behavior with a cachable object, and cache objects instantiated before use,
    /// so it is better to ignore first enable
    /// so make sure that if you use this behavior and CacheBehavior together, this gameobject must be inside a CacheGroup to work correctly
    /// </remarks>    
    [RequireComponent(typeof(EventManager))]
    [RequireComponent(typeof(Weapon))]
    public class ShakeOnFire : StaticBehaviour
    {
        /// <summary> Shake parameter </summary>
        public CameraShakeParams Shake;

        private Weapon _Wp;

        protected override void GetReferences()
        {
            base.GetReferences();
            _Wp = GetComponent<Weapon>();
        }

        protected override void HookEvents()
        {
            base.HookEvents();
            if (_Wp != null)
                _Wp.Shoot += wp_Shoot;
        }

        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (_Wp != null)
                _Wp.Shoot -= wp_Shoot;
        }

        void wp_Shoot(object sender, WeaponShootEventArgs args)
        {
            Global.RaiseCameraShake(this, Shake, transform.position);
        }

    }
}
