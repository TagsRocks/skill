using UnityEngine;
using System.Collections;
using System;
using Skill.UI;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a toggle.
    /// </summary>
    public class ToggleButton : EditorControl
    {
        /// <summary>
        /// Optional label in front of the toggle.
        /// </summary>
        public GUIContent Label { get; private set; }        

        private bool _IsChecked;
        /// <summary>
        /// The shecked state of the toggle.
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
        /// <summary>
        /// when a ToggleButton is checked.
        /// </summary>
        protected virtual void OnChecked()
        {
            if (Checked != null)
                Checked(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when a ToggleButton is unchecked.
        /// </summary>
        public event EventHandler Unchecked;
        /// <summary>
        /// when a ToggleButton is unchecked.
        /// </summary>
        protected virtual void OnUnchecked()
        {
            if (Unchecked != null)
                Unchecked(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a ToggleButton
        /// </summary>
        public ToggleButton()
        {
            Label = new GUIContent();
            this.Height = 16;
        }

        /// <summary>
        /// Render ToggleButton
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                IsChecked = EditorGUI.Toggle(RenderArea, Label, IsChecked, Style);
            }
            else
            {
                IsChecked = EditorGUI.Toggle(RenderArea, Label, IsChecked);
            }
        }
        
    }
}