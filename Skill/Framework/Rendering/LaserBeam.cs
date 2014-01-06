using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Rendering
{
    /// <summary>
    /// Should be parent of one or more "Beam" to work correctly. each beam can have deferent parameters and when combined togather create desired result
    /// </summary>    
    public class LaserBeam : Skill.Framework.DynamicBehaviour
    {
        /// <summary> The particle system, in this case sparks which will be created by the Laser </summary>
        public GameObject EndEffect;
        /// <summary> Maximum lenght of laser beam </summary>
        public float MaxLength = 50.0f;
        /// <summary> check for collision </summary>
        public bool Collision = false;
        /// <summary> if collision, rotate EndEffect along hit normal </summary>
        public bool RotateEffect = true;

        /// <summary> filter raycast collision detection </summary>
        [HideInInspector]
        public int CollisionLayerMask = 0xFFFFFFF; // 

        private Beam[] _Beams;
        private Vector3 _HitPoint;
        private Vector3 _HitNormal;
        private Transform _EndEffectTransform;
        private float _HitDistance;
        private bool _IsHit;

        /// <summary> Hit point (valid if IsHit) </summary>
        public Vector3 HitPoint { get { return _HitPoint; } }
        /// <summary> Hit normal (valid if IsHit) </summary>
        public Vector3 HitNormal { get { return _HitNormal; } }
        /// <summary> Hit distance (valid if IsHit) </summary>
        public float HitDistance { get { return _HitDistance; } }
        /// <summary> Is laser hit by something?</summary>
        public bool IsHit { get { return _IsHit; } }
        /// <summary> child beams </summary>
        public Beam[] Beams { get { return _Beams; } }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _Beams = GetComponentsInChildren<Beam>();// find child beams
            foreach (var beam in _Beams)
            {
                if (beam != null)
                {
                    beam.transform.localPosition = Vector3.zero;
                    //Vector3 euler = beam.transform.localEulerAngles;
                    //euler.x = euler.y = 0;
                    //beam.transform.localEulerAngles = euler;
                }
            }
            if (EndEffect != null)
                _EndEffectTransform = EndEffect.transform;// save effect transform for better performance
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (!Skill.Framework.Global.IsGamePaused)
            {
                //Shoot our laserbeam forwards!            
                if (MaxLength < 0.01f) MaxLength = 0.01f;
                _IsHit = false;
                if (Collision)
                {
                    //Raycast from the location of the cube forwards
                    RaycastHit[] hits = Physics.RaycastAll(_Transform.position, _Transform.forward, MaxLength, CollisionLayerMask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        //Check to make sure we aren't hitting triggers but colliders
                        if (!hits[i].collider.isTrigger)
                        {
                            _IsHit = true;
                            _HitPoint = hits[i].point;
                            _HitNormal = hits[i].normal;
                            _HitDistance = hits[i].distance;
                            break;
                        }
                    }
                }

                if (_IsHit)
                {
                    //Move our End Effect particle system to the hit point and start playing it
                    if (EndEffect != null)
                    {
                        _EndEffectTransform.position = _HitPoint;
                        if (RotateEffect)
                            _EndEffectTransform.forward = _HitNormal;// rotate effect along hit normal
                        if (!EndEffect.activeSelf)
                            EndEffect.SetActive(true);

                    }
                }
                else
                {
                    //If we're not hitting anything, don't play the particle effects
                    if (EndEffect != null)
                    {
                        if (EndEffect.activeSelf)
                            EndEffect.SetActive(false);
                    }
                    _HitDistance = MaxLength;
                }

                // update beam parameters
                foreach (var beam in _Beams)
                {
                    if (beam != null)
                    {
                        beam.IsHit = _IsHit;
                        beam.HitPoint = _HitPoint;
                        beam.Length = _HitDistance;
                    }
                }
            }
            base.Update();
        }
    }
}