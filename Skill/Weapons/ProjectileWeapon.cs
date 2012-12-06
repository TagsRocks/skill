using Skill.Managers;
using System;
using UnityEngine;

namespace Skill.Weapons
{
    /// <summary>
    /// A weapon that uses rigidbody to shoot bullets. bullets of this weapon uses gravity.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Projectile")]
    public class ProjectileWeapon : BaseWeapon
    {
        /// <summary> Angle relative to direction to throw bullets. </summary>
        public float ThrowAngle = 45;

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="consummedAmmo">Amount of consumed ammo</param>
        protected override Bullet ShootBullet(int consummedAmmo)
        {
            // spawn a bullet but inactive
            GameObject go = CacheSpawner.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, CurrentProjectile.BulletPrefab.transform.rotation) as GameObject;

            if (Controller.rigidbody != null)
                Physics.IgnoreCollision(Controller.rigidbody.collider, go.collider);
            if (this.rigidbody != null)
                Physics.IgnoreCollision(this.rigidbody.collider, go.collider);

            Rigidbody rb = go.rigidbody;
            if (rb != null)
            {
                float range = CurrentProjectile.Range;
                Vector3 dir = _Transform.forward;
                dir.y = 0;

                if (Target != null)
                {
                    Vector3 target = Target.position;
                    Vector3 position = _Transform.position;
                    dir = target - position;
                    dir.y = 0;
                    range = dir.magnitude;
                    if (range > CurrentProjectile.Range)
                        range = CurrentProjectile.Range;
                }

                dir.Normalize();
                Quaternion rotation = Quaternion.AngleAxis(ThrowAngle, dir);

                float speed = (Physics.gravity * range).sqrMagnitude;
                dir = rotation * Vector3.forward;
                rb.AddForce(dir * speed * rb.mass, ForceMode.Impulse);
            }
            Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet
            if (bullet != null) // set bullet parameters
            {
                bullet.Shooter = Controller.gameObject;
                bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
            }
            return bullet;
        }
    }
}
