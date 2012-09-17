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
    public class PrefixLabel : EditorControl
    {
        /// <summary>
        /// Label to show in front of the control.
        /// </summary>
        public GUIContent Label { get; private set; }
        /// <summary>
        /// The unique ID of the control.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Rectangle on the screen to use just for the control itself.
        /// </summary>
        public Rect Result { get; private set; }

        /// <summary>
        /// Create an instance of PrefixLabel
        /// </summary>
        public PrefixLabel()
        {
            Label = new GUIContent();
            this.Height = 16;
        }

        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Result = EditorGUI.PrefixLabel(RenderArea, Id, Label);
        }
    }
}
