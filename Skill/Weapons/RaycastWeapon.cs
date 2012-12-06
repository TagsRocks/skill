using Skill.Managers;
using System;
using UnityEngine;

namespace Skill.Weapons
{
    /// <summary>
    /// A weapon that uses raycasting to shoot bullets and needs visual or raycast bullets.
    /// If Target of weapon is valid it shoots on Target, otherwise shoots in front direction.
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Raycast")]
    public class RaycastWeapon : DirectWeapon
    {
        /// <summary> Layer mask to use in raycasting </summary>
        public int LayerMask = 0xFFFFFFF;        
        
        private RaycastHit _HitInfo;
        private Ray _Ray;                

        /// <summary>
        /// Instantiate a bullet and throw it at correct direction and force 
        /// </summary>
        /// <param name="consummedAmmo">Amount of consumed ammo</param>
        protected override Bullet ShootBullet(int consummedAmmo)
        {
            // spawn a bullet but inactive
            GameObject go = CacheSpawner.Spawn(CurrentProjectile.BulletPrefab, CurrentProjectile.SpawnPoint.position, BulletRotation, false) as GameObject;
            VisualBullet bullet = go.GetComponent<VisualBullet>();// get reference to bullet

            if (bullet != null) // set bullet parameters
            {
                bullet.Shooter = Controller.gameObject;
                bullet.Damage = CurrentProjectile.InstantHitDamage * DamageFactor;
                bullet.Direction = BulletDirection;
                bullet.Range = CurrentProjectile.Range;
                bullet.Speed = CurrentProjectile.InitialSpeed;
                bullet.LayerMask = LayerMask;

                if (bullet.HitAtSpawn) // if is is needed to check hit at spawn time
                {
                    _Ray.direction = BulletDirection;
                    _Ray.origin = CurrentProjectile.SpawnPoint.position;

                    if (Physics.Raycast(_Ray, out _HitInfo, CurrentProjectile.Range, LayerMask))
                    {
                        bullet.Range = _HitInfo.distance;

                        EventManager events = _HitInfo.collider.GetComponent<EventManager>();
                        if (events != null)
                        {
                            RaycastHitInfo info = new RaycastHitInfo(bullet.Shooter, HitType.Bullet | HitType.Raycast, _HitInfo.collider);
                            info.Damage = bullet.Damage;
                            info.Tag = this.tag;
                            info.Normal = _HitInfo.normal;
                            info.Point = _HitInfo.point;
                            info.RaycastHit = _HitInfo;
                            events.OnHit(this, new HitEventArgs(info));
                        }
                    }
                }
            }

            go.SetActive(true);
            return bullet;
        }
    }
}
