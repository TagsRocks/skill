using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Reserved to pass some additional info for Painting
    /// </summary>
    public class PaintParameters
    {
        /// <summary>
        /// Screen offset to consider when check for mouse click or some similar actions
        /// </summary>
        /// <remarks>
        /// This value modified by EditorFrame class. so modify it carefully
        /// </remarks>
        public Vector2 ScreenOffset { get; set; }

        /// <summary>
        /// UserData
        /// </summary>
        public object UserData { get; set; }
    }
}
