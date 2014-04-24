using UnityEngine;
using System.Collections;

namespace Skill.Framework.Audio
{
    /// <summary>
    /// Listen to PitchController.Instance.Pitch
    /// </summary>
   
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
            RelinkAudios();
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

        /// <summary>
        /// if number of AudioSources attached to this gameobject is dynamic. call this before change pitch to find all audios again
        /// </summary>
        public void RelinkAudios()
        {
            _AudioSources = GetComponentsInChildren<AudioSource>();
        }
    }

}