using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    public class CustomPropertyField : EditorControl
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

        public CustomPropertyField()
        {
            this.Label = new GUIContent();
            this.Property = null;
            this.Height = 16;
        }

        protected override void Paint()
        {
            throw new NotImplementedException("CustomPropertyField not implemented");
            // should be implemented

            //if (Property != null)
            //    EditorGUI.BeginProperty(PaintArea, Property, Label, IncludeChildren);
        }
    }
}
