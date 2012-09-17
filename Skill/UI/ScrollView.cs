using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.UI
{
    /// <summary>
    /// Make a scrolling view inside your GUI.
    /// </summary>
    public class ScrollView : Canvas
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

        /// <summary> Begin Render control's content </summary>
        protected override void BeginRender()
        {
            base.BeginRender();
            _ViewRect = RenderArea;
            Size ds = DesiredSize;
            _ViewRect.width = Mathf.Max(RenderArea.width, ds.Width);
            _ViewRect.height = Mathf.Max(RenderArea.height, ds.Height);

            //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
            if (HorizontalScrollbarStyle != null && VerticalScrollbarStyle != null)
            {
                ScrollPosition = GUI.BeginScrollView(RenderArea, _ScrollPosition, _ViewRect, AlwayShowHorizontal, AlwayShowVertical, HorizontalScrollbarStyle, VerticalScrollbarStyle);
            }
            else
            {
                ScrollPosition = GUI.BeginScrollView(RenderArea, _ScrollPosition, _ViewRect, AlwayShowHorizontal, AlwayShowVertical);
            }
        }
        /// <summary> End Render control's content </summary>
        protected override void EndRender()
        {
            GUI.EndScrollView(HandleScrollWheel);
            base.EndRender();
        }
    }

}