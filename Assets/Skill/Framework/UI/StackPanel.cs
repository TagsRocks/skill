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


        private float _LayoutHeight;
        private bool _UpdateLayoutHeight;

        /// <summary>
        /// Retrieves Height used in layout. It is dependents on visibility and state of children
        /// </summary>
        public override float LayoutHeight
        {
            get
            {
                if (Visibility == UI.Visibility.Collapsed)
                {
                    return 0;
                }
                else
                {
                    if (_UpdateLayoutHeight)
                    {
                        if (Orientation == UI.Orientation.Vertical)
                        {
                            float height = 0;
                            foreach (var c in Controls)
                            {
                                if (c != null && c.Visibility != UI.Visibility.Collapsed)
                                    height += c.LayoutHeight + c.Margin.Vertical;
                            }

                            _LayoutHeight = Mathf.Max(height, this.Height);
                        }
                        else
                        {
                            _LayoutHeight = this.Height;
                        }
                        _UpdateLayoutHeight = false;
                    }
                    return _LayoutHeight;
                }
            }
        }


        private float _LayoutWidth;
        private bool _UpdateLayoutWidth;
        /// <summary>
        /// Retrieves Width used in layout. It is dependents on visibility and state of children
        /// </summary>
        public override float LayoutWidth
        {
            get
            {
                if (Visibility == UI.Visibility.Collapsed)
                {
                    return 0;
                }
                else
                {
                    if (_UpdateLayoutWidth)
                    {
                        if (Orientation == UI.Orientation.Horizontal)
                        {
                            float width = 0;
                            foreach (var c in Controls)
                            {
                                if (c != null && c.Visibility != UI.Visibility.Collapsed)
                                    width += c.LayoutWidth + c.Margin.Horizontal;
                            }

                            _LayoutWidth = Mathf.Max(width, this.Width);
                        }
                        else
                        {
                            _LayoutWidth = this.Width;
                        }
                        _UpdateLayoutWidth = false;
                    }
                    return _LayoutWidth;
                }
            }
        }

        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        protected override void UpdateLayout()
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

            _UpdateLayoutHeight = true;
            _UpdateLayoutWidth = true;

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
                c.ScaleFactor = this.ScaleFactor;
                Rect cRect = renderArea;
                float xMax = renderArea.xMax;
                cRect.x += c.Margin.Left * this.ScaleFactor;
                cRect.width = c.LayoutWidth * this.ScaleFactor;
                renderArea.xMin = cRect.xMax + c.Margin.Right * this.ScaleFactor;
                renderArea.xMax = Mathf.Max(xMax, renderArea.xMin);
                cRect.height = c.LayoutHeight * this.ScaleFactor;
                switch (c.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        cRect.y += c.Margin.Top * this.ScaleFactor;
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
                c.ScaleFactor = this.ScaleFactor;
                Rect cRect = renderArea;
                float yMax = renderArea.yMax;
                cRect.y += c.Margin.Top * this.ScaleFactor;
                cRect.height = c.LayoutHeight * this.ScaleFactor;
                renderArea.yMin = cRect.yMax + c.Margin.Bottom * this.ScaleFactor;
                renderArea.yMax = Mathf.Max(yMax, renderArea.yMin);
                cRect.width = c.LayoutWidth * this.ScaleFactor;

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


        /// <summary>
        /// When Layout changed
        /// </summary>
        protected override void OnLayoutChanged()
        {
            _UpdateLayoutHeight = true;
            _UpdateLayoutWidth = true;
            base.OnLayoutChanged();
        }


    }

}