using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// A weapon that uses rigidbody to shoot bullets. bullets of this weapon uses gravity.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/ProjectileWP")]
    public class ProjectileWeapon : BaseWeapon
    {
        /// <summary> Angle relative to direction to throw bullets. </summary>
        public float ThrowAngle = 45;

        /// <summary> Rotation of bulet when spawn (default is Identity) </summary>
        public Quaternion InitialBulletRotation { get; set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            InitialBulletRotation = Quaternion.identity;
        }

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="bulletCount">Number of bullet to shoot</param>
        /// <returns> Array of spawned bullets </returns>
        protected override Bullet[] ShootBullets(int bulletCount)
        {
            Bullet[] bullets = new Bullet[bulletCount];

            for (int i = 0; i < bulletCount; i++)
            {
                // spawn a bullet but inactive
                GameObject go = CacheSpawner.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, InitialBulletRotation) as GameObject;

                Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet
                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = Controller.gameObject;
                    bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
                    bullet.Range = CurrentProjectile.Range;
                    bullet.Speed = CurrentProjectile.InitialSpeed;
                }

                Rigidbody rb = go.rigidbody;
                if (rb != null)
                {
                    if (rb.collider != null)
                    {
                        Rigidbody controllerRB = Controller.rigidbody;
                        if (controllerRB != null && controllerRB.collider)
                            Physics.IgnoreCollision(controllerRB.collider, rb.collider);

                        Rigidbody weaponRB = this.rigidbody;
                        if (weaponRB != null && weaponRB.collider)
                            Physics.IgnoreCollision(weaponRB.collider, rb.collider);
                    }
                    float range = CurrentProjectile.Range;
                    Vector3 dir = _Transform.forward;
                    dir.y = 0;

                    if (Target != null && Target.HasValue)
                    {
                        Vector3 target = Target.Value;
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

                    bullet.Direction = dir;
                    bullet.Speed = speed;
                }

                bullets[i] = bullet;
            }
            return bullets;
        }
    }
}
