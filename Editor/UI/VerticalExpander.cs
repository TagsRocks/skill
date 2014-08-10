using System;
using System.Collections.Generic;
using Skill.Framework.UI;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Represents the control that displays a header that has a collapsible window that displays content vertically.
    /// </summary>
    /// <typeparam name="P">Type of Panel that contains controls</typeparam>
    public class VerticalExpander<P> : Control where P : Panel
    {

        /// <summary>
        /// Foldout
        /// </summary>
        public Skill.Editor.UI.Foldout Foldout { get; private set; }

        /// <summary>
        /// Gets and sets header of control.
        /// </summary>
        public string Header { get { return Foldout.Content.text; } set { Foldout.Content.text = value; } }


        private bool _IsExpanded;
        /// <summary>
        /// Gets or sets whether the Expander content window is visible. (see remarks)
        /// </summary>
        /// <remarks>
        /// Since the DesiredSize of panel will be calculated after first Render call, default value of IsExpanded property is false to allow panel calculate it's DesiredSize
        /// then by expanding Expander by user we know correct size of inner panel.
        /// So if you want to set this property to true before first Render call, you have to set Height of innder panel manually for first time.
        /// </remarks>
        public bool IsExpanded
        {
            get
            {
                return _IsExpanded;
            }
            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
                    Foldout.IsOpen = _IsExpanded;
                    if (_IsExpanded)
                        OnExpanded();
                    else
                        OnCollapsed();
                }
            }
        }

        /// <summary>
        /// Occurs when the content window of an VerticalExpander control opens to display both its header and content.
        /// </summary>    
        public event EventHandler Expanded;
        /// <summary>
        /// when the content window of an VerticalExpander control opens to display both its header and content.
        /// </summary>
        protected virtual void OnExpanded()
        {
            Panel.Visibility = Skill.Framework.UI.Visibility.Visible;
            if (Expanded != null)
                Expanded(this, EventArgs.Empty);
            OnLayoutChanged();
        }

        /// <summary>
        /// Occurs when the content window of an VerticalExpander control closes and only the Header is visible.
        /// </summary>
        public event EventHandler Collapsed;
        /// <summary>
        /// when the content window of an VerticalExpander control closes and only the Header is visible.
        /// </summary>
        protected virtual void OnCollapsed()
        {
            Panel.Visibility = Skill.Framework.UI.Visibility.Collapsed;
            if (Collapsed != null)
                Collapsed(this, EventArgs.Empty);
            OnLayoutChanged();
        }

        /// <summary>
        /// Gets or sets Position.height
        /// </summary>
        public override float LayoutHeight
        {
            get
            {
                if (Visibility != Skill.Framework.UI.Visibility.Collapsed)
                {
                    Rect ds = Panel.DesiredSize;
                    if (ds.height > 0)
                        Panel.Height = ds.height;
                    if (IsExpanded)
                        return Foldout.LayoutHeight + Foldout.Margin.Vertical + Panel.LayoutHeight + Panel.Margin.Vertical;
                    else
                        return Foldout.LayoutHeight;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Panel that contain's control
        /// </summary>
        public P Panel { get; private set; }

        /// <summary>
        /// Handle RenderArea is changed
        /// </summary>
        protected override void OnRenderAreaChanged()
        {
            base.OnRenderAreaChanged();
            Rect ra = RenderArea;

            Thickness foldoutMargin = Foldout.Margin;
            Foldout.RenderArea = new Rect(ra.x + foldoutMargin.Left, ra.y + foldoutMargin.Top, ra.width - foldoutMargin.Horizontal, Foldout.Height);

            float headerHeight = foldoutMargin.Vertical + Foldout.Height;

            Thickness panelMargin = Panel.Margin;
            Panel.RenderArea = new Rect(ra.x + panelMargin.Left, ra.y + headerHeight + panelMargin.Top,
                                        ra.width - panelMargin.Horizontal, ra.height - headerHeight - panelMargin.Vertical);
        }

        private void _Foldout_StateChanged(object sender, EventArgs e)
        {
            if (_IsExpanded != Foldout.IsOpen)
            {
                _IsExpanded = Foldout.IsOpen;
                if (_IsExpanded)
                    OnExpanded();
                else
                    OnCollapsed();
            }
        }

        /// <summary>
        /// Create new instance of 
        /// </summary>
        /// <param name="panel"></param>
        public VerticalExpander(P panel)
        {
            this.Panel = panel;
            if (this.Panel == null) throw new ArgumentNullException("Invalid Panel for VerticalExpander");
            if (this.Panel.Parent != null)
                throw new InvalidOperationException("VerticalExpander can not accept the panel that is already child of another Panel");
            this.Panel.Parent = this;
            this.Foldout = new Skill.Editor.UI.Foldout() { ToggleOnLabelClick = true };
            this.Foldout.StateChanged += new EventHandler(_Foldout_StateChanged);
            this.DefaultHeader();
            this.IsExpanded = false;
        }

        /// <summary>
        /// Set default header position
        /// </summary>
        public void DefaultHeader()
        {
            Foldout.Style = null;
            Foldout.Position = new Rect(0, 0, 300, 16);
            Foldout.Margin = new Thickness(5);
        }

        /// <summary>
        /// Render VerticalExpander contents
        /// </summary>
        protected override void Render()
        {
            Foldout.OnGUI();
            if (IsExpanded)
                Panel.OnGUI();
        }
        /// <summary>
        /// Is control in hierarchy of this control
        /// </summary>
        /// <param name="control">control to check</param>
        /// <returns>true if is in hierarchy, otherwise false</returns>
        public override bool IsInHierarchy(Skill.Framework.UI.BaseControl control)
        {
            if (base.IsInHierarchy(control)) return true;
            else return Panel.IsInHierarchy(control);
        }
    }
}
