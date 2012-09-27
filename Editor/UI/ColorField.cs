using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for selecting a Color.
    /// </summary>
    public class ColorField : EditorControl
    {
        /// <summary>
        /// Optional label to display above the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when Color of ColorField changed
        /// </summary>
        public event EventHandler ColorChanged;
        /// <summary>
        /// when Color of ColorField changed
        /// </summary>
        protected virtual void OnColorChanged()
        {
            if (ColorChanged != null) ColorChanged(this, EventArgs.Empty);
        }

        private Color _Color;
        /// <summary>
        /// Color - The color selected by the user.
        /// </summary>
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    OnColorChanged();
                }
            }
        }

        /// <summary>
        /// Create a ColorField
        /// </summary>
        public ColorField()
        {
            Label = new GUIContent();
            this.Height = 16;
        }

        /// <summary>
        /// Render ColorField
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Color = EditorGUI.ColorField(RenderArea, Label, _Color);
        }
    }
}
