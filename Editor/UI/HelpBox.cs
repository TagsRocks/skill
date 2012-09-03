using System;
using System.Collections.Generic;
using UnityEditor;
using Skill.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a help box with a message to the user.
    /// </summary>
    public class HelpBox : Control
    {
        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The type of message.
        /// </summary>
        public MessageType Type { get; set; }


        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            EditorGUI.HelpBox(PaintArea, Message, Type);
        }
    }
}
