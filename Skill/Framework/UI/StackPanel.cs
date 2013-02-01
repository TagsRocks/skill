using UnityEngine;
using System.Collections;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Arranges child elements into a single line that can be oriented horizontally or vertically.    
    /// </summary>
    public class StackPanel : Panel
    {
        /// <summary>
        /// Gets or sets a value that indicates the dimension by which child elements are stacked.
        /// </summary>       
        public virtual Orientation Orientation { get; set; }        

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
            Rect renderArea = base.RenderAreaShrinksByPadding;

            foreach (var c in Controls)
            {
                Rect cRect = renderArea;
                float xMax = renderArea.xMax;
                cRect.x += c.Margin.Left;
                cRect.width = c.LayoutWidth;
                renderArea.xMin = cRect.xMax + c.Margin.Right;
                renderArea.xMax = Mathf.Max(xMax, renderArea.xMin);
                cRect.height = c.LayoutHeight;
                switch (c.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        cRect.y += c.Margin.Top;
                        break;
                    case VerticalAlignment.Center:
                        cRect.y = Mathf.Max(renderArea.y, renderArea.y + (renderArea.height - cRect.height) / 2);
                        if (renderArea.y + c.Margin.Top > cRect.y)
                            cRect.y = Mathf.Min(renderArea.y + c.Margin.Top, renderArea.yMax - cRect.height);
                        else if (cRect.yMax > renderArea.yMax - c.Margin.Bottom)
                            cRect.y = Mathf.Max(renderArea.yMax - cRect.height - c.Margin.Bottom, Mathf.Min(cRect.y, renderArea.yMin + c.Margin.Top));
                        break;
                    case VerticalAlignment.Bottom:
                        cRect.y = renderArea.yMax - cRect.height - c.Margin.Bottom;
                        break;
                    case VerticalAlignment.Stretch:
                        cRect.height = renderArea.height - c.Margin.Vertical;
                        cRect.y += c.Margin.Top;
                        if (cRect.yMax > renderArea.yMax)
                            cRect.y = renderArea.yMax - cRect.height;
                        break;
                }
                if (cRect.yMax > renderArea.yMax)
                    cRect.y = renderArea.yMax - cRect.height;
                c.RenderArea = cRect;

            }
        }

        private void UpdateLayoutVertical()
        {
            Rect renderArea = base.RenderAreaShrinksByPadding;

            foreach (var c in Controls)
            {
                Rect cRect = renderArea;
                float yMax = renderArea.yMax;
                cRect.y += c.Margin.Top;
                cRect.height = c.LayoutHeight;
                renderArea.yMin = cRect.yMax + c.Margin.Bottom;
                renderArea.yMax = Mathf.Max(yMax, renderArea.yMin);
                cRect.width = c.LayoutWidth;

                switch (c.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        cRect.x += c.Margin.Left;
                        break;
                    case HorizontalAlignment.Center:
                        cRect.x = Mathf.Max(renderArea.x, renderArea.x + (renderArea.width - cRect.width) / 2);
                        if (renderArea.x + c.Margin.Left > cRect.x)
                            cRect.x = Mathf.Min(renderArea.x + c.Margin.Left, renderArea.xMax - cRect.width);
                        else if (cRect.xMax > renderArea.xMax - c.Margin.Right)
                            cRect.x = Mathf.Max(renderArea.xMax - cRect.width - c.Margin.Right, Mathf.Min(cRect.x, renderArea.xMin + c.Margin.Left));
                        break;
                    case HorizontalAlignment.Right:
                        cRect.x = renderArea.xMax - cRect.width - c.Margin.Right;
                        break;
                    case HorizontalAlignment.Stretch:
                        cRect.width = renderArea.width - c.Margin.Horizontal;
                        cRect.x += c.Margin.Left;
                        if (cRect.xMax > renderArea.xMax)
                            cRect.x = renderArea.xMax - cRect.width;
                        break;
                }
                c.RenderArea = cRect;
            }
        }       
    }

}