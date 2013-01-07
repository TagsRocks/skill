using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Make a text or texture label on screen.
    /// </summary>
    public class Label : Control
    {
        /// <summary>
        /// Text, image and tooltip for this label.
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary>
        /// Text to display on the label.
        /// </summary>
        public string Text
        {
            get { return Content.text; }
            set { Content.text = value; }
        }

        /// <summary>
        /// Create an instance of Lable
        /// </summary>
        public Label()
        {
            Content = new GUIContent();
        }

        /// <summary>
        /// Render Label
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                GUI.Label(RenderArea, Content, Style);
            }
            else
            {
                GUI.Label(RenderArea, Content);
            }
        }        
    }

}