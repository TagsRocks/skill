using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.Framework.UI
{
    /// <summary>
    /// A group of controls
    /// </summary>
    public class Group : Canvas
    {
        /// <summary>
        /// Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out,
        /// no background is rendered, and mouse clicks are passed
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary>
        /// The style to use. If null, the style from the current GUISkin is used.
        /// </summary>
        public GUIStyle Style { get; set; }      

        /// <summary>
        /// Create new instance of Group
        /// </summary>        
        public Group()
        {
            Content = new GUIContent();
        }        

        /// <summary> Begin render contents </summary>
        protected override void BeginRender()
        {
            base.BeginRender();
            if (Style != null)
            {
                GUI.BeginGroup(RenderArea, Content, Style);
            }
            else
            {
                GUI.BeginGroup(RenderArea, Content);
            }
        }

        /// <summary> End render contents </summary>
        protected override void EndRender()
        {
            GUI.EndGroup();
            base.EndRender();            
        }
    }


}