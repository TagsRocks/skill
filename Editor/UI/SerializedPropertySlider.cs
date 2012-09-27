using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEngine;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary> A slider the user can drag to change a value between a min and a max. </summary>
    public class SerializedPropertySlider : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }             

        private float _MinValue;
        /// <summary>
        /// Minimum value of the slider.
        /// </summary>
        public float MinValue
        {
            get { return _MinValue; }
            set
            {
                _MinValue = value;
                if (_MinValue > _MaxValue)
                    _MaxValue = _MinValue;
            }
        }

        private float _MaxValue;
        /// <summary>
        /// Maximum value of the slider.
        /// </summary>
        public float MaxValue
        {
            get { return _MaxValue; }
            set
            {
                _MaxValue = value;
                if (_MinValue > _MaxValue)
                    _MinValue = _MaxValue;
            }
        }

        /// <summary>
        /// Gets of sets Property of Slider
        /// </summary>
        public SerializedProperty Property { get; set; }

        /// <summary>
        /// Create an istance of SerializedPropertySlider
        /// </summary>
        public SerializedPropertySlider()
        {
            this.Label = new GUIContent();
            this._MinValue = 0;
            this._MaxValue = 100;
            this.Property = null; 
            this.Height = 16;
        }

        /// <summary>
        /// Render SerializedPropertySlider
        /// </summary>
        protected override void Render()
        {
            if (Property != null)
                EditorGUI.Slider(RenderArea, Property, _MinValue, _MaxValue, Label);            
        }        
    }
}
