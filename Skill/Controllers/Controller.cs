using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Skill.Animation;

namespace Skill.Controllers
{
    /// <summary>
    /// Base class for controllers
    /// </summary>
    public class Controller : MonoBehaviour, Managers.IEventManagerHooker
    {
        /// <summary>
        /// BehaviorTree of controller (should provide by subclass)
        /// </summary>
        public Skill.AI.BehaviorTree Behavior { get; protected set; }
        /// <summary>
        /// AnimationTree of controller (should provide by subclass)
        /// </summary>
        public Skill.Animation.AnimationTree Animation { get; protected set; }

        /// <summary>
        /// Current posture of controller
        /// </summary>
        public AnimPosture Posture { get; protected set; }

        /// <summary>
        /// Spawner that spawned this controller
        /// </summary>
        public Spawner Spawner { get; internal set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            HookEvents();
        }

        /// <summary>
        /// When controller dies
        /// </summary>
        /// <param name="userData">User data</param>
        protected virtual void OnDie(object userData)
        {
            if (Behavior != null)
                Behavior.Reset();
        }

        /// <summary>
        /// when controller destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (Behavior != null)
                Behavior.Reset();            
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
        /// Hook required events of EventManager
        /// </summary>
        public virtual void HookEvents() { }        
    }
}
