using Skill.Framework.UI;
using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// a simple class to manage load levels in loading menu
    /// </summary>
    [RequireComponent(typeof(FadeScreen))]
    public class LevelLoadManager : Skill.Framework.DynamicBehaviour
    {
        /// <summary> Load level in editor mode or just fadeout </summary>
        public bool LoadInEditor;

        // next level key
        private const string NextLevelKey = "NLToL";
        /// <summary> Attached FadeScreen component </summary>
        public FadeScreen FadeScreen { get { return _FadeScreen; } }

        private FadeScreen _FadeScreen;
        private TimeWatch _QuitTW;
        private TimeWatch _LoadLevelTW;
        private string _TargetLevel;

        /// <summary> Name of loading scene (default : Loading) </summary>
        public virtual string LoadingSceneName { get { return "Loading"; } }

        /// <summary> GetReferences </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _FadeScreen = GetComponent<FadeScreen>();
        }
        /// <summary> Update </summary>
        protected override void Update()
        {
            if (_QuitTW.IsEnabledAndOver)
            {
                _QuitTW.End();
                if (Application.isEditor)
                    Debug.Log("Quit");
                else
                    Application.Quit();
            }
            if (_LoadLevelTW.IsEnabledAndOver)
            {
                Time.timeScale = 1.0f;
                _LoadLevelTW.End();
                if (!Application.isEditor || LoadInEditor)
                    Application.LoadLevel(_TargetLevel);
                else
                    Debug.Log(string.Format("Load Level : {0}", _TargetLevel));
            }
            base.Update();
        }

        /// <summary>
        /// Load level after fadeout
        /// </summary>
        /// <param name="levelName">Name of next level to load</param>
        /// <param name="loading">load level async in loading menu</param>
        public virtual void LoadLevel(string levelName, bool loading = true)
        {
            Time.timeScale = 1.0f;
            Skill.Framework.Global.IsGamePaused = false;
            Skill.Framework.Global.CutSceneEnable = false;
            if (loading)
                _TargetLevel = LoadingSceneName;
            else
                _TargetLevel = levelName;
            PlayerPrefs.SetString(NextLevelKey, levelName);
            FadeOut();
            _LoadLevelTW.Begin(_FadeScreen.Fading.FadeOutTime + 1.5f, true);
        }

        /// <summary> Quit application after fadeout </summary>
        public virtual void Quit()
        {
            FadeOut();
            _QuitTW.Begin(_FadeScreen.Fading.FadeOutTime + 1.5f, true);
        }

        private void FadeOut()
        {
            _FadeScreen.Fading.FadeOut();
            Skill.Framework.Audio.DynamicSoundVolume[] sounds = FindObjectsOfType<Skill.Framework.Audio.DynamicSoundVolume>();
            if (sounds != null)
            {
                SmoothingParameters fading = new SmoothingParameters() { SmoothTime = _FadeScreen.Fading.FadeOutTime - 0.1f, SmoothType = SmoothType.Damp };
                foreach (var item in sounds)
                {
                    item.Fading = fading;
                    item.VolumeFactor = 0;
                }
            }

        }

        /// <summary> Next level to load(in loading scene) </summary>
        public static string NextLevel
        {
            get
            {
                if (PlayerPrefs.HasKey(NextLevelKey))
                    return PlayerPrefs.GetString(NextLevelKey);
                return Application.loadedLevelName;
            }
        }
    }
}
