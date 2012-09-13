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
    /// Draws a label with a drop shadow.
    /// </summary>
    public class DropShadowLabel : EditorControl
    {
        /// <summary>
        /// text to show
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary>
        /// text to show 
        /// </summary>
        public string Text
        {
            get { return Content.text; }
            set { Content.text = value; }
        }

        /// <summary>
        /// Create a DropShadowLabel
        /// </summary>
        public DropShadowLabel()
        {
            Content = new GUIContent();
            this.Height = 16;
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                EditorGUI.DropShadowLabel(PaintArea, Content, Style);
            }
            else
            {
                EditorGUI.DropShadowLabel(PaintArea, Content);
            }
        }        
    }

}
