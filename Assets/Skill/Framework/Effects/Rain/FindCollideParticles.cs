using UnityEngine;
using System.Collections;
namespace Skill.Framework.Effects
{
    [RequireComponent(typeof(ParticleEmitter))]
    public class FindCollideParticles : MonoBehaviour
    {
        public float BurstEnergy = 10.0f;
        public GameObject SplashPrefab;

        private ParticleEmitter _Emitter;
        private int[] _LiveParticles;
        void Awake()
        {
            _Emitter = GetComponent<ParticleEmitter>();
            _LiveParticles = new int[1000];
        }

        void LateUpdate()
        {
            Particle[] theParticles = _Emitter.particles;
            if (theParticles.Length > _LiveParticles.Length)
                _LiveParticles = new int[theParticles.Length];
            int particlesToKeep = 0;
            for (var i = 0; i < _Emitter.particleCount; i++)
            {
                if (theParticles[i].energy > BurstEnergy)
                {
                    if (SplashPrefab != null)
                        Skill.Framework.Managers.Cache.Spawn(SplashPrefab, theParticles[i].position, SplashPrefab.transform.rotation);
                }
                else
                {
                    _LiveParticles[particlesToKeep++] = i;
                }
            }
            // Copy the ones we keep to a new array
            var keepParticles = new Particle[particlesToKeep];
            for (var j = 0; j < particlesToKeep; j++)
                keepParticles[j] = theParticles[_LiveParticles[j]];
            // And write changes back
            _Emitter.particles = keepParticles;
        }
    }
}