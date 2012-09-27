using UnityEngine;
using System.Collections;
using System;
using Skill.UI;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary> A slider the user can drag to change a value between a min and a max. </summary>
    public class Slider : EditorControl
    {
        /// <summary>
        /// Optional label in front of the slider.
        /// </summary>
        public GUIContent Label { get; private set; }       

        private float _Value;
        /// <summary>
        /// The value the slider shows. This determines the position of the draggable thumb.
        /// </summary>
        public float Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when value of Slider changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when value of Slider changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

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
        /// Maximum value of the slider.
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
        /// Create a Slider
        /// </summary>
        public Slider()
        {
            this.Label = new GUIContent();
            this._MinValue = 0;
            this._MaxValue = 100;
            this._Value = 50;
            this.Height = 16;
        }

        /// <summary>
        /// Render Slider
        /// </summary>
        protected override void Render()
        {
            Value = EditorGUI.Slider(RenderArea, Label, _Value, _MinValue, _MaxValue);
        }        
    }
}