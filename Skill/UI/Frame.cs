using System;
using System.Collections.Generic;
using UnityEngine;


namespace Skill.UI
{
    /// <summary>
    /// Frame is a content control that supports navigation.
    /// </summary>
    public class Frame
    {
        #region Properties

        private Rect _Position;
        /// <summary>
        /// Position of control relative to parent
        /// </summary>
        public Rect Position
        {
            get { return _Position; }
            set
            {
                if (_Position != value)
                {
                    _Position = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets Position.X
        /// </summary>
        public float X
        {
            get
            {
                return _Position.x;
            }
            set
            {
                if (_Position.x != value)
                {
                    _Position.x = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets Position.Y
        /// </summary>
        public float Y
        {
            get
            {
                return _Position.y;
            }
            set
            {
                if (_Position.y != value)
                {
                    _Position.y = value;
                    OnPositionChange();
                }
            }
        }


        /// <summary>
        /// Gets or sets the suggested width of the element
        /// </summary>
        public float Width
        {
            get
            {
                return _Position.width;
            }
            set
            {
                if (_Position.width != value)
                {
                    _Position.width = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the suggested height of the element.
        /// </summary>
        public float Height
        {
            get
            {
                return _Position.height;
            }
            set
            {
                if (_Position.height != value)
                {
                    _Position.height = value;
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Name of Frame. (optional)
        /// </summary>
        public string Name { get; set; }

        private Visibility _Visibility;
        /// <summary>
        /// Gets or sets the user interface (UI) visibility of this element.
        /// </summary>
        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    OnVisibilityChanged();
                }
            }
        }

        /// <summary>
        /// Grid of Frame.
        /// </summary>
        public Grid Grid { get; private set; }

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
        #endregion

        #region Events
        /// <summary> Occurs when position of control changed </summary>
        public event EventHandler PositionChange;
        /// <summary>
        /// when position of control changed
        /// </summary>
        protected virtual void OnPositionChange()
        {
            if (PositionChange != null)
                PositionChange(this, EventArgs.Empty);
        }
        /// <summary> Occurs when Visibility of control changed </summary>
        public event EventHandler VisibilityChanged;
        /// <summary>
        /// when Visibility of control changed
        /// </summary>
        protected virtual void OnVisibilityChanged()
        {
            if (VisibilityChanged != null)
                VisibilityChanged(this, EventArgs.Empty);
        }
        #endregion

        #region OnGUI

        /// <summary>
        /// to render control you have to call this method in OnGUI method of MonoBehavior.(call this for Frame class)
        /// </summary>
        public virtual void OnGUI()
        {
            if (Visibility == UI.Visibility.Visible)
            {
                Rect rect = Position;

                float x = rect.x;
                float y = rect.y;

                Thickness margin = Grid.Margin;

                rect.x = margin.Left;
                rect.y = margin.Top;
                rect.width -= margin.Horizontal;
                rect.height -= margin.Vertical;
                Grid.Position = rect;

                rect.x += x;
                rect.y += y;
                Grid.RenderArea = rect;

                DrawControls();

                string focusedControlName = GUI.GetNameOfFocusedControl();
                if (!string.IsNullOrEmpty(focusedControlName))
                {
                    Control c = Grid.FindControlByName(focusedControlName);
                    if (c != null && c.Focusable)
                        FocusedControl = (FocusableControl)c;
                    else
                        FocusedControl = null;
                }
            }
        }

        /// <summary>
        /// Call Grid.OnGUI() to draw controls
        /// </summary>
        protected virtual void DrawControls()
        {
            Grid.OnGUI();
        }

        #endregion

        #region Construcor
        /// <summary>
        /// Initializes a new instance of the Frame class.
        /// </summary>
        public Frame()
        {
            Grid = new Grid();
            this.Width = 200;
            this.Height = 200;
        }
        #endregion
    }
}