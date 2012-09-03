using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;


namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a selectable label field. (Useful for showing read-only info that can be copy-pasted.)
    /// </summary>
    public class SelectableLabel : Control
    {
        /// <summary>
        /// The text to show.
        /// </summary>
        public string Text { get; set; }        

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                EditorGUI.SelectableLabel(PaintArea, Text, Style);
            }
            else
            {
                EditorGUI.SelectableLabel(PaintArea, Text);
            }
        }
    }
}
