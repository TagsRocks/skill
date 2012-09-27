using UnityEngine;
using System.Collections;
using System;
using Skill.UI;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text area.
    /// </summary>
    public class TextArea : EditorControl
    {
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
        /// Occurs when text of TextArea changed
        /// </summary>
        public event EventHandler TextChanged;
        /// <summary>
        /// when text of TextArea changed
        /// </summary>
        protected virtual void OnTextChanged()
        {
            if (TextChanged != null) TextChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a TextArea
        /// </summary>
        public TextArea()
        {
            this._Text = string.Empty;
            this.Height = 38;
        }

        /// <summary>
        /// Render TextArea
        /// </summary>
        protected override void Render()
        {
            string result;
            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                result = EditorGUI.TextArea(RenderArea, Text, Style);
            }
            else
            {
                result = EditorGUI.TextArea(RenderArea, Text);
            }

            Text = result;
        }
    }

}