using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Audio
{
    /// <summary> Set volume of all audios on enable </summary>
    public class SoundVolume : StaticBehaviour
    {
        /// <summary> Category of sound </summary>
        public Skill.Framework.Audio.SoundCategory Category = Skill.Framework.Audio.SoundCategory.FX;
        /// <summary> Volume factor </summary>
        public float VolumeFactor = 1;

        private AudioSource[] _Audios;

        protected override void GetReferences()
        {
            base.GetReferences();
            _Audios = GetComponents<AudioSource>();
        }

        private void SetVolume()
        {
            float volume = 0;
            if (Global.Instance != null)
                volume = Global.Instance.Settings.Audio.GetVolume(Category) * VolumeFactor;
            foreach (var audio in _Audios)
            {
                if (audio != null)
                    audio.volume = volume;
            }

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            SetVolume();
        }
    }

    
}
