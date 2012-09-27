using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for masks.
    /// </summary>
    public class MaskField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when mask of MaskField changed
        /// </summary>
        public event EventHandler MaskChanged;
        /// <summary>
        /// when mask of MaskField changed
        /// </summary>
        protected virtual void OnMaskChanged()
        {
            if (MaskChanged != null) MaskChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// A string array containing the labels for each flag.
        /// </summary>
        public List<string> DisplayedOptions { get; private set; }

        private int _Mask;
        /// <summary>
        /// int - The value modified by the user.
        /// </summary>
        public int Mask
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
        /// Create an instance of MaskField
        /// </summary>
        public MaskField()
        {
            this.Label = new GUIContent();
            this.DisplayedOptions = new List<string>();
            this.Height = 16;
        }

        /// <summary>
        /// Render MaskField
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Mask = EditorGUI.MaskField(RenderArea, Label, _Mask, DisplayedOptions.ToArray(), Style);
            }
            else
            {
                Mask = EditorGUI.MaskField(RenderArea, Label, _Mask, DisplayedOptions.ToArray());
            }
        }
    }
}
