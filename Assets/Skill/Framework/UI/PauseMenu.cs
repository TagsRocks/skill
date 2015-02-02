using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Implements functionality required to pause and resume game
    /// </summary>
    public class PauseMenu : GameMenu
    {
        /// <summary> Cursor object to be activated on pause and deactivate on resume </summary>
        public GameObject Cursor;
        /// <summary> Pause AudioListener</summary>
        public bool PauseAudioListener = true;
        /// <summary> Pause TimeScale</summary>
        public bool PauseTime = false;
        /// <summary> Pause when application paused</summary>
        public bool PauseOnApplicationPause = true;
        /// <summary> Pause when application lost focus</summary>
        public bool PauseOnApplicationLostFocus = false;

        private float _SavedTimeScale;

        protected override void Start()
        {
            base.Start();
            Menu.Exit += Menu_Exit;
            if (Cursor != null) Cursor.SetActive(false);
        }

        void Menu_Exit(object sender, System.EventArgs e)
        {
            UnPauseGame();
        }


        protected virtual bool UserPressedMenuButton()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        protected override void Update()
        {
            if (Global.CutSceneEnable)
            {
                if (IsVisible)
                {
                    if (Menu.TopDialog == null)
                        Menu.Back();
                }
            }
            else if (UserPressedMenuButton())
            {
                if (IsVisible)
                {
                    if (Menu.TopDialog == null)
                        Menu.Back();
                }
                else
                    PauseGame();
            }
            base.Update();
        }

        /// <summary> Same as Show() </summary>
        public void PauseGame() { Show(); }
        /// <summary> Show Game Menu </summary>
        protected override void Show()
        {
            if (IsVisible) return;
            _SavedTimeScale = Time.timeScale;
            if (PauseTime)
                Time.timeScale = 0;
            Global.IsGamePaused = true;
            if (PauseAudioListener)
                AudioListener.pause = Global.IsGamePaused;

            if (Cursor != null) Cursor.SetActive(true);
            base.Show();

        }

        /// <summary>
        /// Same as Hide()
        /// </summary>
        public void UnPauseGame() { Hide(); }

        /// <summary> Hide Game Menu </summary>
        protected override void Hide()
        {
            if (!IsVisible) return;
            if (PauseTime)
                Time.timeScale = _SavedTimeScale;
            Global.IsGamePaused = false;
            if (PauseAudioListener)
                AudioListener.pause = Global.IsGamePaused;
            if (Cursor != null) Cursor.SetActive(false);
            base.Hide();
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
