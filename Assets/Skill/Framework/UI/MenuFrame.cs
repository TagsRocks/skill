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
        /// <summary> Location of frame in screen </summary>
        public FrameLocation Location = FrameLocation.Fill;
        /// <summary> Width of frame if Location is manual </summary>
        public float Width = 300;
        /// <summary> Height of frame if Location is manual </summary>
        public float Height = 400;

        private Frame _Frame;
        /// <summary> Frame </summary>
        public Frame Frame
        {
            get
            {
                if (_Frame == null)
                    _Frame = CreateUI(gameObject.name);
                return _Frame;
            }
        }
        public GameMenu Menu { get; internal set; }

        private int _ScreenWidth;
        private int _ScreenHeight;
        private FrameLocation _Location = FrameLocation.Manual;


        /// <summary> Create required controls and add them to Frame </summary>
        /// <param name="frameName">name of frame</param>
        protected abstract UI.Frame CreateUI(string frameName);


        /// <summary> Update </summary>
        protected override void Update()
        {
            if (_Frame != null)
            {
                if (Location == FrameLocation.Manual)
                {
                    Rect position = _Frame.Position;
                    _Frame.Position = new Rect(position.x, position.y, Width, Height);
                }
                else
                {
                    if (_Location != Location || _ScreenWidth != Screen.width || _ScreenHeight != Screen.height)
                    {
                        _Location = Location;
                        _ScreenWidth = Screen.width;
                        _ScreenHeight = Screen.height;
                        if (Width < 0) Width = 0;
                        if (Height < 0) Height = 0;

                        if (Location == FrameLocation.Fill)
                            _Frame.Position = new Rect(0, 0, _ScreenWidth, _ScreenHeight);
                        else if (Location == FrameLocation.Center)
                            _Frame.Position = new Rect((_ScreenWidth - Width) * 0.5f, (_ScreenHeight - Height) * 0.5f, Width, Height);
                    }
                }
            }
            base.Update();
        }
    }
}
