using UnityEngine;
using System.Collections;

namespace Skill.Framework.Managers
{

    /// <summary>
    /// After specific time CacheObject automatically cached
    /// </summary>
    [RequireComponent(typeof(CacheBehavior))]
    [AddComponentMenu("Skill/Managers/CacheLifeTime")]
    public class CacheLifeTime : DynamicBehaviour
    {
        /// <summary>
        /// Life Time
        /// </summary>
        public float LifeTime = 5;

        /// <summary>
        /// Whether life time timer begins on awake
        /// </summary>
        public bool EnableOnAwake = true;

        private TimeWatch _LifeTimeTW;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            enabled = EnableOnAwake;
        }

        /// <summary>
        /// On Enable
        /// </summary>
        protected virtual void OnEnable()
        {
            _LifeTimeTW.Begin(LifeTime);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (_LifeTimeTW.EnabledAndOver)
            {
                CacheSpawner.DestroyCache(this.gameObject);
            }
            base.Update();
        }
    }

}