using UnityEngine;
using System.Collections;
using Skill.Framework.Managers;
using System.Collections.Generic;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Apply periodic damage to each collidion objects
    /// </summary>
    [AddComponentMenu("Skill/Triggers/Damage")]
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

        private List<ColliderInfo> _Colliders;
        private Ray _IntersectRay;

        /// <summary> User Data </summary>
        public object UserData { get; set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Colliders = new List<ColliderInfo>();
            _IntersectRay.origin = _Transform.position;
        }

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        /// <returns>True if event handled, otherwise false</returns>
        protected override bool OnEnter(Collider other)
        {
            ApplyDamage(other);
            return true;
        }
        /// <summary>
        /// called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnExit(Collider other)
        {
            RemoveFromList(other);
        }
        /// <summary>
        /// called almost all the frames for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnStay(Collider other)
        {
            ApplyDamage(other);
        }

        private int GetIndexOf(Collider other)
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

        private void RemoveFromList(Collider other)
        {
            int index = GetIndexOf(other);
            if (index >= 0)
                _Colliders.RemoveAt(index);
        }

        private void ApplyDamage(Collider other)
        {
            ColliderInfo cInfo = null;
            int index = GetIndexOf(other);
            if (index >= 0)
            {
                cInfo = _Colliders[index];
            }
            else
            {
                cInfo = new ColliderInfo() { Collider = other };
                _Colliders.Add(cInfo);
            }
            ApplyDamage(cInfo);
        }

        private void ApplyDamage(ColliderInfo cInfo)
        {
            if (cInfo != null)
            {
                if (!cInfo.DamageTW.IsEnabledAndOver)
                    cInfo.DamageTW.End();

                if (!cInfo.DamageTW.IsEnabled)
                {
                    EventManager em = cInfo.Collider.GetComponent<EventManager>();
                    if (em != null)
                    {
                        float d = Damage;
                        if (DecreaseByDistance)
                        {
                            _IntersectRay.origin = _Transform.position;
                            _IntersectRay.direction = (cInfo.Collider.transform.position - _IntersectRay.origin).normalized;

                            float dis;
                            if (!cInfo.Collider.bounds.IntersectRay(_IntersectRay, out dis))
                                dis = 0; // colliders are very close

                            d = Damage * (1.0f - Mathf.Min(Range, dis) / Range);
                        }

                        em.OnDamage(this, new DamageEventArgs(d, tag) { UserData = this.UserData });
                        cInfo.DamageTW.Begin(Interval);
                    }
                }
            }
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