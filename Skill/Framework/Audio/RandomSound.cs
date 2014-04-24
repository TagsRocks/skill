using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Audio
{
    /// <summary>
    /// Play random sound on enable
    /// </summary>
    [RequireComponent(typeof(AudioSource))]    
    public class RandomSound : StaticBehaviour
    {
        /// <summary> Array of sounds to play in random </summary>
        public AudioClip[] Sounds;

        private AudioSource _Audio;

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Audio = audio;
            if (_Audio == null)
                Debug.LogWarning(string.Format("Can not find AudioSource for RandomSound behaviour : {0}", gameObject.name));
        }

        /// <summary>
        /// Play random sound
        /// </summary>
        public virtual void PlaySound()
        {
            if (_Audio != null && Sounds != null && Sounds.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, Sounds.Length);
                AudioClip randomSound = Sounds[index];

                if (randomSound != null)
                {
                    _Audio.PlayOneShot(randomSound);
                }
                else
                    Debug.LogWarning(string.Format("Invalid AudioClip with index : {0}, in GameObject : {1}", index, gameObject.name));
            }
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            PlaySound();
        }
    }
}
