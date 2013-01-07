using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// A weapon that uses velocity for rigidbody bullets and speed/Direction for none rigidbody bullets to shoot bullets.
    /// The RigidBody of bullets use no gravity to go straight.
    /// If Target of weapon is valid it shoots on Target, otherwise shoots in front direction.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/StraightLineWP")]
    public class StraightLineWeapon : BaseWeapon
    {
        private Vector3 _Direction;
        /// <summary> Retrieves direction of weapon to shoot bullets</summary>
        public Vector3 Direction { get { return _Direction; } }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Time.timeScale == 0) return;

            if (Target != null && Target.HasValue)
                _Direction = (Target.Value - CurrentProjectile.SpawnPoint.position).normalized;
            else
                _Direction = _Transform.forward;

            base.Update();
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
                Quaternion spreadRot = Quaternion.Euler(UnityEngine.Random.Range(-Spread.x, Spread.x), UnityEngine.Random.Range(-Spread.y, Spread.y), 0);
                Quaternion bulletRotation = Quaternion.LookRotation(spreadRot * _Direction);
                Vector3 bulletDirection = (bulletRotation * Vector3.forward).normalized;

                // spawn a bullet but inactive
                GameObject go = CacheSpawner.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, bulletRotation) as GameObject;
                Rigidbody rb = go.rigidbody;
                if (rb != null && !rb.isKinematic)
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

                    if (rb.useGravity)
                    {
                        rb.AddForce(bulletDirection * CurrentProjectile.InitialSpeed, ForceMode.Impulse);
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;
                        rb.AddForce(bulletDirection * CurrentProjectile.InitialSpeed, ForceMode.VelocityChange);
                    }
                }

                Bullet bullet = go.GetComponent<Bullet>();// get reference to bullet
                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = Controller.gameObject;
                    bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
                    bullet.Direction = bulletDirection;
                    bullet.Range = CurrentProjectile.Range;
                    bullet.Speed = CurrentProjectile.InitialSpeed;
                }

                bullets[i] = bullet;
            }
            return bullets;
        }
    }
}
