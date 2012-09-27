using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Represents the control that displays a header that has a collapsible window that displays content vertically.
    /// </summary>
    /// <typeparam name="P">Type of Panel that contains controls</typeparam>
    public class VerticalExpander<P> : Control where P : Panel
    {

        private Skill.Editor.UI.Foldout _Foldout;


        /// <summary>
        /// Gets the data used for the header of control.
        /// </summary>
        public Box Header { get; private set; }


        private bool _IsExpanded;
        /// <summary>
        /// Gets or sets whether the Expander content window is visible.
        /// </summary>
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
                    _Foldout.FoldoutState = _IsExpanded;
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
            Panel.Visibility = Skill.UI.Visibility.Visible;
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
            Panel.Visibility = Skill.UI.Visibility.Collapsed;
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
                if (Visibility != Skill.UI.Visibility.Collapsed)
                {
                    if (IsExpanded)
                        return Header.LayoutHeight + Panel.LayoutHeight;
                    else
                        return Header.LayoutHeight;
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
            Rect hpa = RenderArea;
            hpa.height = Header.LayoutHeight;
            Header.RenderArea = hpa;

            _Foldout.RenderArea = new Rect(hpa.x + _Foldout.X, hpa.y + (Header.LayoutHeight - _Foldout.LayoutHeight) / 2, _Foldout.LayoutWidth, _Foldout.LayoutHeight);
            _Foldout.StateChanged += new EventHandler(_Foldout_StateChanged);

            hpa.y += hpa.height;
            hpa.height = RenderArea.height - Header.LayoutHeight;
            Panel.RenderArea = hpa;
        }

        private void _Foldout_StateChanged(object sender, EventArgs e)
        {
            if (_IsExpanded != _Foldout.FoldoutState)
            {
                _IsExpanded = _Foldout.FoldoutState;
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
            if (this.Panel.Parent != null)
                throw new InvalidOperationException("VerticalExpander can not accept the panel that is already child of another Panel");
            if (this.Panel == null) throw new ArgumentNullException("Invalid Panel for VerticalExpander");
            this.Panel.Parent = this;

            this.Header = new Box();
            this._Foldout = new Skill.Editor.UI.Foldout();
            this._Foldout.Position = new Rect(5, 5, 16, 16);
            this.DefaultHeader();
            this.IsExpanded = true;
        }

        /// <summary>
        /// Set default header position
        /// </summary>
        public void DefaultHeader()
        {
            Header.Position = new Rect(0, 0, 300, 20);
            Header.Margin = Thickness.Empty;
        }

        /// <summary>
        /// Render VerticalExpander contents
        /// </summary>
        protected override void Render()
        {
            Header.OnGUI();
            _Foldout.OnGUI();
            if (IsExpanded)
                Panel.OnGUI();
        }
    }
}
