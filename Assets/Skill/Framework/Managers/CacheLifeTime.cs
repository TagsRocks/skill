using UnityEngine;
using System.Collections;

namespace Skill.Framework.Managers
{

    /// <summary>
    /// After specific time CacheObject automatically cached
    /// </summary>        
    public class CacheLifeTime : DynamicBehaviour
    {
        /// <summary> Life Time </summary>
        public float LifeTime = 5;
        /// <summary> cache if renderer is invisible to camera </summary>
        public bool CacheInvisible = false;

        private TimeWatch _LifeTimeTW;
        private Renderer _Renderer;


        protected override void GetReferences()
        {
            base.GetReferences();
            _Renderer = GetComponent<Renderer>();
            if (_Renderer == null)
                _Renderer = GetComponentInChildren<Renderer>();
        }

        /// <summary>
        /// On Enable
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _LifeTimeTW.Begin(LifeTime);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (_LifeTimeTW.IsEnabledAndOver)
            {
                if (_Renderer != null && CacheInvisible)
                {
                    if (_Renderer.isVisible)
                        _LifeTimeTW.Begin(0.5f);
                    else
                        Cache.DestroyCache(this.gameObject);

                }
                else
                    Cache.DestroyCache(this.gameObject);
            }
            base.Update();
        }
    }

}