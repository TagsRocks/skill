using Skill.Managers;
using System;
using UnityEngine;

namespace Skill.Weapons
{
    /// <summary>
    /// A weapon that uses rigidbody to shoot bullets. The RigidBody of bullets use no gravity.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Launcher")]
    public class Launcher : DirectWeapon
    {
        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="consummedAmmo">Amount of consumed ammo</param>
        protected override Bullet ShootBullet(int consummedAmmo)
        {
            // spawn a bullet but inactive
            GameObject go = CacheSpawner.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, BulletRotation) as GameObject;

            if (Controller.rigidbody != null)
                Physics.IgnoreCollision(Controller.rigidbody.collider, go.collider);
            if (this.rigidbody != null)
                Physics.IgnoreCollision(this.rigidbody.collider, go.collider);

            Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet

            if (bullet != null) // set bullet parameters
            {
                bullet.Shooter = Controller.gameObject;
                bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;                
            }

            Rigidbody rb = go.rigidbody;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                Vector3 dir = BulletDirection;
                rb.AddForce(dir * CurrentProjectile.InitialSpeed, ForceMode.VelocityChange);
            }

            return bullet;
        }
    }
}
