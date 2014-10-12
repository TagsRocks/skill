using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Skill.Framework.Animation;

namespace Skill.Framework
{
    /// <summary>
    /// Base class for controllers
    /// </summary>
    [RequireComponent(typeof(Skill.Framework.EventManager))]
    public abstract class Controller : DynamicBehaviour, IBehavioural
    {
        /// <summary>
        /// An optional meaningful identifier for controller to be distinguishable from others.
        /// (set this to a valid value to be visible in BehaviorTree debugger)
        /// </summary>
        public string Identifier;

        private Skill.Framework.AI.BehaviorTree _Behavior;
        /// <summary>
        /// BehaviorTree of controller (should provide by subclass)
        /// </summary>
        public Skill.Framework.AI.BehaviorTree Behavior
        {
            get { return _Behavior; }
            protected set
            {
                if (_Behavior != null)
                    _Behavior.Reset();
                _Behavior = value;
            }
        }

        /// <summary>
        /// Health
        /// </summary>
        public Health Health { get; private set; }

        /// <summary>
        /// Spawner that spawned this controller
        /// </summary>
        public Spawner Spawner { get; internal set; }


        /// <summary>
        /// Hooks required events of EventManager.
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if (Events != null)
            {
                Events.Die += Events_Die;
                Events.Hit += Events_Hit;
                Events.Cached += Events_Cached;
            }
        }



        /// <summary>
        /// Unhooks hooked events
        /// </summary>
        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (Events != null)
            {
                Events.Die -= Events_Die;
                Events.Hit -= Events_Hit;
                Events.Cached -= Events_Cached;
            }
        }

        private void ResetValues()
        {
            if (Behavior != null)
                Behavior.Reset();
            if (Spawner != null)
            {
                Spawner.NotifySpawnedObjectIsDead(this.gameObject);
                Spawner = null;
            }
        }

        /// <summary>
        /// Notify GameObject is dead
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> An System.EventArgs that contains no event data. </param>
        protected virtual void Events_Die(object sender, EventArgs e)
        {
            ResetValues();
        }

        /// <summary>
        /// Handle a ray or somthing Hit this GameObject
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="args"> An HitEventArgs that contains hit event data. </param>        
        protected virtual void Events_Hit(object sender, HitEventArgs args)
        {
        }

        /// <summary>
        /// Handle when object cached by CacheSpawner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void Events_Cached(object sender, Managers.CacheEventArgs args)
        {
            ResetValues();
        }

        /// <summary>
        /// when controller destroyed
        /// </summary>
        protected override void OnDestroy()
        {            
            ResetValues();
            Global.UnRegister(this);
            base.OnDestroy();
        }

        /// <summary>
        /// Destroy controller
        /// </summary>
        public override void DestroySelf()
        {
            ResetValues();
            base.DestroySelf();
        }


        /// <summary>
        /// Use this for initialization
        /// </summary>
        protected override void Start()
        {
            base.Start();
            Global.Register(this);
        }


        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            Health = GetComponent<Health>();
        }


    }
}
