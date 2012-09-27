using UnityEngine;
using System.Collections;
using System;
using Skill.UI;
using UnityEditor;

namespace Skill.Editor.UI
{
    /// <summary> Make a slider the user can drag to change an integer value between a min and a max. </summary>
    public class IntSlider : EditorControl
    {
        /// <summary>
        /// Optional label in front of the field.
        /// </summary>
        public GUIContent Label { get; private set; }       

        private int _Value;
        /// <summary>
        /// The value the slider shows. This determines the position of the draggable thumb.
        /// </summary>
        public int Value
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
        /// OCcurs when Value of IntSlider changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when Value of IntSlider changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private int _MinValue;
        /// <summary>
        /// Minimum value of the slider.
        /// </summary>
        public int MinValue
        {
            get { return _MinValue; }
            set
            {
                _MinValue = value;
                if (_MinValue > _MaxValue)
                    _MaxValue = _MinValue;
            }
        }

        private int _MaxValue;
        /// <summary>
        /// Maximum value of the slider.
        /// </summary>
        public int MaxValue
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
        /// Create an IntSlider
        /// </summary>
        public IntSlider()
        {
            this.Label = new GUIContent();
            this._MinValue = 0;
            this._MaxValue = 100;
            this._Value = 50;
            this.Height = 16;
        }

        /// <summary>
        /// Render IntSlider
        /// </summary>
        protected override void Render()
        {
            Value = EditorGUI.IntSlider(RenderArea, Label, _Value, _MinValue, _MaxValue);
        }        
    }
}