using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an X & Y field for entering a Vector2.
    /// </summary>
    public class Vector2Field : EditorControl
    {
        private string _Label;
        /// <summary>
        /// Label to display above the field.
        /// </summary>
        public String Label
        {
            get { return _Label; }
            set
            {
                _Label = value;
                if (_Label == null)
                    _Label = string.Empty;
            }
        }

        /// <summary>
        /// Occurs when value of Vector2Field changed
        /// </summary>
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private Vector2 _Value;
        /// <summary>
        /// Vector2 - The value entered by the user.
        /// </summary>
        public Vector2 Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Create an instance of Vector2Field
        /// </summary>
        public Vector2Field()
        {
            _Label = string.Empty;
            this.Height = 38;
        }

        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Value = EditorGUI.Vector2Field(RenderArea, _Label, _Value);
        }
    }
}
