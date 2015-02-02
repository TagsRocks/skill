using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Make a button that is active as long as the user holds it down.
    /// </summary>
    public class RepeatButton : FocusableControl
    {
        private bool _IsDown;

        /// <summary>
        /// Text, image and tooltip for this button.
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary>
        /// Occurs when first time button is down
        /// </summary>
        public event EventHandler Down;
        /// <summary>
        /// when first time button is down
        /// </summary>
        protected virtual void OnDown()
        {
            if (Down != null)
                Down(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when first time button is up
        /// </summary>
        public event EventHandler Up;
        /// <summary>
        /// when first time button is up
        /// </summary>
        protected virtual void OnUp()
        {
            if (Up != null)
                Up(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs each frame until it gets up
        /// </summary>
        public event EventHandler Repeat;
        /// <summary>
        /// each frame until it gets up
        /// </summary>
        protected virtual void OnRepeat()
        {
            if (Repeat != null)
                Repeat(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create an instance of RepeatButton
        /// </summary>
        public RepeatButton()
        {
            Content = new GUIContent();
        }

        /// <summary>
        /// Render RepeatButton
        /// </summary>
        protected override void Render()
        {
            bool result;
            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                result = GUI.RepeatButton(RenderArea, Content, Style);
            }
            else
            {
                result = GUI.RepeatButton(RenderArea, Content);
            }

            if (result)
            {
                if (_IsDown == false)
                    OnDown();
                _IsDown = true;
                OnRepeat();
            }
            else
            {
                if (_IsDown == true)
                    OnUp();
                _IsDown = false;
            }
        }

        /// <summary>
        /// Handle specified command. button respond to Enter command
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>   
        public override bool HandleCommand(UICommand command)
        {
            if (command.Key == KeyCommand.Enter)
            {
                if (_IsDown == false)
                    OnDown();
                _IsDown = true;
                OnRepeat();
                return true;
            }
            else
            {
                if (_IsDown == true)
                {
                    OnUp();
                    _IsDown = false;
                    return true;
                }
            }
            return base.HandleCommand(command);
        }
    }

}