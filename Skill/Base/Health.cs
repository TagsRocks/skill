using UnityEngine;
using System.Collections;
using Skill.Managers;

namespace Skill
{
    /// <summary>
    /// containing HealthChange event data.
    /// </summary>
    public class HealthChangeEventArgs : System.EventArgs
    {
        /// <summary> Amount of health change </summary>
        public float DeltaHealth { get; private set; }

        /// <summary>
        /// Create a HealthChangeEventArgs
        /// </summary>
        /// <param name="deltaHealth"> Amount of health change </param>
        public HealthChangeEventArgs(float deltaHealth)
        {
            this.DeltaHealth = deltaHealth;
        }
    }

    /// <summary>
    /// Handle health change
    /// </summary>
    /// <param name="sender">The source of event</param>
    /// <param name="args"> a HealthChangeEventArgs containing HealthChange event data.</param>        
    public delegate void HealthChangeHandler(Object sender, HealthChangeEventArgs args);

    /// <summary>
    /// Defines Health behaviour for gameobject
    /// </summary>
    [AddComponentMenu("Skill/Health")]
    [RequireComponent(typeof(EventManager))]
    public class Health : DynamicBehaviour
    {
        /// <summary> Decale object to spawn on hit( leave it null for no decal). </summary>
        public GameObject Decal;
        /// <summary> Particle(like sparkle) to spawn on hit( leave it null for no particle). </summary>
        public GameObject HitParticle;
        /// <summary> Offset of decale to hit surface </summary>
        public float DecalOffset = 0.1f;
        /// <summary> True if you want HitParticle spawn at inverse direction of hit normal </summary>
        public bool InverseHit;
        /// <summary> Maximum amount of health (if RegenerateSpeed > 0) </summary>
        public float MaxHealth = 100.0f;
        /// <summary> Initial health value </summary>
        public float InitialHealth = 100.0f;
        /// <summary> The speed of regenerate health after damage.</summary>
        public float RegenerateSpeed = 0.0f;
        /// <summary> The delay time before start to regenerate health.</summary>
        public float RegenerateDelay = 3.0f;
        /// <summary> If true, never take damage </summary>
        public bool Invincible = false;
        /// <summary> the time begins when health is zero to delay die </summary>
        public float DieDelay = 0.0f;

        private TimeWatch _RegenerateDelayTW;
        private TimeWatch _DieDelayTW;

        private float _CurrentHealth;
        /// <summary>
        /// Gets or sets current health value
        /// </summary>
        public float CurrentHealth
        {
            get { return _CurrentHealth; }
            set
            {
                if (value < 0) value = 0;
                else if (value > MaxHealth) value = MaxHealth;

                if (_CurrentHealth != value)
                {
                    float deltaHealth = _CurrentHealth - value;
                    _CurrentHealth = value;
                    OnHealthChange(deltaHealth);

                    if (_CurrentHealth == 0)
                    {
                        if (DieDelay < 0)
                            DieDelay = 0;
                        enabled = true;
                        _DieDelayTW.Begin(DieDelay);
                    }
                    else
                    {
                        // Enable so the Update function will be called
                        // if regeneration is enabled
                        if (RegenerateSpeed != 0 && _CurrentHealth < MaxHealth)
                            enabled = true;
                        else
                            enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Whether agent is dead or not.
        /// </summary>
        public bool IsDead { get; private set; }

        /// <summary>
        /// Occurs when amount of Health changed.
        /// </summary>
        public event HealthChangeHandler HealthChange;

        /// <summary>
        /// Occurs when amount of Health changed.
        /// </summary>
        /// <param name="deltaHealth"> amount of Health changed. </param>
        protected virtual void OnHealthChange(float deltaHealth)
        {
            if (HealthChange != null)
                HealthChange(this, new HealthChangeEventArgs(deltaHealth));
        }

        /// <summary>
        /// Hook required events 
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if (Events != null)
            {
                Events.Hit += OnHit;
                Events.Damage += OnDamage;
            }
        }

        /// <summary>
        /// GameObject is dead
        /// </summary>        
        protected virtual void OnDie()
        {
            IsDead = true;
            enabled = false;
            if (Events != null)
                Events.OnDie(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Initialize
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (MaxHealth < 0) MaxHealth = 0;
            if (InitialHealth < 0) InitialHealth = 0;
            else if (InitialHealth > MaxHealth) InitialHealth = MaxHealth;
            _CurrentHealth = InitialHealth;

            enabled = RegenerateSpeed != 0.0f;
        }

        /// <summary>
        /// Handle a ray or somthing Hit this GameObject
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="args"> An HitEventArgs that contains hit event data. </param>        
        protected virtual void OnHit(object sender, HitEventArgs args)
        {
            if (Events != null)
                Events.OnDamage(sender, new DamageEventArgs(args.Hit.Damage));

            if (HitParticle != null)
            {
                Vector3 normal = InverseHit ? -args.Hit.Normal : args.Hit.Normal;
                CacheSpawner.Spawn(HitParticle, args.Hit.Point, Quaternion.LookRotation(normal));
            }
            if (Decal != null)
            {
                CacheSpawner.Spawn(Decal, args.Hit.Point + DecalOffset * args.Hit.Normal, Quaternion.Euler(args.Hit.Normal));
            }
        }


        /// <summary>
        /// Handle imposed damage
        /// </summary>    
        /// <param name="sender">The source of the event.</param>
        /// <param name="args"> An DamageEventArgs that contains damage event data.</param>
        protected virtual void OnDamage(object sender, DamageEventArgs args)
        {
            if (IsDead) return;

            // Take no damage if invincible, dead, or if the damage is zero
            if (Invincible)
                return;
            if (args.Damage <= 0)
                return;

            // Decrease health by damage and send damage signals	
            CurrentHealth -= args.Damage;
            _RegenerateDelayTW.Begin(RegenerateDelay);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Time.timeScale == 0) return;
            if (_DieDelayTW.Enabled)
            {
                if (_DieDelayTW.IsOver)
                {
                    _DieDelayTW.End();
                    OnDie();
                }
            }
            else
            {
                Regenerate();
            }
            base.Update();
        }


        /// <summary>
        /// Regenerate health
        /// </summary>
        protected virtual void Regenerate()
        {
            if (RegenerateSpeed != 0.0f)
            {
                if (_RegenerateDelayTW.Enabled)
                {
                    if (_RegenerateDelayTW.IsOver || RegenerateSpeed < 0)
                    {
                        // regerate health by RegenerateSpeed
                        CurrentHealth += RegenerateSpeed * UnityEngine.Time.deltaTime;
                    }
                }
            }
        }
    }

}