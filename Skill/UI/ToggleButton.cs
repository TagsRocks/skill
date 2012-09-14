using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
{
    /// <summary>
    /// Make an on/off toggle button.
    /// </summary>
    public class ToggleButton : Control
    {
        /// <summary>
        /// Text, image and tooltip for this button.
        /// </summary>
        public GUIContent Content { get; private set; }

        private bool _IsChecked;
        /// <summary>
        /// Gets or sets whether the ToggleButton is checked.
        /// </summary>
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    if (_IsChecked)
                        OnChecked();
                    else
                        OnUnchecked();
                }
            }
        }

        /// <summary>
        /// Occurs when a ToggleButton is checked.
        /// </summary>
        public event EventHandler Checked;
        protected virtual void OnChecked()
        {
            if (Checked != null)
                Checked(this, EventArgs.Empty);
        }
        /// <summary>
        /// Occurs when a ToggleButton is unchecked.
        /// </summary>
        public event EventHandler Unchecked;
        protected virtual void OnUnchecked()
        {
            if (Unchecked != null)
                Unchecked(this, EventArgs.Empty);
        }


        /// <summary>
        /// Create an instance of ToggleButton
        /// </summary>
        public ToggleButton()
        {
            Content = new GUIContent();
        }

        protected override void Paint(PaintParameters paintParams)
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                IsChecked = GUI.Toggle(PaintArea, _IsChecked, Content, Style);
            }
            else
            {
                IsChecked = GUI.Toggle(PaintArea, _IsChecked, Content);
            }
        }        
    }
}