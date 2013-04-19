using Skill.Framework.Managers;
using System;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// A weapon that uses raycasting to shoot bullets and needs StraightLineBullet.
    /// If Target of weapon is valid it shoots on Target, otherwise shoots in front direction.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/RaycastWP")]
    public class RaycastWeapon : StraightLineWeapon
    {
        /// <summary> Layer mask to use in raycasting </summary>
        public int LayerMask = 0xFFFFFFF;

        private RaycastHit _HitInfo;
        private Ray _Ray;

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
                Quaternion bulletRotation = Quaternion.LookRotation(spreadRot * Direction);
                Vector3 bulletDirection = (bulletRotation * Vector3.forward).normalized;

                // spawn a bullet but inactive
                GameObject go = CacheSpawner.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, bulletRotation, false) as GameObject;
                StraightLineBullet bullet = go.GetComponent<StraightLineBullet>();// get reference to bullet

                if (bullet != null) // set bullet parameters
                {
                    bullet.Shooter = Controller.gameObject;
                    bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
                    bullet.Direction = bulletDirection;
                    bullet.Range = CurrentProjectile.Range;
                    bullet.Speed = CurrentProjectile.InitialSpeed;
                    bullet.LayerMask = LayerMask;

                    if (bullet.HitAtSpawn) // if is is needed to check hit at spawn time
                    {
                        _Ray.direction = bulletDirection;
                        _Ray.origin = CurrentProjectile.SpawnPoint.position;

                        if (Physics.Raycast(_Ray, out _HitInfo, CurrentProjectile.Range, LayerMask))
                        {
                            bullet.Range = _HitInfo.distance;

                            EventManager events = _HitInfo.collider.GetComponent<EventManager>();
                            if (events != null)
                            {
                                RaycastHitEventArgs info = new RaycastHitEventArgs(bullet.Shooter, HitType.Bullet | HitType.Raycast, _HitInfo.collider);
                                info.Damage = bullet.Damage;
                                info.Tag = this.tag;
                                info.Normal = _HitInfo.normal;
                                info.Point = _HitInfo.point;
                                info.RaycastHit = _HitInfo;
                                events.OnHit(this, info);
                            }
                        }
                    }
                }

                go.SetActive(true);
                bullets[i] = bullet;
            }
            return bullets;
        }
    }
}
