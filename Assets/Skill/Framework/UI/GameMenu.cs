using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary> Simple game menu that contains MenuFrames. can show on frame at time</summary>
    public class GameMenu : DynamicBehaviour
    {
        /// <summary> The sorting depth. </summary>
        public int GUIDepth = 13;
        /// <summary> Skin </summary>
        public GUISkin Skin;
        public string MainFrame = "MainFrame";
        public bool ShowOnAwake = false;

        /// <summary> Menu </summary>
        public Menu Menu { get; private set; }

        /// <summary> Start </summary>
        protected override void Start()
        {
            base.Start();

            MenuFrame[] pages = GetComponentsInChildren<MenuFrame>();
            if (pages != null && pages.Length > 0)
            {
                List<Frame> frameList = new List<Frame>();
                for (int i = 0; i < pages.Length; i++)
                {
                    pages[i].Menu = this;
                    frameList.Add(pages[i].Frame);
                }
                Menu = new Menu(frameList.ToArray());
            }
            else
            {
                Debug.LogError("Can not find any MenuFrame as child of GameMenu");
            }

            if (ShowOnAwake)
                Show();
        }

        /// <summary> Is GameMenu visible? </summary>
        public bool IsVisible { get; private set; }

        /// <summary> Show Game Menu </summary>
        protected void JustShow()
        {
            if (IsVisible) return;
            IsVisible = true;
        }

        /// <summary> Show Game Menu </summary>
        protected virtual void Show()
        {
            if (IsVisible) return;
            IsVisible = true;
            if (Menu != null && Menu.TopFrame == null && !string.IsNullOrEmpty(MainFrame))
                Menu.ShowFrame(MainFrame);
        }

        /// <summary> Hide Game Menu </summary>
        protected virtual void Hide()
        {
            if (!IsVisible) return;
            IsVisible = false;
        }



        /// <summary> Update </summary>
        protected override void Update()
        {
            if (IsVisible && Menu != null)
                Menu.Update();

            base.Update();
        }

        /// <summary> OnGUI </summary>
        protected virtual void OnGUI()
        {
            if (IsVisible && Menu != null)
            {
                GUI.depth = GUIDepth;
                if (Skin != null) GUI.skin = Skin;
                Menu.OnGUI();
            }
        }
    }
}
