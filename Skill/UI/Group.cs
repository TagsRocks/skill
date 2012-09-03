using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.UI
{
    /// <summary>
    /// A group of controls
    /// </summary>
    public class Group : Panel
    {
        /// <summary>
        /// Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out,
        /// no background is rendered, and mouse clicks are passed
        /// </summary>
        public GUIContent Content { get; private set; }
        
        /// <summary>
        /// The style to use for the background.
        /// </summary>
        public GUIStyle Style { get; set; }

        /// <summary>
        /// Create new instance of Group
        /// </summary>
        public Group()
        {
            Content = new GUIContent();
        }

        protected override void PaintControls()
        {
            
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);

            if (Style != null)
            {
                GUI.BeginGroup(PaintArea, Content, Style);
            }
            else
            {
                GUI.BeginGroup(PaintArea, Content);
            }


            base.PaintControls();

            GUI.EndGroup();
        }

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            foreach (var c in Controls)
            {
                Rect btnRect = new Rect();
                btnRect.x = Padding.Left + c.Position.x + c.Margin.Left;
                btnRect.y = Padding.Top + c.Position.y + c.Margin.Top;
                btnRect.width = c.Size.Width;
                btnRect.height = c.Size.Height;

                c.PaintArea = btnRect;
            }
        }
    }


}