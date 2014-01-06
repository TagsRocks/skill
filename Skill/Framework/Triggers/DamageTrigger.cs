using UnityEngine;
using System.Collections;
using Skill.Framework.Managers;
using System.Collections.Generic;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Apply periodic damage to each collidion objects
    /// </summary>    
    public class DamageTrigger : Trigger
    {
        private class ColliderInfo
        {
            public Collider Collider;
            public TimeWatch DamageTW;
        }

        /// <summary> Amount of damage </summary>
        public float Damage = 100;
        /// <summary> Interval between apply damage while stay in collision </summary>
        public float Interval = 1.0f;
        /// <summary> Maximum distance to apply damage (if DecreaseByDistance is true ) </summary>
        public float Range = 3;
        /// <summary> Decrease amount of damage by distance </summary>
        public bool DecreaseByDistance = false;
        /// <summary> Damage Type </summary>
        public int DamageType;

        private List<ColliderInfo> _EnteredColliders;
        private List<ColliderInfo> _ExitedColliders;

        /// <summary> User Data </summary>
        public object UserData { get; set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _EnteredColliders = new List<ColliderInfo>();
            _ExitedColliders = new List<ColliderInfo>();
        }

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        /// <returns>True if event handled, otherwise false</returns>
        protected override bool OnEnter(Collider other)
        {
            ColliderInfo cInfo = null;
            int index = GetIndexOf(other, _ExitedColliders);
            if (index >= 0)
            {
                cInfo = _ExitedColliders[index];
                _ExitedColliders.RemoveAt(index);
            }
            if (cInfo != null)
            {
                _EnteredColliders.Add(cInfo);
            }
            else
            {
                if (GetIndexOf(other, _EnteredColliders) < 0)
                    _EnteredColliders.Add(new ColliderInfo() { Collider = other });
            }
            return true;
        }
        /// <summary>
        /// called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnExit(Collider other)
        {
            ColliderInfo cInfo = null;
            int index = GetIndexOf(other, _EnteredColliders);
            if (index >= 0)
            {
                cInfo = _EnteredColliders[index];
                _EnteredColliders.RemoveAt(index);
            }
            if (cInfo != null)
            {
                _ExitedColliders.Add(cInfo);
            }
        }
        /// <summary>
        /// called almost all the frames for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnStay(Collider other) { }

        private int GetIndexOf(Collider other, List<ColliderInfo> list)
        {
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Collider == other)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void ApplyDamage(ColliderInfo cInfo)
        {
            if (cInfo.DamageTW.IsOver)
            {
                EventManager em = cInfo.Collider.GetComponent<EventManager>();
                if (em != null)
                {
                    float d = Damage;
                    if (DecreaseByDistance)
                    {
                        Vector3 closestPoint = cInfo.Collider.ClosestPointOnBounds(_Transform.position);
                        float distance = Vector3.Distance(closestPoint, _Transform.position);
                        d *= (1.0f - Mathf.Min(Range, distance) / Range);
                    }

                    em.RaiseDamage(this, new DamageEventArgs(d) { UserData = this.UserData, DamageType = DamageType, Tag = tag });
                    cInfo.DamageTW.Begin(Interval);
                }
            }
        }

        protected virtual void Update()
        {
            if (Global.IsGamePaused) return;
            int index = 0;
            while (index < _ExitedColliders.Count)
            {
                if (_ExitedColliders[index].DamageTW.IsOver)
                    _ExitedColliders.RemoveAt(index);
                else
                    index++;
            }
            for (int i = 0; i < _EnteredColliders.Count; i++)
                ApplyDamage(_EnteredColliders[i]);
        }

        protected override string GizmoFilename { get { return "Damage.png"; } }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (DecreaseByDistance)
                Gizmos.DrawWireSphere(transform.position, Range);
        }
    }

}