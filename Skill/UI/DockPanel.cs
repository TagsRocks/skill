using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.UI
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
            Rect rect = PaintAreaWithPadding;

            int lastIndex = Controls.Count - 1;
            for (int i = 0; i < Controls.Count; i++)
            {
                BaseControl c = Controls[i];
                if (c != null)
                {
                    if (rect.width == 0 || rect.height == 0)
                        c.PaintArea = rect;
                    else
                    {
                        Rect paintArea = rect;
                        if (!(LastChildFill && i == lastIndex))
                        {
                            switch (c.Dock)
                            {
                                case Dock.Left:
                                    float xMax = rect.xMax;
                                    paintArea.width = c.LayoutWidth + c.Margin.Horizontal;
                                    if (paintArea.xMax > xMax)
                                        paintArea.xMax = xMax;
                                    rect.xMin = paintArea.xMax;
                                    rect.xMax = xMax;
                                    break;
                                case Dock.Top:
                                    float yMax = rect.yMax;
                                    paintArea.height = c.LayoutHeight + c.Margin.Vertical;
                                    if (paintArea.yMax > yMax)
                                        paintArea.yMax = yMax;
                                    rect.yMin = paintArea.yMax;
                                    rect.yMax = yMax;
                                    break;
                                case Dock.Right:
                                    float w = c.LayoutWidth + c.Margin.Horizontal;
                                    paintArea.xMin = rect.xMax - w;
                                    paintArea.width = w;
                                    if (paintArea.xMin < rect.xMin)
                                        paintArea.xMin = rect.xMin;
                                    rect.xMax = paintArea.xMin;
                                    break;
                                case Dock.Bottom:
                                    float h = c.LayoutHeight + c.Margin.Vertical;
                                    paintArea.yMin = rect.yMax - h;
                                    paintArea.height = h;
                                    if (paintArea.yMin < rect.yMin)
                                        paintArea.yMin = rect.yMin;
                                    rect.yMax = paintArea.yMin;
                                    break;
                            }
                        }

                        paintArea.x += c.Margin.Left;
                        paintArea.y += c.Margin.Top;
                        paintArea.width -= c.Margin.Horizontal;
                        paintArea.height -= c.Margin.Vertical;

                        c.PaintArea = paintArea;
                    }
                }
            }
        }        
    }
}
