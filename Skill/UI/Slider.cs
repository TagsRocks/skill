using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
{   
    /// <summary> A slider the user can drag to change a value between a min and a max. </summary>
    public class Slider : Control
    {
        /// <summary>
        /// The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.
        /// </summary>
        public GUIStyle ThumbStyle { get; set; }

        /// <summary>
        /// Orientation of slider
        /// </summary>
        public Orientation Orientation { get; set; }

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
        /// Occurs when value of slider changed
        /// </summary>
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private float _MinValue;
        /// <summary>
        /// Minimum valud of slider
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
        /// Maximum valud of slider
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
        /// Create an instance of Slider
        /// </summary>
        public Slider()
        {
            this._MinValue = 0;
            this._MaxValue = 100;
            this._Value = 50;
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (Style != null && ThumbStyle != null)
            {
                if (Orientation == Orientation.Horizontal)
                    Value = GUI.HorizontalSlider(PaintArea, _Value, _MinValue, _MaxValue, Style, ThumbStyle);
                else
                    Value = GUI.VerticalSlider(PaintArea, _Value, _MinValue, _MaxValue, Style, ThumbStyle);
            }
            else
            {
                if (Orientation == Orientation.Horizontal)
                    Value = GUI.HorizontalSlider(PaintArea, _Value, _MinValue, _MaxValue);
                else
                    Value = GUI.VerticalSlider(PaintArea, _Value, _MinValue, _MaxValue);
            }
        }        
    }
}