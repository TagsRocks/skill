using System;
using System.Collections.Generic;
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
        /// <summary> User pressed tab </summary>
        Tab,
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

        /// <summary>
        /// Get default input data from keyboard
        /// </summary>
        /// <returns>True if at least one key command is occured</returns>
        public bool GetDefaultInput()
        {
            Reset();

            this.MousePosition = Input.mousePosition;
            this.Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            this.Control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            this.Alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

            bool handleCommand = this.Shift || this.Control || this.Control;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.Key = KeyCommand.Cancel;
                handleCommand = true;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.Key = KeyCommand.Up;
                handleCommand = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.Key = KeyCommand.Down;
                handleCommand = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.Key = KeyCommand.Left;
                handleCommand = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.Key = KeyCommand.Right;
                handleCommand = true;
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                this.Key = KeyCommand.Tab;
                handleCommand = true;
            }
            else if (Input.GetKeyDown(KeyCode.Home))
            {
                this.Key = KeyCommand.Home;
                handleCommand = true;
            }
            else if (Input.GetKeyDown(KeyCode.End))
            {
                this.Key = KeyCommand.End;
                handleCommand = true;
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                this.Key = KeyCommand.Enter;
                handleCommand = true;
            }

            return handleCommand;
        }
    }
}
