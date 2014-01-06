using System;
using UnityEngine;
using System.Collections;
using Skill.Framework.Managers;

namespace Skill.Framework.Dynamics
{
    /// <summary>
    /// apply damage to in range objects OnEnable
    /// </summary>
    ////<remarks>
    ///// When a GameObject instantiated OnEnable method will called.
    ///// if use this behavior with a cachable object, and cache objects instantiated before use,
    ///// so it is better to ignore first enable
    ///// so make sure that if you use this behavior and CacheBehavior together, this gameobject must be inside a CacheGroup to work correctly
    ///// </remarks>
    [RequireComponent(typeof(EventManager))]
    public class Explosion : StaticBehaviour
    {
        /// <summary> Maximum Damage radius.</summary>
        public float Radius = 2;
        /// <summary> Damage falloff radius.</summary>
        public float FalloffRadius = 5;
        /// <summary> Amount of damage to apply on affected objects </summary>
        public float Damage = 100;
        /// <summary> Type of damage </summary>
        public int DamageType = 0;
        /// <summary> Apply force to objects </summary>
        public float Force = 0;
        /// <summary> ForceMode </summary>
        public ForceMode ForceMode = ForceMode.Force;
        /// <summary> Offset of explosion position </summary>
        public Vector3 Offset = new Vector3(0.0f, 0.2f, 0.0f);
        /// <summary> tags that ignored by explosion</summary>
        public string[] IgnoreExplision;
        /// <summary> tags that filtered for raycast</summary>
        /// <remarks>
        /// for example if a glass be in radius but there was something between glass and explosion as a block, it is not matter, glass will breaks anyway
        /// and this method should return true for tag == "Glass"
        /// </remarks>
        public string[] IgnoreRaycast = new string[] { "Glass" };
        /// <summary> tags that filtered to apply force</summary>
        public string[] IgnoreForceTags;
        /// <summary> Use raycast to know if any object is between explosion and collider to block explosion </summary>
        public bool UseRaycast = false;
        [HideInInspector]
        /// <summary> use raycast to sure there is something between collider and explosion that block explosion</summary>
        public int LayerMask = 0;

        //private bool _IgnoreFirst;

        //protected override void Awake()
        //{
        //    base.Awake();
        //    _IgnoreFirst = GetComponent<Managers.CacheBehavior>() != null;
        //}

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            //if (_IgnoreFirst)
            //    _IgnoreFirst = false;
            //else
                ApplyExplision();
        }

        protected virtual void ApplyExplision()
        {
            if (FalloffRadius < Radius)
                FalloffRadius = Radius;

            float deltaRadius = FalloffRadius - Radius;

            Vector3 explosionPos = _Transform.position + Offset;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, FalloffRadius);
            float distance = 0;

            foreach (Collider collider in colliders)
            {
                if (collider != null)
                {
                    if (IsAffectedByExplosion(collider.gameObject.tag))
                    {
                        Vector3 colliderPos = collider.ClosestPointOnBounds(explosionPos);
                        colliderPos.y += 0.2f;
                        Vector3 dir = colliderPos - explosionPos;
                        bool ignore = false;

                        // use raycast if there is something between collider and explosion
                        if (UseRaycast && IsUsedForRaycast(collider.tag))
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(explosionPos, dir.normalized, out hit, Radius, LayerMask))
                            {
                                if (hit.collider != collider)
                                    ignore = true;
                                else
                                {
                                    distance = hit.distance;
                                    colliderPos = hit.point;
                                }
                            }
                        }
                        else
                        {
                            distance = dir.magnitude;
                        }

                        if (!ignore)
                        {
                            if (Application.isEditor)
                                Debug.DrawLine(explosionPos, colliderPos, Color.red, 3);

                            float percent = 1.0f - Mathf.Clamp01((distance - Radius) / deltaRadius);

                            EventManager manager = collider.gameObject.GetComponent<EventManager>();
                            if (manager != null)
                            {
                                float dmg;
                                if (distance <= Radius)
                                    dmg = Damage;
                                else
                                    dmg = Damage * percent;
                                manager.RaiseDamage(this, new DamageEventArgs(dmg) { Tag = tag, DamageType = DamageType });
                            }

                            if (Force > 0 && collider.rigidbody != null && IsUsedForForce(collider.tag))
                            {
                                collider.rigidbody.AddForceAtPosition((Force * percent) * dir, colliderPos, ForceMode);
                            }
                        }
                    }
                }
            }
        }



        private bool IsUsedForForce(string tag)
        {
            return !IsTagExist(tag, IgnoreForceTags);
        }

        private bool IsUsedForRaycast(string tag)
        {
            return !IsTagExist(tag, IgnoreRaycast);
        }
        private bool IsAffectedByExplosion(string tag)
        {
            return !IsTagExist(tag, IgnoreExplision);
        }

        private bool IsTagExist(string tag, string[] list)
        {
            if (list != null && list.Length > 0)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    if (tag == list[i]) return true;
                }
            }
            return false;
        }
    }
}
