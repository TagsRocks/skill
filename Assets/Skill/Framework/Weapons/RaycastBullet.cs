using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// A very fast bullet and goes in a straight direction. this bullet check collisions by doing a raycast in travelled distance in current frame.
    /// </summary>    
    public class RaycastBullet : StraightLineBullet
    {                        
        private Ray _Ray;
        private RaycastHit _Hit;
        

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;

            Vector3 prePosition = _Transform.position;
            // update to move bullet forward
            base.Update();

            _Ray.direction = Direction;
            _Ray.origin = prePosition;
            float distance = Vector3.Distance(_Transform.position, prePosition);

            if (distance > Mathf.Epsilon)
            {
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

                        events.RaiseHit(this, hitInfo);
                    }

                    OnDie(_Hit.collider);
                }
            }
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
            info.DamageType = DamageType;
            info.Tag = this.tag;
            return info;
        }        
    }
}
