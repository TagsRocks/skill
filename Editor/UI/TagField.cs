using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a tag selection field.
    /// </summary>
    public class TagField : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }      

        /// <summary>
        /// Occurs when tag of TagField changed 
        /// </summary>
        public event EventHandler TagChanged;
        /// <summary>
        /// when tag of TagField changed 
        /// </summary>
        protected virtual void OnTagChanged()
        {
            if (TagChanged != null) TagChanged(this, EventArgs.Empty);
        }

        private string _Tag;
        /// <summary>
        /// The tag selected by the user.
        /// </summary>
        public string Tag
        {
            get { return _Tag; }
            set
            {
                if (_Tag != value)
                {
                    _Tag = value;
                    OnTagChanged();
                }
            }
        }

        /// <summary>
        /// Create an instance of TagField
        /// </summary>
        public TagField()
        {
            Label = new GUIContent();
            this.Height = 16;
        }

        /// <summary>
        /// Render TagField
        /// </summary>
        protected override void Render()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Tag = EditorGUI.TagField(RenderArea, Label, _Tag, Style);
            }
            else
            {
                Tag = EditorGUI.TagField(RenderArea, Label, _Tag);
            }
        }        
    }
}
