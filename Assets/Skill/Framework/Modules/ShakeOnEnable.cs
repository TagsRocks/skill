using System;
using UnityEngine;


namespace Skill.Framework.Modules
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
    public class ShakeOnEnable : StaticBehaviour
    {
        /// <summary> Shake parameter </summary>
        public CameraShakeParams Shake;

        //private bool _IgnoreFirst;

        //protected override void Awake()
        //{
        //    base.Awake();
        //    _IgnoreFirst = GetComponent<Managers.CacheBehavior>() != null;
        //}

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            //if (_IgnoreFirst)
            //    _IgnoreFirst = false;
            //else
                Global.RaiseCameraShake(this, Shake, _Transform.position);
        }
    }
}
