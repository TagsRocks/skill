using UnityEngine;
using System.Collections;

namespace Skill.Framework.UI
{
    /// <summary>    
    /// Defines an area within which you can explicitly position child elements by
    /// using coordinates that are relative to the Canvas area.    
    /// </summary>
    public class Canvas : Panel
    {
        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        public override void UpdateLayout()
        {
            Rect rect = RenderAreaShrinksByPadding;
            if (rect.xMax < rect.xMin) rect.xMax = rect.xMin;
            if (rect.yMax < rect.yMin) rect.yMax = rect.yMin;

            foreach (var c in Controls)
            {
                c.ScaleFactor = this.ScaleFactor;
                Rect cRect = new Rect();

                cRect.x = rect.x + (c.X + c.Margin.Left) * this.ScaleFactor;
                cRect.y = rect.y + (c.Y + c.Margin.Top) * this.ScaleFactor;
                cRect.width = c.LayoutWidth * this.ScaleFactor;
                cRect.height = c.LayoutHeight * this.ScaleFactor;

                c.RenderArea = cRect;
            }
        }

    }

}