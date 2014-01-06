using Skill.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Defiens basic behaviour for static components
    /// </summary>
    public abstract class StaticBehaviour : MonoBehaviour
    {
        /// <summary> Host events </summary>
        public EventManager Events { get; private set; }

        /// <summary> Is GameObject destroyed? </summary>
        public bool IsDestroyed { get; private set; }

        /// <summary> a rederence to transform for better performance</summary>
        protected Transform _Transform;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            GetReferences();            
            IsDestroyed = false;
            HookEvents();
        }

        /// <summary>
        /// Hook required events if needed
        /// </summary>
        protected virtual void HookEvents()
        {

        }

        /// <summary>
        /// Unhook hooked events 
        /// </summary>
        protected virtual void UnhookEvents()
        {

        }

        /// <summary>
        /// Get compoenet references
        /// </summary>
        protected virtual void GetReferences()
        {
            _Transform = transform;
            Events = GetComponent<EventManager>();
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        protected virtual void Start()
        {            
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (IsDestroyed) return;            
            UnhookEvents();
            IsDestroyed = true;
        }

        /// <summary>
        /// Destroy game object
        /// </summary>
        public virtual void DestroySelf()
        {            
            Managers.Cache.DestroyCache(this.gameObject);
        }
    }
}
