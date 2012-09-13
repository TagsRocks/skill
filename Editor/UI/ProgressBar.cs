using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Make a progress bar.
    /// </summary>
    public class ProgressBar : EditorControl
    {
        private string _Text;
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                if (_Text == null)
                    _Text = string.Empty;
            }
        }

        /// <summary>
        /// Occurs when value of ProgressBar changed
        /// </summary>
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private float _Value;
        /// <summary>
        /// Value that is shown.
        /// </summary>
        public float Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    if (_Value < 0) _Value = 0;
                    else if (_Value > 1.0f) _Value = 1.0f;
                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Create a ProgressBar
        /// </summary>
        public ProgressBar()
        {
            Text = string.Empty;
            this.Height = 16;
        }

        protected override void Paint()
        {
            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            EditorGUI.ProgressBar(PaintArea, _Value, _Text);
        }

        /// <summary>
        /// Set value between minimum and maximum
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="value">Value between min and max</param>
        public void SetValue(float min, float max, float value)
        {
            this.Value = (value - min) / (max - min);
        }
    }
}
