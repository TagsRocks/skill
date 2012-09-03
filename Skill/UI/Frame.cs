using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.UI
{
    /// <summary>
    /// Frame is a content control that supports navigation.
    /// </summary>
    public class Frame : Panel
    {
        private FocusableControl _FocusedControl;
        /// <summary>
        /// Get focused FocusableControl in last update or null if no FocusableControl got focus
        /// </summary>
        public FocusableControl FocusedControl
        {
            get { return _FocusedControl; }
            private set
            {
                if (_FocusedControl != value)
                {
                    if (_FocusedControl != null)
                        _FocusedControl.IsFocused = false;
                    _FocusedControl = value;
                    if (_FocusedControl != null)
                        _FocusedControl.IsFocused = true;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the Frame class.
        /// </summary>
        public Frame()
        {
        }


        protected override void PaintControls()
        {
            base.PaintControls();

            string focusedControlName = GUI.GetNameOfFocusedControl();
            if (!string.IsNullOrEmpty(focusedControlName))
            {
                Control c = FindControlByName(focusedControlName);
                if (c != null && c.Focusable)
                    FocusedControl = (FocusableControl)c;
                else
                    FocusedControl = null;
            }
        }


        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            base.PaintArea = new Rect(Position.x, Position.y, Size.Width, Size.Height);
            Rect rect = base.PaintAreaWithPadding;

            if (rect.xMax < rect.xMin) rect.xMax = rect.xMin;
            if (rect.yMax < rect.yMin) rect.yMax = rect.yMin;

            foreach (var c in Controls)
            {
                Rect btnRect = new Rect();

                btnRect.x = rect.x + c.Position.x + c.Margin.Left;
                btnRect.y = rect.y + c.Position.y + c.Margin.Top;
                btnRect.width = c.Size.Width;
                btnRect.height = c.Size.Height;

                c.PaintArea = btnRect;
            }
        }


    }
}