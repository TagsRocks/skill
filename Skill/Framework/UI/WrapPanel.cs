﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.Framework.UI
{

    /// <summary>
    /// Positions child elements in sequential position from left to right, breaking
    /// content to the next line at the edge of the containing box. Subsequent ordering
    /// happens sequentially from top to bottom or from right to left, depending
    /// on the value of the WrapPanel.Orientation property.
    /// </summary>
    public class WrapPanel : Panel
    {
        /// <summary>
        /// Gets or sets a value that specifies the dimension in which child content is arranged.
        /// </summary> 
        /// <returns>
        ///  An Orientation value that represents the physical orientation of content within the WrapPanel as horizontal or vertical.
        ///  The default value is Orientation.Horizontal.
        /// </returns>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        protected override void UpdateLayout()
        {
            if (Orientation == UI.Orientation.Horizontal)
                UpdateLayoutHorizontal();
            else
                UpdateLayoutVertical();
        }



        private void UpdateLayoutHorizontal()
        {
            Rect rect = RenderAreaShrinksByPadding;
            float x = rect.x;
            float y = rect.y, yMax = rect.y;

            foreach (var c in Controls)
            {
                c.ScaleFactor = this.ScaleFactor;
                Rect cRect = new Rect(x, y, (c.Margin.Horizontal + c.LayoutWidth) * this.ScaleFactor, (c.Margin.Vertical + c.LayoutHeight) * this.ScaleFactor);

                if (cRect.xMax > rect.xMax)
                {
                    y = yMax;
                    x = rect.x;
                    cRect.x = x;
                    cRect.y = y;
                }

                x = cRect.xMax;
                yMax = Mathf.Max(cRect.yMax, yMax);
                c.RenderArea = new Rect(cRect.x + c.Margin.Left, cRect.y + c.Margin.Top, c.LayoutWidth, c.LayoutHeight);
            }
        }

        private void UpdateLayoutVertical()
        {
            Rect rect = RenderAreaShrinksByPadding;
            float x = rect.x, xMax = rect.x;
            float y = rect.y;

            foreach (var c in Controls)
            {
                Rect cRect = new Rect(x, y, (c.Margin.Horizontal + c.LayoutWidth) * this.ScaleFactor, (c.Margin.Vertical + c.LayoutHeight) * this.ScaleFactor);

                if (cRect.yMax > rect.yMax)
                {
                    x = xMax;
                    y = rect.y;
                    cRect.x = x;
                    cRect.y = y;
                }

                y = cRect.yMax;
                xMax = Mathf.Max(cRect.xMax, xMax);
                c.RenderArea = new Rect(cRect.x + c.Margin.Left, cRect.y + c.Margin.Top, c.LayoutWidth, c.LayoutHeight);
            }
        }


        protected override void Render()
        {
            Rect ra = RenderArea;
            foreach (var c in Controls)
            {
                if (Contains(ra, c.RenderArea))
                    c.OnGUI();
            }
        }

        private bool Contains(Rect rect1, Rect rect2)
        {
            return ((rect2.xMin >= rect1.xMin) && (rect2.yMin >= rect1.yMin) && (rect2.xMax <= rect1.xMax) && (rect2.yMax <= rect1.yMax));
        }
    }
}
