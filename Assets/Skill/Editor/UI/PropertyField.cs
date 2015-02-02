using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a field for SerializedProperty.
    /// </summary>
    public class PropertyField : EditorControl
    {
        /// <summary>
        /// Optional label to use. If not specified the label of the property itself is used. Use GUIContent.none to not display a label at all.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// The SerializedProperty to make a field for.
        /// </summary>
        public SerializedProperty Property { get; set; }

        /// <summary>
        /// If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).
        /// </summary>
        public bool IncludeChildren { get; set; }



        private bool _Result;

        /// <summary>
        /// True if the property has children and is expanded and includeChildren was set to false; otherwise false.
        /// </summary>
        public bool Result
        {
            get { return _Result; }
            set
            {
                if (_Result != value)
                {
                    _Result = value;
                    OnResultChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when Result of PropertyField changed
        /// </summary>
        public event EventHandler ResultChanged;
        /// <summary>
        /// when Result of PropertyField changed
        /// </summary>
        protected virtual void OnResultChanged()
        {
            if (ResultChanged != null) ResultChanged(this, EventArgs.Empty);
        }


        /// <summary>
        /// Create a PropertyField
        /// </summary>
        public PropertyField()
        {
            this.Label = new GUIContent();
            this.Property = null;
            this.Height = 16;
        }

        /// <summary>
        /// Render PropertyField
        /// </summary>
        protected override void Render()
        {
            if (Property != null)
                Result = EditorGUI.PropertyField(RenderArea, Property, Label, IncludeChildren);
        }
    }
}
