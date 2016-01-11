using UnityEngine;
using System.Collections;
using Skill.Framework;
using System.Collections.Generic;
using System;
namespace Skill.Framework.ModernUI
{
    public class PauseMenu : MenuManager
    {


        public static PauseMenu Instance { get; private set; }

        public MenuPage StartPage;
        /// <summary> Pause AudioListener</summary>
        public bool PauseAudioListener = true;
        /// <summary> Pause TimeScale</summary>
        public bool PauseTime = false;
        /// <summary> Pause when application paused</summary>
        public bool PauseOnApplicationPause = true;
        /// <summary> Pause when application lost focus</summary>
        public bool PauseOnApplicationLostFocus = false;
        /// <summary> Pause when Global.CutsceneEnable</summary>
        public bool PauseOnCutscene = false;

        private float _SavedTimeScale;
        private TimeWatch _StartDelayTW;

        protected override void OnExit()
        {
            base.OnExit();
            UnPauseGame();
        }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < transform.childCount; i++)
            {
                MenuPage p = transform.GetChild(i).GetComponent<MenuPage>();
                if (p != null)
                {
                    if (p.gameObject.activeSelf)
                        p.gameObject.SetActive(false);
                }
            }
        }


        /// <summary> Is GameMenu visible? </summary>
        public bool IsVisible { get; private set; }

        public bool PlayerQuit { get; private set; }

        protected virtual bool UserPressedMenuButton()
        {
            if (!IsVisible && !PauseOnCutscene && Global.CutSceneEnable) return false;
            return (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu));
        }

        protected override void Update()
        {
            if (_StartDelayTW.IsEnabled)
            {
                if (_StartDelayTW.IsOver)
                    _StartDelayTW.End();
            }
            else if (!PlayerQuit)
            {
                if (UserPressedMenuButton())
                {
                    if (IsVisible)
                    {
                        if (TopDialog == null)
                            Back();
                    }
                    else
                        PauseGame();
                }
            }
            base.Update();
        }

        /// <summary> Pause game </summary>
        public virtual void PauseGame()
        {
            if (IsVisible) return;
            IsVisible = true;
            _SavedTimeScale = Time.timeScale;
            if (PauseTime)
                Time.timeScale = 0;
            Global.IsGamePaused = true;
            if (PauseAudioListener)
                AudioListener.pause = Global.IsGamePaused;

            if (StartPage != null)
                ShowPage(StartPage);
        }

        /// <summary>
        /// resume 
        /// </summary>
        private void UnPauseGame()
        {
            if (!IsVisible) return;
            if (PauseTime)
                Time.timeScale = _SavedTimeScale;
            Global.IsGamePaused = false;
            if (PauseAudioListener)
                AudioListener.pause = Global.IsGamePaused;
            IsVisible = false;
        }

        void OnApplicationPause(bool pause)
        {
            if (PauseOnApplicationPause && pause && !IsVisible)
                PauseGame();
        }
        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                if (IsVisible)
                {
                    if (PauseTime)
                        Time.timeScale = 0;
                    Global.IsGamePaused = true;
                    if (PauseAudioListener)
                        AudioListener.pause = Global.IsGamePaused;
                }
            }
            else if (PauseOnApplicationLostFocus && !IsVisible)
            {
                PauseGame();
            }
        }
    }

}