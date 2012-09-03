using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for enum based masks.
    /// </summary>
    public class EnumMaskField : Control
    {
        /// <summary>
        ///  Optional label to display above the EnumMaskField.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when Mask of EnumMaskField changed
        /// </summary>
        public event EventHandler MaskChanged;
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
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Mask = EditorGUI.EnumMaskField(PaintArea, Label, _Mask, Style);
            }
            else
            {
                Mask = EditorGUI.EnumMaskField(PaintArea, Label, _Mask);
            }
        }
    }
}
