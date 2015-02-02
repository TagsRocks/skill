using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an X, Y , Z field for entering a Vector3.
    /// </summary>
    public class Vector3Field : EditorControl
    {        
        /// <summary> Label to display above the field. </summary>        
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when value of Vector3Field changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when value of Vector3Field changed
        /// </summary>
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
            Label = new GUIContent();
            this.Height = 38;
        }

        /// <summary>
        /// Render Vector3Field
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Value = EditorGUI.Vector3Field(RenderArea, Label, _Value);
        }
    }
}
