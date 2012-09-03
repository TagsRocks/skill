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

        protected override void Paint()
        {
            if (_NeedUpdateLayout)
            {
                UpdateLayout(); 
                _NeedUpdateLayout = false;
            }
            PaintControls();
        }

        protected virtual void PaintControls()
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
    }

}