using UnityEngine;
using System.Collections;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Base class for all triggers
    /// </summary>
    public class Trigger : StaticBehaviour
    {
        /// <summary> Filter other colliders by tags </summary>
        public string[] Tags = new string[] { "Player" };
        /// <summary> How many time to execute trigger</summary>
        public int TriggerCount = 1;
        /// <summary> Execution of trigger is unlimited</summary>
        public bool Unlimite = false;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            TriggerCount = Mathf.Max(TriggerCount, 1);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (TriggerCount <= 0) return;
            if (Tags != null && Tags.Length > 0)
            {
                foreach (var tag in Tags)
                {
                    if (other.tag == tag)
                    {
                        if (!Unlimite)
                            TriggerCount--;
                        OnEnter(other);
                        break;
                    }
                }
            }
            else
            {
                OnEnter(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            if (TriggerCount <= 0) return;
            if (Tags != null && Tags.Length > 0)
            {
                foreach (var tag in Tags)
                {
                    if (other.tag == tag)
                    {
                        OnExit(other);
                        break;
                    }
                }
            }
            else
            {
                OnExit(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enabled) return;
            if (Tags != null && Tags.Length > 0)
            {
                foreach (var tag in Tags)
                {
                    if (other.tag == tag)
                    {
                        OnStay(other);
                        break;
                    }
                }
            }
            else
            {
                OnStay(other);
            }
        }

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected virtual void OnEnter(Collider other) { }
        /// <summary>
        /// called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected virtual void OnExit(Collider other) { }
        /// <summary>
        /// called almost all the frames for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected virtual void OnStay(Collider other) { }

        /// <summary>
        /// Name of file in Gizmos folder 
        /// </summary>
        protected virtual string GizmoFilename { get { return "Trigger.png"; } }

        /// <summary>
        /// Draw gizmo
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, GizmoFilename, false);
        }
    }

}