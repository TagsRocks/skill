using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
{
    /// <summary>
    /// Make a single press button. The user clicks them and something happens immediately.
    /// </summary>
    public class Button : Control
    {

        /// <summary>
        /// Text, image and tooltip for this button.
        /// </summary>
        public GUIContent Content { get; private set; }


        /// <summary>
        /// Occurs when users clicks the button
        /// </summary>
        public event EventHandler Click;
        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a Button
        /// </summary>
        public Button()
        {
            Content = new GUIContent();
        }

        /// <summary>
        /// Paint button
        /// </summary>
        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                if (GUI.Button(PaintArea, Content, Style))
                    OnClick();
            }
            else
            {
                if (GUI.Button(PaintArea, Content))
                    OnClick();
            }
        }

            }

}