using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Managers
{

    /// <summary>
    /// Behavior that required for a Cacheable object
    /// </summary>    
    /// <remarks>
    /// when an object is cached, it instantiated at scene start but disabled, and because unity call Awake and OnEnable method right after instantiation, sometimes
    /// it is better to set cachable prefab to be deactivated. for example if there is a "ShakeOnEnable" component on GameObject when you play game you can see a shake camera
    /// happens when game started, it is because OnEnable of "ShakeOnEnable" was called at initialize time. if you set GameObject to be deactivated in prefab, initialize time of cachable object
    /// do not shake camera.
    /// Oo... do not bother yourself to understand my weak english(:D) just "select prefab and uncheck the checkbox at top left corner of inspector"
    /// </remarks>
    public sealed class CacheBehavior : MonoBehaviour
    {
        /// <summary> Unique id for all instance of this object </summary>
        public int CacheId { get; set; }
        /// <summary> whether this object collected by CacheSpawner or not </summary>
        public bool IsCached { get; set; }
        /// <summary> Group </summary>
        public CacheGroup Group { get; set; }
    }
}