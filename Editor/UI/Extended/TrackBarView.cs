using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// Track view area of TimeLine
    /// </summary>
    public class TrackBarView : TimeLineView
    {        
        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        protected override void UpdateLayout()
        {
            Rect ra = RenderAreaShrinksByPadding;
            double zoomFactor = TimeLine.ZoomFactor;

            _ViewRect = ra;
            _ViewRect.x = (float)(ra.x + TimeLine.MinTime * zoomFactor);
            _ViewRect.width = (float)(ra.width * zoomFactor);

            float y = _ViewRect.y;
            foreach (var c in Controls)
            {
                Rect cRect = _ViewRect;

                cRect.y = y;
                cRect.height = c.LayoutHeight - c.Margin.Vertical;
                y += c.LayoutHeight;

                c.RenderArea = cRect;
                if (c is TrackBar) ((TrackBar)c).Invalidate();
            }

            _ViewRect.yMax = y;
            _ViewRect.height = Mathf.Max(ra.height - ScrollbarThickness + 1, _ViewRect.height);

        }

        protected override double GetFrameAllTime()
        {
            double time = 1.0;
            foreach (var c in Controls)
            {
                if (c is TrackBar)
                {
                    double t = ((TrackBar)c).GetValidTime();
                    if (t > time)
                        time = t;
                }
            }
            return time;
        }
    }

}