using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an X, Y & Z field for entering a Vector3.
    /// </summary>
    public class Vector3Field : EditorControl
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
        /// Occurs when value of Vector3Field changed
        /// </summary>
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private Vector3 _Value;
        /// <summary>
        /// Vector3 - The value entered by the user.
        /// </summary>
        public Vector3 Value
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
        /// Create an instance of Vector3Field
        /// </summary>
        public Vector3Field()
        {
            _Label = string.Empty;
            this.Height = 38;
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Value = EditorGUI.Vector3Field(PaintArea, _Label, _Value);
        }
    }
}
