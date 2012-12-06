using UnityEngine;
namespace Skill.Weapons
{
    /// <summary>
    /// Defines state of weapon
    /// </summary>
    public enum WeaponState
    {
        /// <summary> Weapon is ready to fire </summary>
        Ready,
        /// <summary> Weapon is refill another ammo. infact this is FireInterval between shots</summary>
        Refill,
        /// <summary> Weapon is reloading </summary>
        Reloading,
        /// <summary> Weapon is changing projectile </summary>
        ChangeProjectile
    }
}