using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Managers
{
    
    /// <summary>
    /// Behavior that required for a Cacheable object
    /// </summary>
    [AddComponentMenu("Skill/Managers/CacheBehavior")]
    public sealed class CacheBehavior : MonoBehaviour
    {
        /// <summary> Unique id for all instance of this object </summary>
        public int CacheId { get; set; }
        /// <summary> whether this object collected by CacheSpawner or not </summary>
        public bool IsCollected { get; set; }
        /// <summary> Group </summary>
        public CacheGroup Group { get; set; }        
    }    
}