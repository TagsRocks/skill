using UnityEngine;
using System.Collections;

namespace Skill.Managers
{
    [AddComponentMenu("Skill/Managers/CacheGroup")]
    public class CacheGroup : MonoBehaviour
    {
        public CacheObject[] Caches;
        public float CleanInterval = 20;

        private Skill.TimeWatch _CleanTW;

        void Awake()
        {
            // Loop through the caches
            for (var i = 0; i < Caches.Length; i++)
            {
                // Initialize each cache
                Caches[i].Initialize(this);
            }
        }

        void OnDestroy()
        {
            if (Caches != null)
            {
                foreach (var item in Caches)
                {
                    item.Destroy();
                }
            }
        }

        void Update()
        {
            if (CleanInterval > 0)
            {
                if (_CleanTW.Enabled)
                {
                    if (_CleanTW.IsOver)
                    {
                        foreach (var item in Caches)
                        {
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