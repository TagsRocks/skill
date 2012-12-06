using UnityEngine;
using System.Collections;

namespace Skill.Sounds
{
    /// <summary>
    /// Listen to PitchController.Instance.Pitch
    /// </summary>
    [AddComponentMenu("Skill/Sounds/PitchListener")]
    public class PitchListener : DynamicBehaviour
    {
        private AudioSource[] _AudioSources;
        private float _Pitch;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _AudioSources = GetComponents<AudioSource>();
        }

        /// <summary>
        /// Initialization
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (PitchController.Instance != null)
                _Pitch = PitchController.Instance.Pitch;
            else
            {
                _Pitch = 1.0f;
                Debug.LogWarning("There is no instance of PitchController");
            }
            ApplyPitch();
        }

        private void ApplyPitch()
        {
            if (_AudioSources != null)
            {
                foreach (var audio in _AudioSources)
                {
                    audio.pitch = _Pitch;
                }
            }
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        protected override void Update()
        {
            if (PitchController.Instance != null)
            {
                if (_Pitch != PitchController.Instance.Pitch)
                {
                    _Pitch = PitchController.Instance.Pitch;
                    ApplyPitch();
                }
            }            
            base.Update();
        }
    }

}