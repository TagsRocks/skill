using UnityEngine;
using Skill.Managers;

namespace Skill.Weapons
{
    /// <summary>
    /// Defines base class of bullets that spawned by weapons
    /// </summary>    
    public abstract class Bullet : DynamicBehaviour
    {
        /// <summary>
        /// Lift time of bullet after spawn. A time with this value begins OnEnable and destroy gameobject when timer end. ( zero or negative for infinit lift time)
        /// </summary>
        public float LifeTime = 1.5f;

        /// <summary>
        /// The GameObject that shoots this bullet. it can be a Weapon, Controller, ...
        /// </summary>
        public UnityEngine.GameObject Shooter { get; set; }

        /// <summary>
        /// Amount of damage caused by this bullet.
        /// </summary>
        public float Damage { get; set; }

        private Skill.TimeWatch _LifeTimeTW;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (LifeTime > 0)
                _LifeTimeTW.Begin(LifeTime);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            Shooter = null;
            Damage = 0;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (_LifeTimeTW.EnabledAndOver)
            {
                CacheSpawner.DestroyCache(gameObject);
                enabled = false;
            }
            base.Update();
        }

        
    }
}