using UnityEngine;
using System.Collections;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Base class for all triggers
    /// </summary>
    public abstract class Trigger : StaticBehaviour
    {
        public Color GizmoColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        public GizmosHelper.DrawColliderMode GizmoDrawMode = GizmosHelper.DrawColliderMode.SolidAndWire;
        public string GizmoFilename = "Trigger.png";
        public Vector3 GizmoOffset = Vector3.zero;
        /// <summary> Filter other colliders by tags </summary>
        public string[] Tags = new string[] { "Player" };
        /// <summary> How many times execute trigger</summary>
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
                        bool b = OnEnter(other);
                        if (!Unlimite && b)
                            TriggerCount--;
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
        /// <returns>True if event handled, otherwise false</returns>
        protected virtual bool OnEnter(Collider other) { return false; }
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
        /// Draw gizmo
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (!string.IsNullOrEmpty(GizmoFilename))
                Gizmos.DrawIcon(transform.position + transform.TransformDirection(GizmoOffset), GizmoFilename, false);
            if (GizmoDrawMode != GizmosHelper.DrawColliderMode.None)
                GizmosHelper.DrawCollider(collider, GizmoColor, GizmoDrawMode);
        }
    }

}