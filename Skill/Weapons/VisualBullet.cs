using Skill.Managers;
using System;
using UnityEngine;

namespace Skill.Weapons
{
    /// <summary>
    /// A usually very fast bullet goes in a straight direction, but don't hit any object . Infact this is just a visual of bullet.    
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Bullets/Visual")]
    public class VisualBullet : Bullet
    {
        /// <summary>
        /// Layers to raycast. (should be setted by weapon)
        /// </summary>
        public int LayerMask { get; set; }

        /// <summary>
        /// Speed of bullet in air. (should be setted by weapon)
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// Direction of bullet in air. (should be setted by weapon)
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Range of bullet after spawn. (should be setted by weapon)
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        /// The travelled distance of bullet after spawn.
        /// </summary>
        public float TravelledDistance { get { return _TravelledDistance; } }

        private float _TravelledDistance;

        /// <summary>
        /// Whether weapon check hit posint of this bullet at spawn time or let bullet check hits itself.
        /// </summary>
        public virtual bool HitAtSpawn { get { return true; } }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _TravelledDistance = 0;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Time.timeScale == 0.0f) return;
            float move = Speed * Time.deltaTime;
            _Transform.position += Direction * move;
            _TravelledDistance += move;

            if (_TravelledDistance >= Range)
            {
                CacheSpawner.DestroyCache(gameObject);
                enabled = false;
            }
            base.Update();
        }
    }
}
