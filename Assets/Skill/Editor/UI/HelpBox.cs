using System;
using System.Collections.Generic;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a help box with a message to the user.
    /// </summary>
    public class HelpBox : EditorControl
    {
        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The type of message.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Create a HelpBox
        /// </summary>
        public HelpBox()
        {
            this.Height = 16;
        }

        /// <summary>
        /// Render HelpBox
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            EditorGUI.HelpBox(RenderArea, Message, Type);
        }
    }
}
