using UnityEngine;
using System.Collections;
using System;
using Skill.Framework.UI;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a toggle.
    /// </summary>
    public class ToggleButton : EditorControl, IToggleButton
    {
        /// <summary>
        /// Optional label in front of the toggle.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Make a toggle field where the toggle is to the left and the label immediately to the right of it.
        /// </summary>
        public bool Left { get; set; }

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
                    OnChanged();
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
        private void OnChecked()
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
        private void OnUnchecked()
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

            if (Left)
            {
                if (Style != null)
                    IsChecked = EditorGUI.ToggleLeft(RenderArea, Label, IsChecked, Style);
                else
                    IsChecked = EditorGUI.ToggleLeft(RenderArea, Label, IsChecked);
            }
            else
            {
                if (Style != null)
                    IsChecked = EditorGUI.Toggle(RenderArea, Label, IsChecked, Style);
                else
                    IsChecked = EditorGUI.Toggle(RenderArea, Label, IsChecked);
            }
        }

    }
}