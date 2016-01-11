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
        public string[] IgnoreExplosion;
        /// <summary> tags that filtered for raycast</summary>
        /// <remarks>
        /// for example if a glass be in radius but there was something between glass and explosion as a block, it is not matter, glass will breaks anyway
        /// and this method should return true for tag == "Glass"
        /// </remarks>
        public string[] IgnoreRaycast = new string[] { "Glass" };
        /// <summary> tags that filtered to apply force</summary>
        public string[] IgnoreForce;
        /// <summary> Colliders to ignore on explosion </summary>
        public Collider[] SelfColliders;
        /// <summary> Use raycast to know if any object is between explosion and collider to block explosion </summary>
        public bool Raycast = false;
        /// <summary> use raycast to sure there is something between collider and explosion that block explosion</summary>        
        public LayerMask LayerMask = 0;        

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();            
            ApplyExplosion();
        }

        protected virtual void ApplyExplosion()
        {

            if (SelfColliders == null)
                SelfColliders = new Collider[0];
            bool[] colliderStates = new bool[SelfColliders.Length];
            for (int i = 0; i < SelfColliders.Length; i++)
            {
                colliderStates[i] = SelfColliders[i].enabled;
                SelfColliders[i].enabled = false;
            }

            if (FalloffRadius < Radius)
                FalloffRadius = Radius;

            float deltaRadius = FalloffRadius - Radius;

            Vector3 explosionPos = transform.position + Offset;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, FalloffRadius);
            float distance = 0;

            foreach (Collider collider in colliders)
            {
                if (collider != null)
                {
                    if (IsAffectedByExplosion(collider.gameObject.tag))
                    {
                        Vector3 colliderPos = collider.ClosestPointOnBounds(explosionPos);
                        distance = Vector3.Distance(explosionPos, colliderPos);
                        bool ignore = distance > FalloffRadius;
                        // use raycast if there is something between collider and explosion
                        if (!ignore && Raycast && IsUsedForRaycast(collider.tag))
                        {
                            if (Physics.Linecast(explosionPos, colliderPos, LayerMask))
                                ignore = true;
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

                            Rigidbody crb = collider.GetComponent<Rigidbody>();
                            if (Force > 0 && crb != null && IsUsedForForce(collider.tag))
                            {
                                crb.AddForceAtPosition((Force * percent) * (colliderPos - explosionPos).normalized, colliderPos, ForceMode);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < SelfColliders.Length; i++)
                SelfColliders[i].enabled = colliderStates[i];
        }



        private bool IsUsedForForce(string tag)
        {
            return !IsTagExist(tag, IgnoreForce);
        }

        private bool IsUsedForRaycast(string tag)
        {
            return !IsTagExist(tag, IgnoreRaycast);
        }
        private bool IsAffectedByExplosion(string tag)
        {
            return !IsTagExist(tag, IgnoreExplosion);
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
