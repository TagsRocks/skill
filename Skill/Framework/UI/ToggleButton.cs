using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Make an on/off toggle button.
    /// </summary>
    public class ToggleButton : FocusableControl
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
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when a ToggleButton is checked.
        /// </summary>
        public event EventHandler Checked;
        /// <summary>
        /// Occurs when a ToggleButton is checked.
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
        /// Occurs when a ToggleButton is unchecked.
        /// </summary>
        protected virtual void OnUnchecked()
        {
            if (Unchecked != null)
                Unchecked(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when a ToggleButton is Changed.
        /// </summary>
        public event EventHandler Changed;
        /// <summary>
        /// when a ToggleButton is Changed.
        /// </summary>
        protected virtual void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        private ToggleButtonGroup _Group;
        /// <summary>
        /// only one of ToggleButtons inside a group is checked
        /// </summary>
        public ToggleButtonGroup Group
        {
            get { return _Group; }
            set
            {
                if (_Group != null)
                    _Group.Remove(this);
                _Group = value;
                if (_Group != null)
                    _Group.Add(this);
            }
        }

        /// <summary>
        /// Create an instance of ToggleButton
        /// </summary>
        public ToggleButton()
        {
            Content = new GUIContent();
        }

        /// <summary>
        /// Render ToggleButton
        /// </summary>
        protected override void Render()
        {
            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                IsChecked = GUI.Toggle(RenderArea, _IsChecked, Content, Style);
            }
            else
            {
                IsChecked = GUI.Toggle(RenderArea, _IsChecked, Content);
            }
        }

        /// <summary>
        /// Handle specified command. toggle button respond to Enter command
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>   
        public override bool HandleCommand(UICommand command)
        {
            if (command.Key == KeyCommand.Enter)
            {
                IsChecked = !IsChecked;
                return true;
            }
            return base.HandleCommand(command);
        }
    }
}