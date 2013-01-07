using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Weapons
{
    [Serializable]
    public class Projectile
    {
        /// <summary> Name of projectile </summary>
        public string Name;
        /// <summary> where to spawn bullets. usually it is a point in child of weapon that moves by weapon </summary>
        public Transform SpawnPoint;        
        /// <summary> A prefab that has a Bullet script component </summary>
        public GameObject BulletPrefab;        
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
        /// <summary> whether this projectile has infinite clips </summary>
        public bool InfinitClip = false;
        /// <summary> whether this projectile has infinite ammo </summary>
        public bool InfinitAmmo = false;
        
        /// <summary> Number of ammo in current clip </summary>
        public int ClipAmmo { get; set; }
        /// <summary> Total number of ammo without clip ammo </summary>
        public int TotalAmmo { get; set; }        
    }
}