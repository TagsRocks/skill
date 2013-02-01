using UnityEngine;
using Skill.Framework.Managers;

namespace Skill.Framework.Weapons
{    
    /// <summary>
    /// Bullet to use by weapons. if bullet has RigidBody with low speed controlled by physics engine. it is possible to enable gravity to fall down after spawn or
    /// disable gravity to go in straight direction. The weapon do AddForce to RigidBody bullets at spawn time.
    /// if collider is a trigger the bullet check collision by OnTriggerEnter method
    /// if collider is not a trigger the bullet check collision by OnCollisionEnter method
    /// </summary>
    [AddComponentMenu("Skill/Weapons/Bullets/Bullet")]  
    public class Bullet : DynamicBehaviour
    {
        /// <summary> Object to spawn on collision </summary>
        public GameObject Explosion;

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
        public float Damage { get; set; }

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

        /// <summary> Target of projectile </summary>
        public Vector3? Target { get; set; }

        private TimeWatch _LifeTimeTW;
        private bool _IsDead;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (LifeTime > 0)
                _LifeTimeTW.Begin(LifeTime);
            enabled = true;
            _IsDead = false;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            Shooter = null;
            Damage = 0;
            Target = null;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (_LifeTimeTW.EnabledAndOver) OnDie();
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
                    eventManager.OnDamage(this, new DamageEventArgs(this.Damage));
                }
                OnDie();
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
                    eventManager.OnDamage(this, new DamageEventArgs(this.Damage));
                }
                OnDie();
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
        /// Call this when bullet reach end of range or hit something or out of life time
        /// </summary>
        protected virtual void OnDie()
        {
            if (_IsDead) return;
            _IsDead = true;
            if (Explosion != null)
                Skill.Framework.Managers.CacheSpawner.Spawn(Explosion, _Transform.position, Explosion.transform.rotation);
            if (Events != null)
                Events.OnDie(this, System.EventArgs.Empty);
            Target = null;
            CacheSpawner.DestroyCache(gameObject);
        }
    }
}