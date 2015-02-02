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
            public EventManager Events;
            public Collider Collider;
            public TimeWatch DamageTW;
            public TimeWatch IgnoreTW;

            public bool IsDestroyed { get { return Events != null && Events.IsDestroyed; } }

            public ColliderInfo(Collider collider)
            {
                this.Collider = collider;
                this.Events = collider.GetComponent<EventManager>();
                DamageTW.End();
            }
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

        private List<ColliderInfo> _Colliders;

        /// <summary> User Data </summary>
        public object UserData { get; set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Colliders = new List<ColliderInfo>(5);
        }

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        /// <returns>True if event handled, otherwise false</returns>
        protected override bool OnEnter(Collider other)
        {
            if (!Contains(other))
            {
                ColliderInfo cInfo = new ColliderInfo(other);
                if (cInfo.Events != null)
                {
                    _Colliders.Add(cInfo);
                    ApplyDamage(cInfo);
                }
            }
            return true;
        }
        /// <summary>
        /// called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnExit(Collider other)
        {
            int index = IndexOf(other);
            if (index >= 0)
                _Colliders.RemoveAt(index);
        }
        /// <summary>
        /// called almost all the frames for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnStay(Collider other)
        {
            int index = IndexOf(other);
            if (index >= 0)
                ApplyDamage(_Colliders[index]);
        }

        private int IndexOf(Collider other)
        {
            int index = -1;
            for (int i = 0; i < _Colliders.Count; i++)
            {
                if (_Colliders[i].Collider == other)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private bool Contains(Collider other)
        {
            return IndexOf(other) >= 0;
        }

        private void ApplyDamage(ColliderInfo cInfo)
        {
            if (cInfo.DamageTW.IsOver)
            {
                EventManager em = cInfo.Events;
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
                    cInfo.IgnoreTW.Begin(Interval * 2);
                }
            }
        }

        protected virtual void Update()
        {
            if (Global.IsGamePaused) return;
            int index = 0;
            while (index < _Colliders.Count)
            {
                if (_Colliders[index].IsDestroyed || _Colliders[index].IgnoreTW.IsOver)
                    _Colliders.RemoveAt(index);
                else
                    index++;
            }
        }
    }

}