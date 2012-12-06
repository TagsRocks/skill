using System;
using UnityEngine;
using Skill.Managers;

namespace Skill.Weapons
{
    /// <summary>
    /// This type of bullet has lower speed and after spawn will controlled by physics engine. it is possible to enable gravity to fall down after spawn or
    /// disable gravity to go in straight direction. The weapon do AddForce to bullet at spawn time.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Bullets/Projectile")]
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBullet : Bullet
    {
        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        /// </summary>
        /// <param name="collision"> Collision information</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!IsDestroyed)
            {
                if (!IsCollisionAccepted(collision)) return;
                ContactPoint cp = collision.contacts[0];
                EventManager eventManager = cp.otherCollider.GetComponent<EventManager>();
                if (eventManager != null)
                {
                    CollisionHitInfo hitInfo = CreateHitInfo(cp.otherCollider);
                    hitInfo.Normal = cp.normal;
                    hitInfo.Point = cp.point;
                    hitInfo.CollisionInfo = collision;

                    eventManager.OnHit(this, new HitEventArgs(hitInfo));
                }
                enabled = false;
                CacheSpawner.DestroyCache(gameObject);
            }
        }

        /// <summary>
        /// Allow subclass to filter collsions. it is possible to filter collisions by tags based on whether this bullet shooted by player/friend or
        /// shooted by an enemy.
        /// </summary>
        /// <param name="collision">Collision</param>
        /// <returns>True if accepted, false for rejected</returns>
        protected virtual bool IsCollisionAccepted(Collision collision)
        {
            return true;
        }

        /// <summary>
        /// Create a default CollisionHitInfo. subclass can override this method to create a new type of CollisionHitInfo
        /// </summary>
        /// <param name="other">The collider that this bullet hits with</param>
        /// <returns> CollisionHitInfo </returns>
        protected virtual CollisionHitInfo CreateHitInfo(UnityEngine.Collider other)
        {
            CollisionHitInfo info = new CollisionHitInfo(Shooter, HitType.Bullet | HitType.Collision, other);
            info.Damage = Damage;
            info.Tag = this.tag;
            return info;
        }
    }
}
