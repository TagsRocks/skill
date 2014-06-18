using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    public class PauseMenu : GameMenu
    {
        public bool PauseAudioListener = true;
        public bool PauseTime = false;
        public bool PauseOnApplicationPause = true;

        private float _SavedTimeScale;

        protected override void Start()
        {
            base.Start();
            Menu.Exit += Menu_Exit;
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
            else if (PauseOnApplicationPause && !IsVisible)
            {
                PauseGame();
            }
        }
    }
}
