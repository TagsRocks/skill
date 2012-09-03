using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.UI
{
    /// <summary>
    /// Make a scrolling view inside your GUI.
    /// </summary>
    public class ScrollView : Panel
    {        
        /// <summary>
        /// Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.
        /// </summary>
        public GUIStyle HorizontalScrollbarStyle { get; set; }
        /// <summary>
        /// Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.
        /// </summary>
        public GUIStyle VerticalScrollbarStyle { get; set; }

        private Vector2 _ScrollPosition;
        /// <summary>
        /// The pixel distance that the view is scrolled in the X and Y directions.
        /// </summary>
        public Vector2 ScrollPosition
        {
            get
            {
                return _ScrollPosition;
            }
            set
            {
                if (_ScrollPosition != value)
                {
                    _ScrollPosition = value;
                    //if (_ScrollPosition.x < 0) _ScrollPosition.x = 0;
                    //else if (_ScrollPosition.x > _ViewRect.x) _ScrollPosition.x = _ViewRect.x;

                    //if (_ScrollPosition.y < 0) _ScrollPosition.y = 0;
                    //else if (_ScrollPosition.y > _ViewRect.y) _ScrollPosition.y = _ViewRect.y;
                }
            }
        }

        /// <summary>
        /// The pixel distance that the view is scrolled in the X direction.
        /// </summary>
        public float ScrollX
        {
            get { return _ScrollPosition.x; }
            set
            {
                if (_ScrollPosition.x != value)
                {
                    _ScrollPosition.x = value;
                    //if (_ScrollPosition.x < 0) _ScrollPosition.x = 0;
                    //else if (_ScrollPosition.x > _ViewRect.x) _ScrollPosition.x = _ViewRect.x;
                }
            }
        }

        /// <summary>
        /// The pixel distance that the view is scrolled in the Y direction.
        /// </summary>
        public float ScrollY
        {
            get { return _ScrollPosition.y; }
            set
            {
                if (_ScrollPosition.y != value)
                {
                    _ScrollPosition.y = value;
                    //if (_ScrollPosition.y < 0) _ScrollPosition.y = 0;
                    //else if (_ScrollPosition.y > _ViewRect.y) _ScrollPosition.y = _ViewRect.y;
                }
            }
        }

        private Rect _ViewRect;
        /// <summary>
        /// The rectangle used inside the scrollview.
        /// </summary>
        public Rect ViewRect
        {
            get
            {
                return _ViewRect;
            }
        }

        /// <summary>
        /// Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.
        /// </summary>
        public bool AlwayShowHorizontal { get; set; }
        /// <summary>
        /// Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.
        /// </summary>
        public bool AlwayShowVertical { get; set; }

        /// <summary>
        /// Optional parameter to handle ScrollWheel
        /// </summary>
        public bool HandleScrollWheel { get; set; }

        /// <summary>
        /// Create an instance of ScrollView
        /// </summary>
        public ScrollView()
        {

        }

        private void UpdateViewRect()
        {
            if (Controls.Count <= 0)
            {
                _ViewRect = PaintArea;
            }
            else
            {
                Rect view = new Rect();
                view.xMin = view.yMin = 0;
                view.xMax = view.yMax = 0;

                foreach (var c in Controls)
                {
                    Rect cRect = c.PaintArea;
                    view.xMin = Mathf.Min(cRect.xMin, view.xMin);
                    view.yMin = Mathf.Min(cRect.yMin, view.yMin);

                    view.xMax = Mathf.Max(cRect.xMax, view.xMax);
                    view.yMax = Mathf.Max(cRect.yMax, view.yMax);
                }
                _ViewRect = view;
            }
            float deltaX = 0.0f - _ViewRect.xMin;
            float deltaY = 0.0f - _ViewRect.yMin;

            _ViewRect.xMin += deltaX;
            _ViewRect.xMax += deltaX;

            _ViewRect.yMin += deltaY;
            _ViewRect.yMax += deltaY;
        }

        protected override void PaintControls()
        {
            UpdateViewRect();

            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (HorizontalScrollbarStyle != null && VerticalScrollbarStyle != null)
            {
                ScrollPosition = GUI.BeginScrollView(PaintArea, _ScrollPosition, _ViewRect, AlwayShowHorizontal, AlwayShowVertical, HorizontalScrollbarStyle, VerticalScrollbarStyle);
            }
            else
            {
                ScrollPosition = GUI.BeginScrollView(PaintArea, _ScrollPosition, _ViewRect, AlwayShowHorizontal, AlwayShowVertical);
            }

            base.PaintControls();

            GUI.EndScrollView(HandleScrollWheel);
        }

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            foreach (var c in Controls)
            {
                Rect btnRect = new Rect();
                btnRect.x = Padding.Left + c.Position.x + c.Margin.Left;
                btnRect.y = Padding.Top + c.Position.y + c.Margin.Top;
                btnRect.width = c.Size.Width;
                btnRect.height = c.Size.Height;

                c.PaintArea = btnRect;
            }
        }        
    }

}