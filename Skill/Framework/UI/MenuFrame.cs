using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Base class for frames to use in GameMenu. you have to inherite this and implement CreateUI() method and create required controls and add them to Frame
    /// this GameObject must be child of GameMenu
    /// </summary>
    /// <remarks>
    /// Because each Frame requires some specific textures and skins, i decided to separate each frame to GameObjects for better management.
    /// </remarks>
    public abstract class MenuFrame : DynamicBehaviour
    {
        /// <summary> Unique name of of frame </summary>
        public string FrameName = "Page1";
        /// <summary> Location of frame in screen </summary>
        public FrameLocation Location = FrameLocation.Fill;
        /// <summary> Width of frame if Location is manual </summary>
        public float Width = 300;
        /// <summary> Height of frame if Location is manual </summary>
        public float Height = 400;

        /// <summary> Frame </summary>
        public Frame Frame { get; private set; }

        private int _ScreenWidth;
        private int _ScreenHeight;

        /// <summary> Awake </summary>
        protected override void Awake()
        {
            base.Awake();
            Frame = CreateUI();
        }

        /// <summary> Create required controls and add them to Frame </summary>
        /// <returns>Root frame of ui</returns>
        protected abstract Frame CreateUI();


        /// <summary> Update </summary>
        protected override void Update()
        {
            if (Location == FrameLocation.Manual)
            {
                Rect position = Frame.Position;
                Frame.Position = new Rect(position.x, position.y, Width, Height);
            }
            else
            {
                if (_ScreenWidth != Screen.width || _ScreenHeight != Screen.height)
                {
                    _ScreenWidth = Screen.width;
                    _ScreenHeight = Screen.height;
                    if (Width < 0) Width = 0;
                    if (Height < 0) Height = 0;

                    if (Location == FrameLocation.Fill)
                        Frame.Position = new Rect(0, 0, _ScreenWidth, _ScreenHeight);
                    else if (Location == FrameLocation.Center)
                        Frame.Position = new Rect((_ScreenWidth - Width) * 0.5f, (_ScreenHeight - Height) * 0.5f, Width, Height);
                }
            }
            base.Update();
        }
    }
}
