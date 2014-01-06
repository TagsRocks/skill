using UnityEngine;
using Skill.Framework.Managers;
using System.Collections.Generic;

namespace Skill.Framework.Weapons
{
    /// <summary>
    /// Bullet to use by weapons. if bullet has RigidBody with low speed controlled by physics engine. it is possible to enable gravity to fall down after spawn or
    /// disable gravity to go in straight direction. The weapon do AddForce to RigidBody bullets at spawn time.
    /// if collider is a trigger the bullet check collision by OnTriggerEnter method
    /// if collider is not a trigger the bullet check collision by OnCollisionEnter method
    /// </summary>    
    public class Bullet : DynamicBehaviour
    {
        /// <summary> Object to spawn on collision </summary>
        public GameObject[] Explosions;

        /// <summary> Position of explosion </summary>
        public Transform ExplosionPos;

        /// <summary>
        /// Lift time of bullet after spawn. A time with this value begins OnEnable and destroy gameobject when timer end. ( zero or negative for infinit lift time)
        /// </summary>
        public float LifeTime = 1.5f;

        /// <summary>
        /// The GameObject that shoots this bullet. usually it is Controller of weapon.
        /// </summary>
        public UnityEngine.GameObject Shooter { get; set; }

        /// <summary>
        /// Amount of damage caused by this bullet.
        /// </summary>
        public virtual float Damage { get; set; }

        /// <summary>
        /// Speed of bullet in air. (should be setted by weapon)
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// Direction of bullet in air. (should be setted by weapon)
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Range of bullet after spawn. (should be setted by weapon)
        /// </summary>
        public float Range { get; set; }

        /// <summary> User Data </summary>
        public object UserData { get; set; }

        /// <summary> Damage Type of projectil</summary>
        public int DamageType { get; set; }

        /// <summary> Target of projectile </summary>
        public Vector3? Target { get; set; }

        private TimeWatch _LifeTimeTW;
        private bool _IsDead;        

        

        /// <summary>
        /// Called by weapon when initialize bullet
        /// </summary>
        public virtual void StartJourney()
        {
            if (LifeTime > 0)
                _LifeTimeTW.Begin(LifeTime);
            enabled = true;
            _IsDead = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (_LifeTimeTW.IsEnabledAndOver)
            {
                _LifeTimeTW.End();
                OnDie(null);
            }
            base.Update();
        }

        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        /// </summary>
        /// <param name="collision"> Collision information</param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!IsDestroyed)
            {
                if (!IsCollisionAccepted(collision)) return;
                ContactPoint cp = collision.contacts[0];
                EventManager eventManager = cp.otherCollider.GetComponent<EventManager>();
                if (eventManager != null)
                {
                    eventManager.RaiseDamage(this, new DamageEventArgs(this.Damage) { UserData = this.UserData, DamageType = DamageType, Tag = tag });
                }
                OnDie(cp.otherCollider);
            }
        }



        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        /// </summary>
        /// <param name="other"> other collider</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!IsDestroyed)
            {
                if (!IsCollisionAccepted(other)) return;
                EventManager eventManager = other.GetComponent<EventManager>();
                if (eventManager != null)
                {
                    eventManager.RaiseDamage(this, new DamageEventArgs(this.Damage) { UserData = this.UserData, DamageType = DamageType, Tag = tag });
                }
                OnDie(other);
            }
        }

        /// <summary>
        /// Allow subclass to filter collsions. it is possible to filter collisions by tags based on whether this bullet shooted by player/friend or
        /// shooted by an enemy.
        /// </summary>
        /// <param name="other"> other collider</param>
        /// <returns>True if accepted, false for rejected</returns>
        protected virtual bool IsCollisionAccepted(Collider other)
        {
            if (Shooter != null)
                return other.tag != Shooter.tag;
            else
                return true;
        }

        /// <summary>
        /// Allow subclass to filter collsions. it is possible to filter collisions by tags based on whether this bullet shooted by player/friend or
        /// shooted by an enemy.
        /// </summary>
        /// <param name="collision">Collision</param>
        /// <returns>True if accepted, false for rejected</returns>
        protected virtual bool IsCollisionAccepted(Collision collision)
        {
            if (Shooter != null)
                return collision.collider.tag != Shooter.tag;
            else
                return true;
        }


        /// <summary>
        /// Allow subclass to filter explosion particle based on collider bullet hit with
        /// by default return random explosion if exist
        /// </summary>
        /// <param name="collider">The collider that bullet hit with (can be null)</param>
        /// <returns>valid GameObjct if explosion required, otherwise null</returns>
        protected virtual GameObject GetExplosion(Collider collider)
        {
            if (collider == null) return null;
            if (Explosions != null && Explosions.Length > 0)
                return Explosions[Random.Range(0, Explosions.Length)];
            else
                return null;
        }

        /// <summary>
        /// Call this when bullet reach end of range or hit something or out of life time
        /// </summary>
        protected virtual void OnDie(Collider collider)
        {
            if (_IsDead) return;

            Shooter = null;
            Damage = 0;
            Target = null;

            if (rigidbody != null && !rigidbody.isKinematic)
                rigidbody.velocity = Vector3.zero;

            _IsDead = true;

            var exp = GetExplosion(collider);
            if (exp != null)
                Skill.Framework.Managers.Cache.Spawn(exp, (ExplosionPos != null) ? ExplosionPos.position : _Transform.position, exp.transform.rotation);
            if (Events != null)
                Events.RaiseDie(this, System.EventArgs.Empty);
            Target = null;
            Cache.DestroyCache(gameObject);
        }        
    }
}