using UnityEngine;
using System.Collections;

namespace Skill.UI
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
            Rect rect = PaintAreaWithPadding;
            if (rect.xMax < rect.xMin) rect.xMax = rect.xMin;
            if (rect.yMax < rect.yMin) rect.yMax = rect.yMin;            

            foreach (var c in Controls)
            {
                Rect cRect = new Rect();

                cRect.x = rect.x + c.Position.x + c.Margin.Left;
                cRect.y = rect.y + c.Position.y + c.Margin.Top;
                cRect.width = c.LayoutWidth;
                cRect.height = c.LayoutHeight;                

                c.PaintArea = cRect;
            }            
        }        

    }

}