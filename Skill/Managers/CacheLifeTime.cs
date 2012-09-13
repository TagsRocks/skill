using UnityEngine;
using System.Collections;

namespace Skill.Managers
{

    [RequireComponent(typeof(CacheBehavior))]
    [AddComponentMenu("Skill/Managers/CacheLifeTime")]
    public class CacheLifeTime : MonoBehaviour
    {
        public float LifeTime = 5;

        private Skill.TimeWatch _LifeTimeTW;
        void OnEnable()
        {
            _LifeTimeTW.Begin(LifeTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (_LifeTimeTW.EnabledAndOver)
            {
                CacheSpawner.DestroyCache(this.gameObject);
            }
        }
    }

}