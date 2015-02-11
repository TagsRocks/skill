using UnityEngine;
using System.Collections;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Track view area of TimeLine
    /// </summary>
    public class TrackBarView : TimeLineView
    {

        public bool SideView { get; set; }

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

            float yMax = _ViewRect.y;
            foreach (TrackBar tb in Controls)
            {
                Rect cRect = _ViewRect;
                if (SideView)
                {
                    if (tb.TreeViewItem != null && tb.TreeViewItem.IsVisible)
                    {
                        Rect itemRa = tb.TreeViewItem.RenderArea;
                        cRect.y = itemRa.y + tb.TreeViewItem.Margin.Top + 1;
                        cRect.height = itemRa.height - tb.TreeViewItem.Margin.Vertical - 2;
                        yMax = Mathf.Max(yMax, itemRa.yMax);
                        tb.Visibility = Framework.UI.Visibility.Visible;
                    }
                    else
                        tb.Visibility = Framework.UI.Visibility.Collapsed;
                }
                else
                {
                    cRect.y = yMax;
                    cRect.height = tb.LayoutHeight - tb.Margin.Vertical;
                    yMax += tb.LayoutHeight;
                }
                tb.RenderArea = cRect;
                tb.Invalidate();
            }

            _ViewRect.yMax = yMax;
            _ViewRect.height = Mathf.Max(ra.height - ScrollbarThickness + 1, _ViewRect.height);

        }

        protected override void GetTimeBounds(out double minTime, out double maxTime)
        {
            minTime = 0.0;
            maxTime = 1.0;

            bool found = false;

            foreach (var c in Controls)
            {
                if (c is TrackBar)
                {
                    if (!found)
                    {
                        found = true;
                        minTime = float.MaxValue;
                        maxTime = float.MinValue;
                    }

                    double dMinTime, dMaxTime;
                    ((TrackBar)c).GetTimeBounds(out dMinTime, out dMaxTime);

                    if (dMaxTime > maxTime) maxTime = dMaxTime;
                    if (dMinTime < minTime) minTime = dMinTime;
                }
            }
        }
    }

}