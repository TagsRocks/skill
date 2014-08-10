using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field.
    /// </summary>
    public class TextField : EditorControl
    {
        /// <summary>
        /// Optional label to display in front of the text field.
        /// </summary>
        public GUIContent Label { get; private set; }

        private string _TextBeforChange;
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
                    _TextBeforChange = _Text = value;
                    OnTextChanged();
                }
            }
        }

        /// <summary>
        /// TextChanged event occurs when user press Return
        /// </summary>
        public bool ChangeOnReturn { get; set; }

        /// <summary>
        /// Occurs when Text of TextField changed
        /// </summary>
        public event EventHandler TextChanged;
        /// <summary>
        /// when Text of TextField changed
        /// </summary>
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
            this.Height = 16;
            this.ChangeOnReturn = false;
        }

        /// <summary>
        /// Render TextField
        /// </summary>
        protected override void Render()
        {
            string result;

            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);

            if (Style != null)
            {
                if (ChangeOnReturn)
                    _TextBeforChange = EditorGUI.TextField(RenderArea, Label, _TextBeforChange, Style);
                else
                    Text = EditorGUI.TextField(RenderArea, Label, _Text, Style);
            }
            else
            {
                if (ChangeOnReturn)
                    _TextBeforChange = EditorGUI.TextField(RenderArea, Label, _TextBeforChange);
                else
                    Text = EditorGUI.TextField(RenderArea, Label, _Text);
            }

            if (ChangeOnReturn)
            {
                if (_Text != _TextBeforChange)
                {
                    Event e = Event.current;
                    if (e != null && e.isKey)
                    {
                        if (e.keyCode == KeyCode.Return)
                        {
                            Text = _TextBeforChange;
                        }
                        else if (e.keyCode == KeyCode.Escape)
                        {
                            _TextBeforChange = _Text;
                        }
                    }
                }
            }
        }

    }
}