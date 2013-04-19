using UnityEngine;
using System.Collections;

namespace Skill.Framework.Managers
{
    /// <summary>
    /// Group of CacheObjects for better management
    /// </summary>
    [AddComponentMenu("Skill/Managers/CacheGroup")]
    public class CacheGroup : DynamicBehaviour
    {        
        /// <summary> CacheObjects </summary>
        public CacheObject[] Caches;
        /// <summary> Clean Interval of this group</summary>
        public float CleanInterval = 20;

        private TimeWatch _CleanTW;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            // Loop through the caches
            for (var i = 0; i < Caches.Length; i++)
            {
                // Initialize each cache
                Caches[i].Initialize(this);
            }

            enabled = CleanInterval > 0;
        }

        /// <summary>
        /// On Destroy
        /// </summary>
        protected override void OnDestroy()
        {
            if (Caches != null)
            {
                foreach (var item in Caches)
                {
                    if (item != null)
                        item.Destroy();
                }
            }
            base.OnDestroy();
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (CleanInterval > 0)
            {
                if (_CleanTW.IsEnabled)
                {
                    if (_CleanTW.IsOver)
                    {
                        foreach (var item in Caches)
                        {
                            if (item != null)
                                item.Clean();
                        }
                        _CleanTW.End();
                    }
                }
                else
                    _CleanTW.Begin(CleanInterval);
            }            
            base.Update();
        }
    }

}