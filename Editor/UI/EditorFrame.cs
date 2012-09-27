using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{


    /// <summary>
    /// Specifies the position of EditorFrame inside EditorWindow
    /// </summary>
    public enum EditorFrameLocation
    {
        /// <summary>
        /// The location of a EditorFrame fill entire size of the EditorWindow that owns it.
        /// </summary>
        Fill,
        /// <summary>
        /// The location of a EditorFrame is the center of the EditorWindow that owns it.
        /// </summary>
        CenterOwner,
        /// <summary>
        /// The location of a EditorFrame is set from code
        /// </summary>
        Manual,
    }

    /// <summary>
    /// EditorFrame is a content control that supports navigation.
    /// </summary>
    public class EditorFrame : Frame
    {
        /// <summary>
        /// Specifies the position of EditorFrame inside the EditorWindow that owns it.
        /// </summary>
        public EditorFrameLocation Location { get; set; }

        /// <summary>
        /// The EditorWindow that owns EditorFrame.
        /// </summary>
        public EditorWindow Owner { get; private set; }

        /// <summary>
        /// Create an EditorFrame
        /// </summary>
        /// <param name="owner"> The EditorWindow that owns EditorFrame. </param>
        public EditorFrame(EditorWindow owner)
        {
            if (owner == null)
                throw new ArgumentNullException("Invalid EditorWindow");
            this.Owner = owner;
        }

        /// <summary>
        /// to render control you have to call this method in OnGUI method of MonoBehavior.(call this for Frame class)
        /// </summary>
        public override void OnGUI()
        {
            Rect position = Owner.position;
            switch (Location)
            {
                case EditorFrameLocation.Fill:
                    position.x = 0;
                    position.y = 0;
                    break;
                case EditorFrameLocation.CenterOwner:
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
