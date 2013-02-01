using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Defines types of commands by Keyboard or Gamepad
    /// </summary>
    public enum KeyCommand
    {
        /// <summary> Nothing </summary>
        None,
        /// <summary> User pressed left direction </summary>
        Left,
        /// <summary> User pressed right direction </summary>
        Right,
        /// <summary> User pressed up direction </summary>
        Up,
        /// <summary> User pressed down direction </summary>
        Down,
        /// <summary> User pressed enter (or something else) to accept</summary>
        Enter,
        /// <summary> User pressed escape (or something else) to cancel. also usable as Back command </summary>
        Cancel,
        /// <summary> User pressed home </summary>
        Home,
        /// <summary> User pressed end </summary>
        End,                
    }

    /// <summary>
    /// Defines commands by Keyboard or Gamepad
    /// </summary>
    public class UICommand
    {
        /// <summary> Key command </summary>
        public KeyCommand Key { get; set; }
        /// <summary> Position of mouse </summary>
        public Vector2 MousePosition { get; set; }
        /// <summary> User holds control </summary>
        public bool Control { get; set; }
        /// <summary> Uset holds shift </summary>
        public bool Shift { get; set; }
        /// <summary> Uset holds alt </summary>
        public bool Alt { get; set; }

        /// <summary> Reset all variables </summary>
        public void Reset()
        {
            Key = KeyCommand.None;
            MousePosition = Vector2.zero;
            Control = Shift = Alt = false;
        }
    }
}
