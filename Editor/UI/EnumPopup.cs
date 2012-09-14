using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Skill.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an enum popup selection field.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumPopup : EditorControl 
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        private Enum _Value;
        /// <summary>
        /// The enum option the field shows.
        /// </summary>
        public Enum Value
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
        /// Occurs when Value of EnumPopup changed
        /// </summary>
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create an instance of EnumPopup
        /// </summary>
        public EnumPopup()
        {
            this.Label = new GUIContent();
            this.Height = 16;           
        }

        protected override void Paint(PaintParameters paintParams)
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Value = EditorGUI.EnumPopup(PaintArea, Label, _Value, Style);
            }
            else
            {
                Value = EditorGUI.EnumPopup(PaintArea, Label, _Value);
            }
        }        
    }
}
