using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for enum based masks.
    /// </summary>
    public class EnumMaskField : EditorControl
    {
        /// <summary>
        ///  Optional label to display above the EnumMaskField.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when Mask of EnumMaskField changed
        /// </summary>
        public event EventHandler MaskChanged;
        /// <summary>
        /// when Mask of EnumMaskField changed
        /// </summary>
        protected virtual void OnMaskChanged()
        {
            if (MaskChanged != null) MaskChanged(this, EventArgs.Empty);
        }        

        private Enum _Mask;
        /// <summary>
        /// System.Enum - The value modified by the user.
        /// </summary>
        public Enum Mask
        {
            get { return _Mask; }
            set
            {
                if (_Mask != value)
                {
                    _Mask = value;
                    OnMaskChanged();                    
                }
            }
        }


        /// <summary>
        /// Create an instance of EnumMaskField
        /// </summary>
        public EnumMaskField()
        {
            this.Label = new GUIContent();
            this.Height = 16;
        }

        /// <summary>
        /// Render EnumMaskField
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Mask = EditorGUI.EnumMaskField(RenderArea, Label, _Mask, Style);
            }
            else
            {
                Mask = EditorGUI.EnumMaskField(RenderArea, Label, _Mask);
            }
        }
    }
}
