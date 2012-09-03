using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using Skill.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field.
    /// </summary>
    public class TextField : FocusableControl
    {
        /// <summary>
        /// Optional label to display in front of the text field.
        /// </summary>
        public GUIContent Label { get; private set; }

        private string _Text;
        /// <summary>
        /// The text to edit.
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (_Text != value)
                {
                    _Text = value;
                    OnTextChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when Text of TextField changed
        /// </summary>
        public event EventHandler TextChanged;
        protected virtual void OnTextChanged()
        {
            if (TextChanged != null) TextChanged(this, EventArgs.Empty);
        }        

        /// <summary>
        /// Create a TextField
        /// </summary>
        public TextField()
        {
            this.Label = new GUIContent();
            this._Text = string.Empty;            
        }

        protected override void Paint()
        {
            string result;

            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);

            if (Style != null)
            {
                result = EditorGUI.TextField(PaintArea, Label, Text, Style);
            }
            else
            {
                result = EditorGUI.TextField(PaintArea, Label, Text);
            }

            Text = result;
        }
        
    }
}