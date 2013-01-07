using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make an X, Y, W and H field for entering a Rect.
    /// </summary>
    public class RectField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when Value of RectField changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when Value of RectField changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private Rect _Value;
        /// <summary>
        /// Rect - The value entered by the user.
        /// </summary>
        public Rect Value
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
        /// Create an instance of RectField
        /// </summary>
        public RectField()
        {
            Label = new GUIContent();
            this.Height = 58;
        }

        /// <summary>
        /// Render RectField
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Value = EditorGUI.RectField(RenderArea, Label, _Value);
        }
    }
}
