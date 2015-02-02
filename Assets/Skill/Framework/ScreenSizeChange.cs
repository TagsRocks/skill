using System;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary>
    /// Keep track of screen size and notify when is changed
    /// </summary>
    public struct ScreenSizeChange
    {
        private int _Width;
        /// <summary> width of screen after last 'IsChanged' call</summary>
        public int Width { get { return _Width; } }

        private int _Height;
        /// <summary> Height  of screen after last 'IsChanged' call</summary>
        public int Height { get { return _Height; } }

        /// <summary>
        /// Check if screen size is changed. return true at first call and return false until screen size changed again.
        /// </summary>
        public bool IsChanged
        {
            get
            {
                if (Screen.width != _Width || Screen.height != _Height)
                {
                    _Width = Screen.width;
                    _Height = Screen.height;
                    return true;
                }
                return false;
            }
        }
    }
}
