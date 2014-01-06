using UnityEngine;
using System.Collections;

namespace Skill.Framework.Managers
{

    /// <summary>
    /// After specific time CacheObject automatically cached
    /// </summary>        
    public class CacheLifeTime : DynamicBehaviour
    {
        /// <summary>
        /// Life Time
        /// </summary>
        public float LifeTime = 5;        

        private TimeWatch _LifeTimeTW;        

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
            if (_LifeTimeTW.IsEnabledAndOver)            
                Cache.DestroyCache(this.gameObject);            
            base.Update();
        }
    }

}