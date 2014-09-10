using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Provides a base class for all Panel elements. Use Panel elements to position and arrange child objects
    /// </summary>
    public abstract class Panel : BaseControl
    {
        // Variables
        private bool _Invalidate; // true when panel needs update layout at next render

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
        public virtual Thickness Padding { get; set; }


        /// <summary>
        /// Gets a BaseControlCollection of child elements of this Panel.
        /// </summary>
        public BaseControlCollection Controls { get; private set; }

        private Rect _DesiredSize;
        /// <summary> The Size needs for all controls </summary>
        public Rect DesiredSize { get { return _DesiredSize; } }

        /// <summary> Use DesiredSize.Width as Width </summary>
        public bool AutoWidth { get; set; }

        /// <summary> Use DesiredSize.Height as Height </summary>
        public bool AutoHeight { get; set; }

        /// <summary>
        /// Gets RenderArea that shrinks by Padding.
        /// </summary>
        protected Rect RenderAreaShrinksByPadding
        {
            get
            {
                Thickness padding = Padding;
                Rect rect = RenderArea;
                rect.x += padding.Left;
                rect.y += padding.Top;
                rect.width -= padding.Horizontal;
                rect.height -= padding.Vertical;
                return rect;
            }
        }

        /// <summary>
        /// Invalidate to force update layout in next render
        /// </summary>
        public virtual void Invalidate() { _Invalidate = true; }


        #region Constructor
        /// <summary>
        /// Create a panel
        /// </summary>
        protected Panel()
        {
            this._Invalidate = true;
            this.Controls = new BaseControlCollection(this);
            this.Controls.LayoutChange += new System.EventHandler(Controls_LayoutChange);
        }

        void Controls_LayoutChange(object sender, System.EventArgs e)
        {
            OnLayoutChanged();
        }

        /// <summary>
        /// When RenderArea changed
        /// </summary>
        protected override void OnRenderAreaChanged()
        {
            this.Invalidate();
            base.OnRenderAreaChanged();
        }
        #endregion

        /// <summary>
        /// When Layout changed
        /// </summary>
        protected override void OnLayoutChanged()
        {
            this.Invalidate();
            base.OnLayoutChanged();

        }

        internal void UpdateLayoutRecursive()
        {
            if (_Invalidate)
            {
                UpdateLayout();
                foreach (var c in Controls)
                {
                    if (c.ControlType == UI.ControlType.Panel)
                        ((Panel)c).UpdateLayoutRecursive();
                }

                CalcDesiredSize();

                if (AutoWidth) Width = _DesiredSize.width;
                if (AutoHeight) Height = _DesiredSize.height;

                _Invalidate = false;
            }
        }

        ///// <summary>
        ///// Prepare for render
        ///// </summary>
        //protected override void BeginRender()
        //{
        //    base.BeginRender();
        //    if (_Invalidate)
        //    {
        //        UpdateLayout();
        //        CalcDesiredSize();
        //        _Invalidate = false;                
        //    }
        //}

        private void CalcDesiredSize()
        {
            Rect ra = RenderArea;
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            foreach (var c in Controls)
            {
                Rect cRenderArea = c.RenderArea;
                // covert to local
                Rect cLocalRenderArea = new Rect(cRenderArea.xMin - ra.xMin, cRenderArea.yMin - ra.yMin, cRenderArea.width, cRenderArea.height);

                min.x = Mathf.Min(cLocalRenderArea.xMin, min.x);
                min.y = Mathf.Min(cLocalRenderArea.yMin, min.y);

                max.x = Mathf.Max(cLocalRenderArea.xMax, max.x);
                max.y = Mathf.Max(cLocalRenderArea.yMax, max.y);

                //if (c.ControlType == UI.ControlType.Panel)
                //{
                //    Rect ds = ((Panel)c)._DesiredSize;
                //    min.x = Mathf.Min(min.x, ds.xMin);
                //    min.y = Mathf.Min(min.y, ds.yMin);
                //    max.x = Mathf.Max(max.x, ds.xMax);
                //    max.y = Mathf.Max(max.y, ds.yMax);
                //}

            }
            _DesiredSize = new Rect(min.x + ra.xMin, min.y + ra.yMin, max.x - min.x, max.y - min.y);
        }

        /// <summary>
        /// Render child controls
        /// </summary>
        protected override void Render()
        {
            foreach (var c in Controls)
            {
                c.OnGUI();
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
        protected abstract void UpdateLayout();


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
                        Control result = ((Panel)c).FindControlByName(controlName);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Is control in hierarchy of this control
        /// </summary>
        /// <param name="control">control to check</param>
        /// <returns>true if is in hierarchy, otherwise false</returns>
        public override bool IsInHierarchy(Skill.Framework.UI.BaseControl control)
        {
            if (base.IsInHierarchy(control)) return true;
            foreach (var c in Controls)
            {
                if (c != null)
                {
                    bool result = c.IsInHierarchy(control);
                    if (result) return result;
                }
            }
            return false;
        }


        /// <summary>
        /// Returns first control that given point is inside
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>found BaseControl </returns>
        public override BaseControl GetControlAtPoint(Vector2 point)
        {
            if (Contains(point))
            {
                foreach (var c in Controls)
                {
                    if (c != null)
                    {
                        BaseControl result = c.GetControlAtPoint(point);
                        if (result != null) return result;
                    }
                }
                return this;
            }
            return null;
        }

        /// <summary>
        /// Find control in hierarchy with specified tab index
        /// </summary>
        /// <param name="tabIndex">Tab index of control to search</param>
        /// <returns>FocusableControl with specified tab index</returns>
        internal IFocusable FindControlByTabIndex(uint tabIndex)
        {
            foreach (var control in Controls)
            {
                if (control != null && control.IsEnabled)
                {
                    if (control.IsFocusable)
                    {
                        IFocusable focusableControl = (IFocusable)control;
                        if (focusableControl.TabIndex == tabIndex)
                            return focusableControl;
                    }
                    else if (control.ControlType == ControlType.Panel)
                    {
                        IFocusable result = ((Panel)control).FindControlByTabIndex(tabIndex);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Find control in hierarchy with maximum tab index
        /// </summary>        
        /// <returns>FocusableControl with specified tab index</returns>
        internal IFocusable FindControlByMaxTabIndex()
        {
            int ti = int.MinValue;
            return FindControlByMaxTabIndexBefore(ti, int.MaxValue);
        }

        /// <summary>
        /// Find control in hierarchy with maximum tab index before specified index
        /// </summary>
        /// <param name="before">Tab index of control to search</param>
        /// <returns>FocusableControl with specified tab index</returns>
        internal IFocusable FindControlByMaxTabIndexBefore(int before)
        {
            int ti = int.MinValue;
            return FindControlByMaxTabIndexBefore(ti, before);
        }

        private IFocusable FindControlByMaxTabIndexBefore(int tabIndex, int before)
        {
            IFocusable result = null;
            foreach (var control in Controls)
            {
                if (control != null && control.IsEnabled)
                {
                    if (control.IsFocusable)
                    {
                        IFocusable focusableControl = (IFocusable)control;
                        if (focusableControl.TabIndex < before && focusableControl.TabIndex > tabIndex)
                        {
                            result = focusableControl;
                            tabIndex = focusableControl.TabIndex;
                        }
                    }
                    else if (control.ControlType == ControlType.Panel)
                    {
                        IFocusable focusableControl = ((Panel)control).FindControlByMaxTabIndexBefore(tabIndex, before);
                        if (focusableControl != null)
                        {
                            if (result == null || (result.TabIndex < focusableControl.TabIndex && focusableControl.TabIndex < before && focusableControl.TabIndex > tabIndex))
                                result = focusableControl;
                            tabIndex = result.TabIndex;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Find control in hierarchy with minimum tab index
        /// </summary>        
        /// <returns>FocusableControl with specified tab index</returns>
        internal IFocusable FindControlByMinTabIndex()
        {
            int ti = int.MaxValue;
            return FindControlByMinTabIndexAfter(ti, int.MinValue);
        }

        /// <summary>
        /// Find control in hierarchy with minimum tab index but greater than specified index
        /// </summary>
        /// <param name="tabIndex">Tab index of control to search</param>
        /// <returns>FocusableControl with specified tab index</returns>
        internal IFocusable FindControlByMinTabIndexAfter(int tabIndex)
        {
            int ti = int.MaxValue;
            return FindControlByMinTabIndexAfter(ti, tabIndex);
        }

        private IFocusable FindControlByMinTabIndexAfter(int tabIndex, int after)
        {
            IFocusable result = null;
            foreach (var control in Controls)
            {
                if (control != null && control.IsEnabled)
                {
                    if (control.IsFocusable)
                    {
                        IFocusable focusableControl = (IFocusable)control;
                        if (focusableControl.TabIndex > after && focusableControl.TabIndex < tabIndex)
                        {
                            result = focusableControl;
                            tabIndex = focusableControl.TabIndex;
                        }
                    }
                    else if (control.ControlType == ControlType.Panel)
                    {
                        IFocusable focusableControl = ((Panel)control).FindControlByMinTabIndexAfter(tabIndex, after);
                        if (focusableControl != null)
                        {
                            if (result == null || (result.TabIndex >= focusableControl.TabIndex && focusableControl.TabIndex > after && focusableControl.TabIndex < result.TabIndex))
                                result = focusableControl;

                            tabIndex = result.TabIndex;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// calculate RenderArea of Control based on given available Rect
        /// </summary>
        /// <param name="c">Control to calc it's RenderArea</param>
        /// <param name="cellRect">Available space</param>
        protected void SetControlRenderArea(BaseControl c, Rect cellRect)
        {
            Rect renderArea = cellRect;

            // check for VerticalAlignment
            renderArea.height = Mathf.Min(c.LayoutHeight, cellRect.height);
            switch (c.VerticalAlignment)
            {
                case VerticalAlignment.Top: // only Margin.Top is important
                    renderArea.y = Mathf.Min(cellRect.y + c.Margin.Top, cellRect.yMax - renderArea.height);
                    break;
                case VerticalAlignment.Center: // none of Margin.Top and Margin.Bottom is important
                    renderArea.y = cellRect.y + (cellRect.height - renderArea.height) / 2;
                    break;
                case VerticalAlignment.Bottom: // only Margin.Bottom is important
                    renderArea.y = Mathf.Max(cellRect.y, cellRect.yMax - renderArea.height - c.Margin.Bottom);
                    break;
                case VerticalAlignment.Stretch: // both Margin.Top and Margin.Bottom is important
                    renderArea.height = cellRect.height - c.Margin.Vertical;
                    renderArea.y += c.Margin.Top;
                    break;
            }

            if (renderArea.yMax > cellRect.yMax)
                renderArea.y = cellRect.yMax - renderArea.height;



            // check for HorizontalAlignment
            renderArea.width = Mathf.Min(c.LayoutWidth, cellRect.width);
            switch (c.HorizontalAlignment)
            {
                case HorizontalAlignment.Left: // only Margin.Left is important
                    renderArea.x = Mathf.Min(cellRect.x + c.Margin.Left, cellRect.xMax - renderArea.width);
                    break;
                case HorizontalAlignment.Center: // none of Margin.Left and Margin.Right is important
                    renderArea.x = cellRect.x + (cellRect.width - renderArea.width) / 2;
                    break;
                case HorizontalAlignment.Right: // only Margin.Right is important
                    renderArea.x = Mathf.Max(cellRect.x, cellRect.xMax - renderArea.width - c.Margin.Right);
                    break;
                case HorizontalAlignment.Stretch: // both Margin.Left and Margin.Right is important
                    renderArea.width = cellRect.width - c.Margin.Horizontal;
                    renderArea.x += c.Margin.Left;
                    break;
            }

            if (renderArea.xMax > cellRect.xMax)
                renderArea.x = cellRect.xMax - renderArea.width;


            c.RenderArea = renderArea;
        }


        /// <summary>
        /// Handle specified command.
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>   
        /// <remarks>
        /// let controls that contains mouse position to handle input        
        /// as soon as first control handled the command ignore next steps
        /// </remarks>        
        public override bool HandleCommand(UICommand command)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var c = Controls[i];
                Rect cRA = c.RenderArea;
                if (cRA.Contains(command.MousePosition))
                {
                    if (c.HandleCommand(command))
                        return true;
                }
            }
            return base.HandleCommand(command);
        }

        /// <summary>
        /// HandleEvent event
        /// </summary>
        /// <param name="e">event to handle</param>
        public override void HandleEvent(Event e)
        {
            if (IsInScrollView && !IsHandlingEventInternal) return;
            if (e != null && e.type != EventType.Used)
            {
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    var c = Controls[i];
                    if (c != null)
                    {
                        if (OwnerFrame != null && OwnerFrame.IsPrecedenceEvent(c))
                            continue;
                        if (c.Visibility == UI.Visibility.Visible)
                            c.HandleEvent(e);
                        if (e.type == EventType.Used)
                            break;
                    }
                }
                BaseHandleEvent(e);
            }

        }
    }
}