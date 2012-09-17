using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
{
    /// <summary>
    /// Make a graphical box.
    /// </summary>
    public class Box : Control
    {
        /// <summary>
        /// Text, image and tooltip for this box.
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary>
        /// Make a graphical box.
        /// </summary>
        public Box()
        {
            Content = new GUIContent();
        }

        /// <summary> Render box content </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                GUI.Box(RenderArea, Content, Style);
            }
            else
            {
                GUI.Box(RenderArea, Content);
            }
        }        

    }

}