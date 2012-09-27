using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a text field for entering ints.
    /// </summary>
    public class IntField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }      

        /// <summary>
        /// Occurs when Value of IntField changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when Value of IntField changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private int _Value;
        /// <summary>
        /// int - The value entered by the user.
        /// </summary>
        public int Value
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
        /// Create an instance of IntField
        /// </summary>
        public IntField()
        {
            Label = new GUIContent();
            this.Height = 16;
        }

        /// <summary>
        /// Render IntField
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Value = EditorGUI.IntField(RenderArea, Label, _Value, Style);
            }
            else
            {
                Value = EditorGUI.IntField(RenderArea, Label, _Value);
            }
        }        
    }
}
