using UnityEngine;
using System.Collections;

namespace Skill.UI
{
    /// <summary>
    /// Arranges child elements into a single line that can be oriented horizontally or vertically.    
    /// </summary>
    public class StackPanel : Panel
    {
        /// <summary>
        /// Gets or sets a value that indicates the dimension by which child elements are stacked.
        /// </summary>       
        public Orientation Orientation { get; set; }        

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    UpdateLayoutHorizontal();
                    break;
                case Orientation.Vertical:
                    UpdateLayoutVertical();
                    break;
            }
        }

        /// <summary>
        /// Create an instance of StackPanel
        /// </summary>
        public StackPanel()
        {
            Orientation = UI.Orientation.Vertical;
        }

        private void UpdateLayoutHorizontal()
        {
            Rect paintArea = base.PaintAreaWithPadding;

            foreach (var c in Controls)
            {
                Rect cRect = paintArea;
                float xMax = paintArea.xMax;
                cRect.x += c.Margin.Left;
                cRect.width = c.LayoutWidth;
                paintArea.xMin = cRect.xMax + c.Margin.Right;
                paintArea.xMax = Mathf.Max(xMax, paintArea.xMin);
                cRect.height = c.LayoutHeight;
                switch (c.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        cRect.y += c.Margin.Top;
                        break;
                    case VerticalAlignment.Center:
                        cRect.y = Mathf.Max(paintArea.y, paintArea.y + (paintArea.height - cRect.height) / 2);
                        if (paintArea.y + c.Margin.Top > cRect.y)
                            cRect.y = Mathf.Min(paintArea.y + c.Margin.Top, paintArea.yMax - cRect.height);
                        else if (cRect.yMax > paintArea.yMax - c.Margin.Bottom)
                            cRect.y = Mathf.Max(paintArea.yMax - cRect.height - c.Margin.Bottom, Mathf.Min(cRect.y, paintArea.yMin + c.Margin.Top));
                        break;
                    case VerticalAlignment.Bottom:
                        cRect.y = paintArea.yMax - cRect.height - c.Margin.Bottom;
                        break;
                    case VerticalAlignment.Stretch:
                        cRect.height = Mathf.Max(paintArea.height - c.Margin.Vertical, c.LayoutHeight);
                        cRect.y += c.Margin.Top;
                        if (cRect.yMax > paintArea.yMax)
                            cRect.y = paintArea.yMax - cRect.height;
                        break;
                }
                if (cRect.yMax > paintArea.yMax)
                    cRect.y = paintArea.yMax - cRect.height;
                c.PaintArea = cRect;

            }
        }

        private void UpdateLayoutVertical()
        {
            Rect paintArea = base.PaintArea;

            foreach (var c in Controls)
            {
                Rect cRect = paintArea;
                float yMax = paintArea.yMax;
                cRect.y += c.Margin.Top;
                cRect.height = c.LayoutHeight;
                paintArea.yMin = cRect.yMax + c.Margin.Bottom;
                paintArea.yMax = Mathf.Max(yMax, paintArea.yMin);
                cRect.width = c.LayoutWidth;

                switch (c.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        cRect.x += c.Margin.Left;
                        break;
                    case HorizontalAlignment.Center:
                        cRect.x = Mathf.Max(paintArea.x, paintArea.x + (paintArea.width - cRect.width) / 2);
                        if (paintArea.x + c.Margin.Left > cRect.x)
                            cRect.x = Mathf.Min(paintArea.x + c.Margin.Left, paintArea.xMax - cRect.width);
                        else if (cRect.xMax > paintArea.xMax - c.Margin.Right)
                            cRect.x = Mathf.Max(paintArea.xMax - cRect.width - c.Margin.Right, Mathf.Min(cRect.x, paintArea.xMin + c.Margin.Left));
                        break;
                    case HorizontalAlignment.Right:
                        cRect.x = paintArea.xMax - cRect.width - c.Margin.Right;
                        break;
                    case HorizontalAlignment.Stretch:
                        cRect.width = Mathf.Max(paintArea.width - c.Margin.Horizontal, c.LayoutWidth);
                        cRect.x += c.Margin.Left;
                        if (cRect.xMax > paintArea.xMax)
                            cRect.x = paintArea.xMax - cRect.width;
                        break;
                }
                c.PaintArea = cRect;
            }
        }       
    }

}