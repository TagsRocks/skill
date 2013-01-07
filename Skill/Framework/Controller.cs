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
    public abstract class Controller : DynamicBehaviour
    {
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
        /// Notify GameObject is dead
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> An System.EventArgs that contains no event data. </param>
        protected virtual void OnDie(object sender, EventArgs e)
        {
            if (Behavior != null)
                Behavior.Reset();
            if (Spawner != null)
                Spawner.NotifySpawnedObjectIsDead(this.gameObject);
        }

        /// <summary>
        /// Handle a ray or somthing Hit this GameObject
        /// </summary>
        /// <param name="sender"> sender </param>
        /// <param name="args"> An HitEventArgs that contains hit event data. </param>        
        protected virtual void OnHit(object sender, HitEventArgs args)
        {
        }

        /// <summary>
        /// when controller destroyed
        /// </summary>
        protected override void OnDestroy()
        {
            if (Behavior != null)
                Behavior.Reset();
            base.OnDestroy();
        }

        /// <summary>
        /// Destroy controller
        /// </summary>
        public virtual void Destroy()
        {
            if (Spawner != null)
                this.Spawner.DestroySpawnedObject(this.gameObject);
            else
                GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// Hooks required events of EventManager. ('Die' and 'Hit' events)
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if (Events != null)
            {
                Events.Die += OnDie;
                Events.Hit += OnHit;
            }
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
