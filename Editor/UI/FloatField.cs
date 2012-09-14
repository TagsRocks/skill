using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field for entering floats.
    /// </summary>
    public class FloatField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }        

        /// <summary>
        /// Occurs when value of FloatField changed
        /// </summary>
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private float _Value;
        /// <summary>
        /// float - The value entered by the user.
        /// </summary>
        public float Value
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
        /// Create an instance of FloatField
        /// </summary>
        public FloatField()
        {
            Label = new GUIContent();
            this.Height = 16;
        }

        protected override void Paint(PaintParameters paintParams)
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Value = EditorGUI.FloatField(PaintArea, Label, _Value, Style);
            }
            else
            {
                Value = EditorGUI.FloatField(PaintArea, Label, _Value);
            }
        }        
    }
}
