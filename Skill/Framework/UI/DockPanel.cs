using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{

    /// <summary>
    /// Specifies the Dock position of a child element that  is inside a DockPanel.
    /// </summary>
    public enum Dock
    {
        /// <summary>
        /// A child element that is positioned on the left side of the DockPanel.
        /// </summary>
        Left = 0,
        /// <summary>
        /// A child element that is positioned at the top of the DockPanel.
        /// </summary>
        Top = 1,
        /// <summary>
        /// A child element that is positioned on the right side of the DockPanel.
        /// </summary>
        Right = 2,
        /// <summary>
        /// A child element that is positioned at the bottom of the DockPanel.
        /// </summary>
        Bottom = 3,
    }

    /// <summary>
    /// Defines an area where you can arrange child elements either horizontally or vertically, relative to each other.
    /// </summary>
    public class DockPanel : Panel
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the last child element within
        /// a DockPanel stretches to fill the remaining available space. (The default value is true.)
        /// </summary>
        public bool LastChildFill { get; set; }

        /// <summary>
        /// Initializes a new instance of the DockPanel class.
        /// </summary>
        public DockPanel()
        {
            LastChildFill = true;
        }
        
        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            Rect rect = RenderAreaShrinksByPadding;

            int lastIndex = Controls.Count - 1;
            for (int i = 0; i < Controls.Count; i++)
            {
                BaseControl c = Controls[i];
                if (c != null)
                {
                    if (rect.width == 0 || rect.height == 0)
                        c.RenderArea = rect;
                    else
                    {
                        Rect renderArea = rect;
                        if (!(LastChildFill && i == lastIndex))
                        {
                            switch (c.Dock)
                            {
                                case Dock.Left:
                                    float xMax = rect.xMax;
                                    renderArea.width = c.LayoutWidth + c.Margin.Horizontal;
                                    if (renderArea.xMax > xMax)
                                        renderArea.xMax = xMax;
                                    rect.xMin = renderArea.xMax;
                                    rect.xMax = xMax;
                                    break;
                                case Dock.Top:
                                    float yMax = rect.yMax;
                                    renderArea.height = c.LayoutHeight + c.Margin.Vertical;
                                    if (renderArea.yMax > yMax)
                                        renderArea.yMax = yMax;
                                    rect.yMin = renderArea.yMax;
                                    rect.yMax = yMax;
                                    break;
                                case Dock.Right:
                                    float w = c.LayoutWidth + c.Margin.Horizontal;
                                    renderArea.xMin = rect.xMax - w;
                                    renderArea.width = w;
                                    if (renderArea.xMin < rect.xMin)
                                        renderArea.xMin = rect.xMin;
                                    rect.xMax = renderArea.xMin;
                                    break;
                                case Dock.Bottom:
                                    float h = c.LayoutHeight + c.Margin.Vertical;
                                    renderArea.yMin = rect.yMax - h;
                                    renderArea.height = h;
                                    if (renderArea.yMin < rect.yMin)
                                        renderArea.yMin = rect.yMin;
                                    rect.yMax = renderArea.yMin;
                                    break;
                            }
                        }

                        renderArea.x += c.Margin.Left;
                        renderArea.y += c.Margin.Top;
                        renderArea.width -= c.Margin.Horizontal;
                        renderArea.height -= c.Margin.Vertical;

                        c.RenderArea = renderArea;
                    }
                }
            }
        }        
    }
}
