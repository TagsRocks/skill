using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
{
    /// <summary>
    /// Make a button that is active as long as the user holds it down.
    /// </summary>
    public class RepeatButton : Control
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
        protected virtual void OnDown()
        {
            if (Down != null)
                Down(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when first time button is up
        /// </summary>
        public event EventHandler Up;
        protected virtual void OnUp()
        {
            if (Up != null)
                Up(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs each frame until it gets up
        /// </summary>
        public event EventHandler Repeat;
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

        protected override void Paint(PaintParameters paintParams)
        {
            bool result;
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                result = GUI.RepeatButton(PaintArea, Content, Style);
            }
            else
            {
                result = GUI.RepeatButton(PaintArea, Content);
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
    }

}