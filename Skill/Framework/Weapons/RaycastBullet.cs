using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// A very fast bullet and goes in a straight direction. this bullet check collisions by doing a raycast in travelled distance in current frame.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Bullets/Raycast")]
    public class RaycastBullet : StraightLineBullet
    {

        /// <summary>
        /// Whether weapon check hit posint of this bullet at spawn time or let bullet check hits itself.
        /// </summary>
        public override bool HitAtSpawn { get { return false; } }

        private Vector3 _PrePosition;
        private Ray _Ray;
        private RaycastHit _Hit;


        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _PrePosition = _Transform.position;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Time.timeScale == 0) return;
            // update to move bullet forward
            base.Update();

            _Ray.direction = Direction;
            _Ray.origin = _PrePosition;
            float distance = Vector3.Distance(_Transform.position, _PrePosition);

            if (Physics.Raycast(_Ray, out _Hit, distance, LayerMask))
            {
                EventManager events = _Hit.collider.GetComponent<EventManager>();
                if (events != null)
                {
                    RaycastHitEventArgs hitInfo = CreateHitInfo(_Hit.collider);
                    hitInfo.Normal = _Hit.normal;
                    hitInfo.Point = _Hit.point;

                    // set distance of bullet to real value travelled by bullet
                    _Hit.distance = TravelledDistance - (distance - _Hit.distance);
                    hitInfo.RaycastHit = _Hit;

                    events.OnHit(this, hitInfo);
                }

                OnDie();
            }

            _PrePosition = _Transform.position;
        }

        /// <summary>
        /// Create a default RaycastHitInfo. subclass can override this method to create a new type of RaycastHitInfo
        /// </summary>
        /// <param name="other">The collider that this bullet hits with</param>
        /// <returns> RaycastHitInfo </returns>
        protected virtual RaycastHitEventArgs CreateHitInfo(UnityEngine.Collider other)
        {
            RaycastHitEventArgs info = new RaycastHitEventArgs(Shooter, HitType.Bullet | HitType.Raycast, other);
            info.Damage = Damage;
            info.Tag = this.tag;
            return info;
        }
    }
}
