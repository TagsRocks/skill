using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;


namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a selectable label field. (Useful for showing read-only info that can be copy-pasted.)
    /// </summary>
    public class SelectableLabel : EditorControl
    {
        /// <summary>
        /// The text to show.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Create a SelectableLabel
        /// </summary>
        public SelectableLabel()
        {
            this.Height = 16;
        }

        /// <summary>
        /// Render SelectableLabel
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                EditorGUI.SelectableLabel(RenderArea, Text, Style);
            }
            else
            {
                EditorGUI.SelectableLabel(RenderArea, Text);
            }
        }
    }
}
