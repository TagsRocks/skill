using UnityEngine;
using System.Collections;

namespace Skill.Managers
{
    /// <summary>
    /// Group of CacheObjects for better management
    /// </summary>
    [AddComponentMenu("Skill/Managers/CacheGroup")]
    public class CacheGroup : MonoBehaviour
    {
        /// <summary> CacheObjects </summary>
        public CacheObject[] Caches;
        /// <summary> Clean Interval of this group</summary>
        public float CleanInterval = 20;

        private Skill.TimeWatch _CleanTW;

        /// <summary>
        /// Awake
        /// </summary>
        protected void Awake()
        {
            // Loop through the caches
            for (var i = 0; i < Caches.Length; i++)
            {
                // Initialize each cache
                Caches[i].Initialize(this);
            }
        }

        /// <summary>
        /// On Destroy
        /// </summary>
        protected void OnDestroy()
        {
            if (Caches != null)
            {
                foreach (var item in Caches)
                {
                    if (item != null)
                        item.Destroy();
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        protected void Update()
        {
            if (CleanInterval > 0)
            {
                if (_CleanTW.Enabled)
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
        }
    }

}