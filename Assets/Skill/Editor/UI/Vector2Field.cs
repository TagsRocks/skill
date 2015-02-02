using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an X , Y field for entering a Vector2.
    /// </summary>
    public class Vector2Field : EditorControl
    {
        /// <summary> Label to display above the field. </summary>        
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when value of Vector2Field changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when value of Vector2Field changed
        /// </summary>
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
            Label = new GUIContent();
            this.Height = 38;
        }

        /// <summary>
        /// Render Vector2Field
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Value = EditorGUI.Vector2Field(RenderArea, Label, _Value);
        }
    }
}
