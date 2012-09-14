using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.UI
{

    /// <summary>
    /// Provides a base class for all Panel elements. Use Panel elements to position and arrange child objects
    /// </summary>
    public abstract class Panel : BaseControl
    {
        // Variables
        private bool _NeedUpdateLayout; // true when panel needs update layout at next paint

        /// <summary>  Type of Control : Panel </summary>
        public override ControlType ControlType { get { return ControlType.Panel; } }

        /// <summary>
        /// Gets or sets the padding inside a control.
        /// </summary>
        /// <returns>
        /// The amount of space between the content of a Panel
        /// and its Margin or Border.
        /// The default is a thickness of 0 on all four sides.
        /// </returns>
        public Thickness Padding { get; set; }

        /// <summary>
        /// Gets a BaseControlCollection of child elements of this Panel.
        /// </summary>
        public BaseControlCollection Controls { get; private set; }

        private Size _DesiredSize;
        /// <summary> The Size needs for all controls </summary>
        public Size DesiredSize { get { return _DesiredSize; } }

        /// <summary>
        /// Gets PaintArea that shrinks by Padding.
        /// </summary>
        protected Rect PaintAreaWithPadding
        {
            get
            {
                Rect rect = PaintArea;
                rect.x += Padding.Left;
                rect.y += Padding.Top;
                rect.width -= Padding.Horizontal;
                rect.height -= Padding.Vertical;
                return rect;
            }
        }

        /// <summary>
        /// used by inherited objets to request UpdateLayout
        /// </summary>
        protected void RequestUpdateLayout() { _NeedUpdateLayout = true; }

        #region Constructor
        /// <summary>
        /// Create a panel
        /// </summary>
        protected Panel()
        {
            this._NeedUpdateLayout = true;
            this.Controls = new BaseControlCollection(this);
            this.Controls.LayoutChange += new System.EventHandler(Controls_LayoutChange);
        }


        void Controls_LayoutChange(object sender, System.EventArgs e)
        {
            this._NeedUpdateLayout = true;
        }

        protected override void OnPaintAreaChanged()
        {
            RequestUpdateLayout();
            base.OnPaintAreaChanged();
        }
        #endregion

        protected override void OnLayoutChanged()
        {
            _NeedUpdateLayout = true;
            base.OnLayoutChanged();

        }


        protected override void BeginPaint(PaintParameters paintParams)
        {
            base.BeginPaint(paintParams);
            if (_NeedUpdateLayout)
            {
                UpdateLayout();
                CalcDesiredSize();
                _NeedUpdateLayout = false;
            }
        }

        private void CalcDesiredSize()
        {
            Rect pa = PaintArea;
            Vector2 min = new Vector2(pa.xMin, pa.yMin);
            Vector2 max = new Vector2(pa.xMax, pa.yMax);

            foreach (var c in Controls)
            {
                Rect cPaintArea = c.PaintArea;
                Thickness cMargin = c.Margin;

                min.x = Mathf.Min(cPaintArea.xMin - cMargin.Left, min.x);
                min.y = Mathf.Min(cPaintArea.yMin - cMargin.Top, min.y);

                max.x = Mathf.Max(cPaintArea.xMax + cMargin.Right, max.x);
                max.y = Mathf.Max(cPaintArea.yMax + cMargin.Bottom, max.y);

            }
            _DesiredSize = new Size(max.x - min.x, max.y - min.y);
        }

        protected override void Paint(PaintParameters paintParams)
        {
            foreach (var c in Controls)
            {
                c.OnGUI(paintParams);
            }
        }



        /// <summary>
        /// Ensures that all visual child elements of this element are properly updated for layout.
        /// </summary>
        /// <remarks>
        /// Calling this method has no effect if layout is unchanged, or if neither arrangement nor measurement state of a layout is invalid. 
        /// However, if layout is invalid in either respect, the UpdateLayout call will redo the entire layout.
        /// Therefore, you should avoid calling UpdateLayout after each incremental and minor change in the element tree.
        /// The layout system will perform element layout in a deferred manner, using an algorithm that balances performance and currency, and with a weighting strategy to defer changes to roots until all child elements are valid.
        /// You should only call UpdateLayout if you absolutely need updated sizes and positions, and only after you are certain that all changes to properties that you control and that may affect layout are completed.
        /// </remarks>
        public abstract void UpdateLayout();


        /// <summary>
        /// Find control in hierarchy with specified name
        /// </summary>
        /// <param name="controlName">Name of control to search</param>
        /// <returns></returns>
        public Control FindControlByName(string controlName)
        {
            foreach (var c in Controls)
            {
                if (c != null)
                {
                    if (c.ControlType == ControlType.Control)
                    {
                        Control control = (Control)c;
                        if (control.Name == controlName)
                            return control;
                    }
                    else if (c.ControlType == ControlType.Panel)
                    {
                        Control result = FindControlByName((Panel)c, controlName);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }

        private Control FindControlByName(Panel panel, string controlName)
        {
            if (panel != null)
            {
                foreach (var c in panel.Controls)
                {
                    if (c != null)
                    {
                        if (c.ControlType == ControlType.Control)
                        {
                            Control control = (Control)c;
                            if (control.Name == controlName)
                                return control;
                        }
                        else if (c.ControlType == ControlType.Panel)
                        {
                            Control result = FindControlByName((Panel)c, controlName);
                            if (result != null)
                                return result;
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// calculate PaintArea of Control based on given available Rect
        /// </summary>
        /// <param name="c">Control to calc it's PaintArea</param>
        /// <param name="cellRect">Available space</param>
        protected void SetControlPaintArea(BaseControl c, Rect cellRect)
        {
            Rect paintArea = cellRect;

            // check for VerticalAlignment
            paintArea.height = Mathf.Min(c.LayoutHeight, cellRect.height);
            switch (c.VerticalAlignment)
            {
                case VerticalAlignment.Top: // only Margin.Top is important
                    paintArea.y = Mathf.Min(cellRect.y + c.Margin.Top, cellRect.yMax - paintArea.height);
                    break;
                case VerticalAlignment.Center: // none of Margin.Top and Margin.Bottom is important
                    paintArea.y = cellRect.y + (cellRect.height - paintArea.height) / 2;
                    break;
                case VerticalAlignment.Bottom: // only Margin.Bottom is important
                    paintArea.y = Mathf.Max(cellRect.y, cellRect.yMax - paintArea.height - c.Margin.Bottom);
                    break;
                case VerticalAlignment.Stretch: // both Margin.Top and Margin.Bottom is important
                    paintArea.height = cellRect.height - c.Margin.Vertical;
                    paintArea.y += c.Margin.Top;
                    break;
            }

            if (paintArea.yMax > cellRect.yMax)
                paintArea.y = cellRect.yMax - paintArea.height;



            // check for HorizontalAlignment
            paintArea.width = Mathf.Min(c.LayoutWidth, cellRect.width);
            switch (c.HorizontalAlignment)
            {
                case HorizontalAlignment.Left: // only Margin.Left is important
                    paintArea.x = Mathf.Min(cellRect.x + c.Margin.Left, cellRect.xMax - paintArea.width);
                    break;
                case HorizontalAlignment.Center: // none of Margin.Left and Margin.Right is important
                    paintArea.x = cellRect.x + (cellRect.width - paintArea.width) / 2;
                    break;
                case HorizontalAlignment.Right: // only Margin.Right is important
                    paintArea.x = Mathf.Max(cellRect.x, cellRect.xMax - paintArea.width - c.Margin.Right);
                    break;
                case HorizontalAlignment.Stretch: // both Margin.Left and Margin.Right is important
                    paintArea.width = cellRect.width - c.Margin.Horizontal;
                    paintArea.x += c.Margin.Left;
                    break;
            }

            if (paintArea.xMax > cellRect.xMax)
                paintArea.x = cellRect.xMax - paintArea.width;


            c.PaintArea = paintArea;
        }
    }
}