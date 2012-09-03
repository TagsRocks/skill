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

        /// <summary> Paint box content </summary>
        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                GUI.Box(PaintArea, Content, Style);
            }
            else
            {
                GUI.Box(PaintArea, Content);
            }
        }        

    }

}