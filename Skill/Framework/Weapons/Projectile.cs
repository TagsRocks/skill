using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Weapons
{

    /// <summary>
    /// Type of bullet
    /// </summary>
    public enum ProjectileType
    {
        /// <summary>
        /// A weapon that uses velocity for rigidbody bullets and speed/Direction for none rigidbody bullets to shoot bullets.
        /// The RigidBody of bullets use no gravity to go straight.
        /// If Target of weapon is valid it shoots on Target, otherwise shoots in front direction.
        /// </summary>
        StraightLine,
        /// <summary>        
        /// use raycasting to shoot bullets and needs StraightLineBullet.
        /// If Target of weapon is valid it shoots on Target, otherwise shoots in front direction.        
        /// </summary>
        Raycast,
        /// <summary>
        /// Bullet of this projectile needs rigidbody and use gravity to move.
        /// </summary>
        Curve
    }

    [Serializable]
    public class Projectile
    {
        /// <summary> Name of projectile </summary>
        public string Name;
        /// <summary> where to spawn bullets. usually it is a point in child of weapon that moves by weapon </summary>
        public Transform SpawnPoint;
        /// <summary> A prefab that has a Bullet script component </summary>
        public GameObject BulletPrefab;
        /// <summary> Type of bullet. each type needs deferent parameters and components</summary>
        public ProjectileType Type;
        /// <summary> Sound to play on fire </summary>
        public AudioClip[] FireSounds;
        /// <summary> Sound to play on reload </summary>
        public AudioClip ReloadSound;
        /// <summary> Sound to play on reload when clip is empty and complete reload needed</summary>
        public AudioClip CompleteReloadSound;
        /// <summary> Initial speed of bullet at spawn time </summary>
        public float InitialSpeed = 30;
        /// <summary> </summary>
        public float DamagePerSecond = 20.0f;
        /// <summary> Size of eack clip </summary>
        public int ClipSize = 30;
        /// <summary> Maximum number of ammo count </summary>
        public int MaxAmmo = 270;
        /// <summary> Default ammo count </summary>
        public int DefaultAmmo = 200;
        /// <summary> Range of bullet </summary>
        public float Range = 50;
        /// <summary> How long does it take to switch to this projectile </summary>
        public float EquipTime = 1;
        /// <summary> How long does it take to change a clip </summary>
        public float ReloadTime = 1;
        /// <summary> How long does it take to change a clip when current clip is empty and complete reload needed </summary>
        public float CompleteReloadTime = 1.2f;
        /// <summary> Holds the amount of time a single shot takes </summary>
        public float FireInterval;
        /// <summary> How much damage does a given instanthit shot do </summary>
        public float InstantHitDamage = 20;
        /// <summary> DamageTypes for Instant Hit Weapons </summary>
        public int DamageType = 0;
        /// <summary> whether this projectile has infinite clips( reload happens but no ammo consumes ). </summary>
        public bool InfinitClip = false;
        /// <summary> whether this projectile has infinite ammo.( no reload no consume ammo ).   </summary>
        public bool InfinitAmmo = false;
        /// <summary> Curve projectile specific parameters </summary>
        public CurveProjectileParams CurveParams;
        /// <summary> Layer mask to use in raycasting </summary>
        public int LayerMask = 0xFFFFFFF;
        /// <summary> Whether weapon check hit posint of this bullet at spawn time or let bullet check hits itself.</summary>
        public bool HitAtSpawn;

        /// <summary> Number of ammo in current clip </summary>
        public int ClipAmmo { get; set; }

        /// <summary> Normalized number of ammo in current clip </summary>
        public float NormalizedClipAmmo
        {
            get { return (float)ClipAmmo / ClipSize; }
            set
            {
                ClipAmmo = Mathf.FloorToInt(ClipSize * Mathf.Clamp01(value));
            }
        }

        /// <summary> Total number of ammo without clip ammo </summary>
        public int Ammo { get; set; }
        /// <summary> Total number of ammo </summary>
        public int TotalAmmo { get { return ClipAmmo + Ammo; } }


        internal Vector3 SpawnPosition { get; set; }
    }
}