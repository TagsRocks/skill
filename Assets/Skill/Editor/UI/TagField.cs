using System;
using System.Collections.Generic;
using Skill.Framework.UI;
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

        private string _SelectedTag;
        /// <summary>
        /// The tag selected by the user.
        /// </summary>
        public string SelectedTag
        {
            get { return _SelectedTag; }
            set
            {
                if (_SelectedTag != value)
                {
                    _SelectedTag = value;
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
                SelectedTag = EditorGUI.TagField(RenderArea, Label, _SelectedTag, Style);
            }
            else
            {
                SelectedTag = EditorGUI.TagField(RenderArea, Label, _SelectedTag);
            }
        }        
    }
}
