using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Dynamics
{
    /// <summary>
    /// Simulation force of explosion without Rigidbody in y axis until returns to initial position.
    /// This behavior is disable and by enabling this behavior simulation starts.
    /// </summary>    
    public class ThrowUp : DynamicBehaviour
    {

        /// <summary> Gravity (must be positive)</summary>
        public float Gravity = 10;
        /// <summary> Initial force(speed) of explosion </summary>
        public float Force = 5;
        /// <summary> require valid EventManager component assined to game object to hook Die event </summary>
        public bool SimulateOnDie = false;

        private float _ElapsedTime;
        private float _HeightOnExplosion;
        private float _HalfGravity;

        /// <summary>
        /// Hook required events
        /// </summary>
        protected override void HookEvents()
        {
            base.HookEvents();
            if (SimulateOnDie)
            {
                if (Events != null)
                {
                    Events.Die += Events_Die;
                }
            }
        }

        /// <summary>
        /// Hook required events
        /// </summary>
        protected override void UnhookEvents()
        {
            base.UnhookEvents();
            if (SimulateOnDie)
            {
                if (Events != null)
                {
                    Events.Die -= Events_Die;
                }
            }
        }

        /// <summary>
        /// Start simulation
        /// </summary>
        public void Go()
        {
            // just enable to start simulation
            enabled = true;
            _ElapsedTime = 0;
            _HalfGravity = -0.5f * Gravity;
        }

        /// <summary>
        /// Notify GameObject is dead
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="e"> An System.EventArgs that contains no event data. </param>
        protected virtual void Events_Die(object sender, EventArgs e)
        {
            Go();
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _HeightOnExplosion = transform.position.y;
        }

        /// <summary>
        /// prepare for simulation
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Go();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            _ElapsedTime += Time.deltaTime;
            float y = (_HalfGravity * _ElapsedTime + Force) * _ElapsedTime;

            Vector3 pos = transform.position;
            pos.y = _HeightOnExplosion + y;
            if (pos.y < _HeightOnExplosion)
            {
                pos.y = _HeightOnExplosion;
                enabled = false;
            }
            transform.position = pos;
            base.Update();
        }
    }
}
