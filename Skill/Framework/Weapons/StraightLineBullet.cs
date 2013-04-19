using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// This bullet goes in a straight direction.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Bullets/StraightLine")]
    public class StraightLineBullet : Bullet
    {
        /// <summary>
        /// Amount of damage fall of per unit.
        /// </summary>
        public float DamageFallOf = 0;

        /// <summary>
        /// Damage of bullet do not be lower that this do to fallof
        /// </summary>
        public float MinDamage = 0;

        /// <summary>
        /// Layers to raycast. (should be setted by weapon)
        /// </summary>
        public int LayerMask { get; set; }

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
        /// Amount of damage caused by this bullet.
        /// </summary>
        public override float Damage
        {
            get
            {
                if (DamageFallOf > 0)
                    return Mathf.Max(MinDamage, base.Damage - (DamageFallOf * _TravelledDistance));
                else
                    return base.Damage;
            }
            set
            {
                base.Damage = value;
            }
        }

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
                OnDie();
            }
            base.Update();
        }
    }
}
