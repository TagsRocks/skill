using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework
{
    /// <summary>
    /// Defines types hit
    /// </summary>
    public enum HitType
    {
        /// <summary> None </summary>
        None = 0,
        /// <summary> hit caused by a bullet </summary>
        Bullet = 1,
        /// <summary> hit caused by a raycast </summary>
        Raycast = 2,
        /// <summary> hit caused by colliding another object</summary>
        Collision = 4,
        /// <summary> user define hit 1  </summary>
        Hit1 = 8,
        /// <summary> user define hit 2  </summary>
        Hit2 = 16,
        /// <summary> user define hit 3  </summary>
        Hit3 = 32,
        /// <summary> user define hit 4  </summary>
        Hit4 = 64,
        /// <summary> user define hit 5  </summary>
        Hit5 = 128
    }

    /// <summary>
    /// Defines information about when an object hits
    /// </summary>
    public class HitEventArgs : EventArgs
    {
        /// <summary> Type of hit </summary>
        public HitType Type { get; private set; }
        /// <summary> The object that caused this hit </summary>
        public GameObject Hitter { get; private set; }
        /// <summary> Collider </summary>
        public UnityEngine.Collider Collider { get; private set; }
        /// <summary> User data </summary>
        public System.Object UserData { get; set; }

        /// <summary> Tag </summary>
        public string Tag;
        /// <summary> Position of hit </summary>
        public Vector3 Point;
        /// <summary> Normal of hit </summary>
        public Vector3 Normal;
        /// <summary> Amount of damage imposed by this hit </summary>
        public float Damage;
        /// <summary> Whether this hit cause particle on colliding object?</summary>
        /// <remarks>
        /// Maybe you don't want an explosion cause to particles spawns.
        /// </remarks>
        public bool CauseParticle = true;

        /// <summary>
        /// Create a HitInfo
        /// </summary>
        /// <param name="owner"> The object that caused this hit </param>
        /// <param name="type"> Type of hit </param>
        /// <param name="collider"> Collider </param>       
        public HitEventArgs(GameObject owner, HitType type, UnityEngine.Collider collider)
        {
            this.Type = type;
            this.Hitter = owner;
            this.Collider = collider;            
        }
    }

    /// <summary>
    /// Defines information about when an object hits by raycast
    /// </summary>
    public class RaycastHitEventArgs : HitEventArgs
    {
        /// <summary>
        /// Raycast hit information
        /// </summary>
        public RaycastHit RaycastHit;

        /// <summary>
        /// Create a RaycastHitInfo
        /// </summary>
        /// <param name="owner"> The object that caused this hit </param>
        /// <param name="type"> Type of hit </param>
        /// <param name="other"> Other collider </param>       
        public RaycastHitEventArgs(GameObject owner, HitType type, UnityEngine.Collider other)
            : base(owner, type, other)
        {
        }
    }

    /// <summary>
    /// Defines information about when an object hits by another object
    /// </summary>
    public class CollisionHitEventArgs : HitEventArgs
    {
        /// <summary>
        /// Collision hit information
        /// </summary>
        public Collision CollisionInfo;

        /// <summary>
        /// Create a CollisionHitInfo
        /// </summary>
        /// <param name="owner"> The object that caused this hit </param>
        /// <param name="type"> Type of hit </param>
        /// <param name="other"> Other collider </param>       
        public CollisionHitEventArgs(GameObject owner, HitType type, UnityEngine.Collider other)
            : base(owner, type, other)
        {
        }
    }    
    /// <summary>
    /// containing damage event data.
    /// </summary>
    public class DamageEventArgs : EventArgs
    {
        /// <summary> Amount of damage </summary>
        public float Damage { get; private set; }

        /// <summary> Tag of object that cause damage </summary>
        public string Tag { get; private set; }

        /// <summary> User Data </summary>
        public object UserData { get; set; }

        /// <summary>
        /// Create DamageEventArgs
        /// </summary>
        /// <param name="damage"> Amount of damage </param>
        /// <param name="tag"> tag of object that caused damage</param>
        public DamageEventArgs(float damage, string tag)
        {
            this.Damage = damage;
            this.Tag = tag;
        }
    }
}