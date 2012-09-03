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
    public class TagField : Control
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }      

        /// <summary>
        /// Occurs when tag of TagField changed 
        /// </summary>
        public event EventHandler TagChanged;
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
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null)
            {
                Tag = EditorGUI.TagField(PaintArea, Label, _Tag, Style);
            }
            else
            {
                Tag = EditorGUI.TagField(PaintArea, Label, _Tag);
            }
        }        
    }
}
