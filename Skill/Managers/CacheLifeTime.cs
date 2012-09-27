using UnityEngine;
using System.Collections;

namespace Skill.Managers
{

    /// <summary>
    /// After specific time CacheObject automatically cached
    /// </summary>
    [RequireComponent(typeof(CacheBehavior))]
    [AddComponentMenu("Skill/Managers/CacheLifeTime")]
    public class CacheLifeTime : MonoBehaviour
    {
        /// <summary>
        /// Life Time
        /// </summary>
        public float LifeTime = 5;

        /// <summary>
        /// Whether life time timer begins on awake
        /// </summary>
        public bool EnableOnAwake = true;

        private Skill.TimeWatch _LifeTimeTW;

        /// <summary>
        /// Awake
        /// </summary>
        protected void Awake()
        {
            enabled = EnableOnAwake;
        }

        /// <summary>
        /// On Enable
        /// </summary>
        protected void OnEnable()
        {
            _LifeTimeTW.Begin(LifeTime);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected void Update()
        {
            if (_LifeTimeTW.EnabledAndOver)
            {
                CacheSpawner.DestroyCache(this.gameObject);
            }
        }
    }

}