using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.UI;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a label field. (Useful for showing read-only info.)
    /// </summary>
    public class LabelField : Control
    {
        /// <summary>
        /// Label in front of the label field.
        /// </summary>
        public GUIContent Label { get; private set; }
        /// <summary>
        /// The label to show to the right.
        /// </summary>
        public GUIContent Label2 { get; private set; }
       
        /// <summary>
        /// Create an instance of LabelField
        /// </summary>
        public LabelField()
        {
            Label = new GUIContent();
            Label2 = new GUIContent();

        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                EditorGUI.LabelField(PaintArea, Label, Label2, Style);
            }
            else
            {
                EditorGUI.LabelField(PaintArea, Label, Label2);
            }
        }        
    }
}
