using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a special slider the user can use to specify a range between a min and a max.
    /// </summary>
    public class MinMaxSlider : EditorControl
    {
        /// <summary>
        /// Optional label in front of the slider.
        /// </summary>
        public GUIContent Label { get; private set; }

        /// <summary>
        /// Occurs when MinValue or MaxValue of MinMaxSlider changed
        /// </summary>
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private float _MinValue;
        /// <summary>
        /// The lower value of the range the slider shows;
        /// </summary>
        public float MinValue
        {
            get { return _MinValue; }
            set
            {
                if (_MinValue != value)
                {
                    _MinValue = value;
                    if (_MinValue > _MaxValue)
                        _MaxValue = _MinValue;
                    OnValueChanged();
                }
            }
        }

        private float _MaxValue;
        /// <summary>
        /// The upper value at the range the slider shows.
        /// </summary>
        public float MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (_MaxValue != value)
                {
                    _MaxValue = value;
                    if (_MinValue > _MaxValue)
                        _MinValue = _MaxValue;
                    OnValueChanged();
                }
            }
        }


        private float _MinLimit;
        /// <summary>
        /// The limit at the left end of the slider.
        /// </summary>
        public float MinLimit
        {
            get
            {
                return _MinLimit;
            }
            set
            {
                _MinLimit = value;
                if (_MinLimit > _MaxLimit)
                    _MinLimit = _MaxLimit;
            }
        }


        private float _MaxLimit;
        /// <summary>
        /// The limit at the right end of the slider.
        /// </summary>
        public float MaxLimit
        {
            get
            {
                return _MaxLimit;
            }
            set
            {
                _MaxLimit = value;
                if (_MaxLimit < _MinLimit)
                    _MaxLimit = _MinLimit;
            }
        }

        /// <summary>
        /// Create an instance of MinMaxSlider
        /// </summary>
        public MinMaxSlider()
        {
            this.Label = new GUIContent();
            this._MinValue = 25;
            this._MaxValue = 75;
            this._MinLimit = 0;
            this._MaxLimit = 100;
            this.Height = 16;
        }

        protected override void Render()
        {
            float minV = _MinValue;
            float maxV = _MaxValue;

            EditorGUI.MinMaxSlider(Label, RenderArea, ref minV, ref maxV, _MinLimit, _MaxLimit);
            bool change = minV != _MinValue || maxV != _MaxValue;
            _MinValue = minV;
            _MaxValue = maxV;
            if (change) OnValueChanged();
        }        
    }
}
