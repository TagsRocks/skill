using UnityEngine;
using System.Collections;

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
    public class HitInfo
    {
        /// <summary> Type of hit </summary>
        public HitType Type { get; private set; }
        /// <summary> The object that caused this hit </summary>
        public GameObject Owner { get; private set; }
        /// <summary> Other collider </summary>
        public UnityEngine.Collider Other { get; private set; }
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
        /// <param name="other"> Other collider </param>       
        public HitInfo(GameObject owner, HitType type, UnityEngine.Collider other)
        {
            this.Type = type;
            this.Owner = owner;
            this.Other = other;            
        }
    }

    /// <summary>
    /// Defines information about when an object hits by raycast
    /// </summary>
    public class RaycastHitInfo : HitInfo
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
        public RaycastHitInfo(GameObject owner, HitType type, UnityEngine.Collider other)
            : base(owner, type, other)
        {
        }
    }

    /// <summary>
    /// Defines information about when an object hits by another object
    /// </summary>
    public class CollisionHitInfo : HitInfo
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
        public CollisionHitInfo(GameObject owner, HitType type, UnityEngine.Collider other)
            : base(owner, type, other)
        {
        }
    }
}