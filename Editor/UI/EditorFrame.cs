using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Framework.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    
    /// <summary>
    /// EditorFrame is a content control that supports navigation.
    /// </summary>
    public class EditorFrame : Frame
    {
        /// <summary>
        /// Specifies the position of EditorFrame inside the EditorWindow that owns it.
        /// </summary>
        public FrameLocation Location { get; set; }

        /// <summary>
        /// The EditorWindow that owns EditorFrame.
        /// </summary>
        public EditorWindow Owner { get; private set; }

        /// <summary>
        /// Create an EditorFrame
        /// </summary>        
        /// <param name="name">Valid and unique name of frame</param>
        /// <param name="owner"> The EditorWindow that owns EditorFrame. </param>
        public EditorFrame(string name, EditorWindow owner)
            : base(name)
        {
            if (owner == null)
                throw new ArgumentNullException("Invalid EditorWindow");
            this.Owner = owner;
            this.Location = FrameLocation.Fill;
        }

        /// <summary>
        /// to render control you have to call this method in OnGUI method of MonoBehavior.(call this for Frame class)
        /// </summary>
        public override void OnGUI()
        {
            Rect position = Owner.position;
            switch (Location)
            {
                case FrameLocation.Fill:
                    position.x = 0;
                    position.y = 0;
                    break;
                case FrameLocation.Center:
                    position.x = (position.width - Width) / 2;
                    position.y = (position.height - Height) / 2;
                    position.width = Width;
                    position.height = Height;
                    break;
            }
            this.Position = position;
            base.OnGUI();
        }
    }
}
