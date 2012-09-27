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
        /// <summary>
        /// when Value of EnumPopup changed
        /// </summary>
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

        /// <summary>
        /// Render EnumPopup
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Value = EditorGUI.EnumPopup(RenderArea, Label, _Value, Style);
            }
            else
            {
                Value = EditorGUI.EnumPopup(RenderArea, Label, _Value);
            }
        }        
    }
}
