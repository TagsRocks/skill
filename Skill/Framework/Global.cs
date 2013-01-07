using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Provides global variables, events and methods to be shared between all objetcs
    /// </summary>
    [AddComponentMenu("Skill/Base/Global")]
    public class Global : StaticBehaviour
    {
        /// <summary>
        /// The only instance of Global object in scene
        /// </summary>
        public static Global Instance { get; private set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (Instance != null)
                Debug.LogWarning("More thn on instance of Skill.Global object found");
            Instance = this;
        }


        // ******** Static Properties *********       

        /// <summary> 
        /// Static shared variable to allow behaviours know about whether game is paused(PauseMenu is visible) or not.
        /// update this value value when you pause/resume game, do to PauseMenu or any reasons.
        /// </summary>
        public static bool IsGamePaused { get; set; }

        /// <summary> 
        /// Static shared variable to allow behaviours know about whether game is in cutscene mode.
        /// update this value value when you enter/exist cutscene.
        /// </summary>
        public static bool CutSceneEnable { get; set; }

        // ********** Initialization **********

        /// <summary>
        /// Overriding this allow programmers to modify all instances of Skill.Dynamics.Explosive
        /// </summary>
        /// <param name="explosive"> Dynamics.Explosive to initialize </param>
        public virtual void Initialize(Skill.Framework.Dynamics.Explosive explosive) { }


        // ************* Sounds **************

        /// <summary>
        /// Play sound
        /// </summary>
        /// <param name="source">Source of sound</param>
        /// <param name="clip">Sound to play</param>
        /// <param name="category">Category of sound</param>
        public virtual void PlayOneShot(AudioSource source, AudioClip clip, Skill.Framework.Sounds.SoundCategory category)
        {
            float volume = 1.0f;

            switch (category)
            {
                case Skill.Framework.Sounds.SoundCategory.FX:
                    volume = GameSettings.Audio.FxVolume;
                    break;
                case Skill.Framework.Sounds.SoundCategory.Music:
                    volume = GameSettings.Audio.MusicVolume;
                    break;
                case Skill.Framework.Sounds.SoundCategory.Dialog:
                    volume = GameSettings.Audio.DialogVolume;
                    break;
                default:
                    break;
            }
            source.PlayOneShot(clip, volume * GameSettings.Audio.MasterVolume);
        }

        // ********** static events **********

        /// <summary>
        /// Occurs when a CameraShake happened
        /// </summary>
        public static event CameraShakeEventHandler CameraShake;

        /// <summary>
        /// Notify globla that a CameraShake happened
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="info"> Camera shake information </param>
        /// <param name="source"> Source of shake </param>
        public static void OnCameraShake(object sender, CameraShakeInfo info, Vector3 source)
        {
            if (CameraShake != null)
                CameraShake(sender, new CameraShakeEventArgs(new CameraShakeInfo(info), source));
        }


        /// <summary>
        /// Occurs when a SlowMotion happened
        /// </summary>
        public static event SlowMotionEventHandler SlowMotion;

        /// <summary>
        /// Notify globla that a SlowMotion happened
        /// </summary>
        /// <param name="sender"> The source of the event. </param>
        /// <param name="info"> Slow motion information</param>
        public static void OnSlowMotion(object sender, SlowMotionInfo info)
        {
            if (SlowMotion != null)
                SlowMotion(sender, new SlowMotionEventArgs(new SlowMotionInfo(info)));
        }
    }
}
