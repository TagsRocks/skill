using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Provides global variables, events and methods to be shared between all objetcs
    /// you can name GameObject start with 'A' to be the first object created in scene
    /// </summary>    
    public class Global : StaticBehaviour
    {
        /// <summary>
        /// The only instance of Global object in scene
        /// </summary>
        public static Global Instance { get; private set; }


        private Settings _Settings;
        /// <summary> Settings </summary>
        public Settings Settings
        {
            get
            {
                if (_Settings == null)
                    _Settings = CreateSettings();
                return _Settings;
            }
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                base.Awake();
                Instance = this;
                IsGamePaused = false;
                if (Settings == null)
                    throw new Exception("Invalid Settings. you have to return valid setting by CreateSettings() method");
                DontDestroyOnLoad(this.gameObject);
            }
        }

        protected override void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            base.OnDestroy();

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

        #region Settings
        /// <summary>
        /// Allow subclass to instantiate another custom Settings
        /// </summary>
        /// <returns></returns>
        protected virtual Settings CreateSettings() { return new Settings(); }

        protected virtual string SettingsKey { get { return "KEY_SETTINGS"; } }
        public void SaveSettingsToPlayerPrefs(string password = null)
        {
            Skill.Framework.IO.PCSaveGame.SaveXmlToPlayerPrefs(this.Settings, SettingsKey, password);
        }
        public void LoadSettingsFromPlayerPrefs(string password = null)
        {
            bool hasKey;
            if (password == null)
                hasKey = PlayerPrefs.HasKey(SettingsKey);
            else
                hasKey = Skill.Framework.IO.SecurePlayerPrefs.HasKey(SettingsKey);
            if (hasKey)
                Skill.Framework.IO.PCSaveGame.LoadXmlFromPlayerPrefs(this.Settings, SettingsKey, password);
            //else
            //    Debug.LogWarning("Can not find settings data in PlayerPrefs");
        }

        protected virtual string SettingsFile { get { return System.IO.Path.Combine(Application.persistentDataPath, "Settings.xml"); } }
        public void SaveSettingsToFile(string password = null)
        {
            Skill.Framework.IO.PCSaveGame.SaveToXmlFile(this.Settings, SettingsFile, password);
        }
        public void LoadSettingsFromFile(string password = null)
        {
            if (System.IO.File.Exists(SettingsFile))
            {
                try
                {
                    Skill.Framework.IO.PCSaveGame.LoadFromXmlFile(this.Settings, SettingsFile, password);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            //else
            //    Debug.LogWarning("Can not find settings data in file");
        }
        #endregion


        // ************* Sounds **************

        /// <summary>
        /// Play sound
        /// </summary>
        /// <param name="source">Source of sound</param>
        /// <param name="clip">Sound to play</param>
        /// <param name="category">Category of sound</param>
        public virtual void PlayOneShot(AudioSource source, AudioClip clip, Skill.Framework.Audio.SoundCategory category)
        {
            if (source != null && clip != null)
            {
                float volume = Settings.Audio.GetVolume(category);
                source.PlayOneShot(clip, volume);
            }
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
        public static void RaiseCameraShake(object sender, CameraShakeParams info, Vector3 source)
        {
            if (CameraShake != null)
                CameraShake(sender, new CameraShakeEventArgs(new CameraShakeParams(info), source));
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
        public static void RaiseSlowMotion(object sender, SlowMotionInfo info)
        {
            if (SlowMotion != null)
                SlowMotion(sender, new SlowMotionEventArgs(new SlowMotionInfo(info)));
        }


        private static List<IControllerManager> _ControllerManagerList;

        /// <summary>
        /// Register an IControllerManager to be notified when Controller start or destroy
        /// </summary>
        /// <param name="host">IControllerManager to register</param>
        public static void Register(IControllerManager host)
        {
            if (host != null)
            {
                if (_ControllerManagerList == null)
                    _ControllerManagerList = new List<IControllerManager>();
                if (!_ControllerManagerList.Contains(host))
                    _ControllerManagerList.Add(host);
            }
        }
        /// <summary>
        /// UnRegister IControllerManager
        /// </summary>
        /// <param name="host">IControllerManager to unregister</param>
        public static bool UnRegister(IControllerManager host)
        {
            if (host != null && _ControllerManagerList != null)
            {
                return _ControllerManagerList.Remove(host);
            }
            return false;
        }

        internal static void Register(Controller controller)
        {
            if (controller != null)
            {
                if (_ControllerManagerList != null && _ControllerManagerList.Count > 0)
                {
                    foreach (var manager in _ControllerManagerList)
                    {
                        manager.Register(controller);
                    }
                }
            }
        }
        internal static bool UnRegister(Controller controller)
        {
            bool result = false;
            if (controller != null)
            {
                if (_ControllerManagerList != null && _ControllerManagerList.Count > 0)
                {
                    foreach (var manager in _ControllerManagerList)
                    {
                        result |= manager.UnRegister(controller);
                    }
                }
            }
            return result;
        }
    }
}
