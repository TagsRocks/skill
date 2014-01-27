using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary> A slider the user can drag to change a value between a min and a max. </summary>
    public class Slider : FocusableControl
    {
        /// <summary>
        /// The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.
        /// </summary>
        public GUIStyle ThumbStyle { get; set; }

        /// <summary>
        /// Orientation of slider
        /// </summary>
        public Orientation Orientation { get; set; }


        /// <summary>
        /// Unity draw slider at top of RenderArea, to bring it to center set DrawCenter to witdh of slider.
        /// </summary>
        public float DrawCenter { get; set; }

        /// <summary>
        /// Amount of change when user press direction button (only used in HandleCommand method)
        /// </summary>
        public float KeyStep { get; set; }

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
        /// <summary>
        /// Occurs when value of slider changed
        /// </summary>
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
            this.KeyStep = 1.0f;
            this._MinValue = 0;
            this._MaxValue = 100;
            this._Value = 50;
        }

        /// <summary>
        /// Render Slider
        /// </summary>
        protected override void Render()
        {
            if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            Rect ra = RenderArea;
            if (DrawCenter > 0)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    ra.y += (ra.height - DrawCenter) * 0.5f;
                    ra.height = DrawCenter;
                }
                else
                {
                    ra.x += (ra.width - DrawCenter) * 0.5f;
                    ra.width = DrawCenter;
                }
            }
            if (Style != null && ThumbStyle != null)
            {
                if (Orientation == Orientation.Horizontal)
                    Value = GUI.HorizontalSlider(ra, _Value, _MinValue, _MaxValue, Style, ThumbStyle);
                else
                    Value = GUI.VerticalSlider(ra, _Value, _MinValue, _MaxValue, Style, ThumbStyle);
            }
            else
            {
                if (Orientation == Orientation.Horizontal)
                    Value = GUI.HorizontalSlider(ra, _Value, _MinValue, _MaxValue);
                else
                    Value = GUI.VerticalSlider(ra, _Value, _MinValue, _MaxValue);
            }
        }



        /// <summary>
        /// Handle specified command. slider respond to direction commands
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>   
        public override bool HandleCommand(UICommand command)
        {
            if (Orientation == UI.Orientation.Horizontal)
            {
                if (command.Key == KeyCommand.Left)
                {
                    Value -= KeyStep;
                    return true;
                }
                else if (command.Key == KeyCommand.Right)
                {
                    Value += KeyStep;
                    return true;
                }
            }
            else
            {
                if (command.Key == KeyCommand.Up)
                {
                    Value -= KeyStep;
                    return true;
                }
                else if (command.Key == KeyCommand.Down)
                {
                    Value += KeyStep;
                    return true;
                }
            }

            if (command.Key == KeyCommand.Home)
            {
                Value = _MinValue;
                return true;
            }
            else if (command.Key == KeyCommand.End)
            {
                Value = _MaxValue;
                return true;
            }
            return base.HandleCommand(command);
        }
    }
}