using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// Show child controls in tree view. use FolderView class to arrange controls
    /// </summary>
    public class TreeView : StackPanel, IFocusable
    {

        private BaseControl _SelectedItem;

        /// <summary> Gets or sets the selected item. it must be in hierarchy of TreeView </summary>    
        public BaseControl SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (value != null && _SelectedItem != value)
                {
                    if (value == this || value == this.Background || value == this.FocusedBackground || !IsInHierarchy(value))
                        value = null;
                }

                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                    Focus();
                    OnSelectedItemChanged();
                }
            }
        }

        /// <summary>
        /// Style to use for Box used as background of selected items
        /// </summary>
        public GUIStyle SelectedStyle { get; set; }

        /// <summary>
        /// Occurs when the SelectedItem of TreeView changes.
        /// </summary>
        public event System.EventHandler SelectedItemChanged;
        /// <summary>
        /// when the SelectedItem of TreeView changes.
        /// </summary>
        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null) SelectedItemChanged(this, System.EventArgs.Empty);
            Skill.Editor.UI.EditorFrame.RepaintParentEditorWindow(this);
        }

        /// <summary>
        /// LayoutChanged
        /// </summary>
        protected override void OnLayoutChanged()
        {
            if (_SelectedItem != null)
            {
                if (!IsInHierarchy(_SelectedItem))
                    _SelectedItem = null;
            }
            base.OnLayoutChanged();
        }

        /// <summary>
        /// Create a TreeView
        /// </summary>
        public TreeView()
        {
            this.Orientation = Skill.Framework.UI.Orientation.Vertical;
            this.Background = new Box() { Parent = this, Visibility = Visibility.Visible };
            this.FocusedBackground = new Box() { Parent = this, Visibility = Visibility.Visible };
            this._ScrollbarThickness = 16;
            this.Padding = new Thickness(0);
        }

        protected override void BeginRender()
        {
            _IgnoreScrollbarThickness = true;
            base.BeginRender();
            _IgnoreScrollbarThickness = false;
            Rect ra = RenderAreaShrinksByPadding;
            Rect ds = DesiredSize;
            _ScrollViewRect = ds;
            _ScrollViewRect.width = Mathf.Max(ra.width, ds.width);
            _ScrollViewRect.height = Mathf.Max(ra.height, ds.height);
            _ScrollViewRect.width -= _ScrollbarThickness;

            if (this.Background.Visibility == Visibility.Visible)
            {
                this.Background.RenderArea = RenderArea;
                this.Background.OnGUI();
            }
            if (_IsFocused && this.FocusedBackground.Visibility == Visibility.Visible)
            {
                this.FocusedBackground.RenderArea = RenderArea;
                this.FocusedBackground.OnGUI();
            }

            if (HorizontalScrollbarStyle != null && VerticalScrollbarStyle != null)
            {
                ScrollPosition = GUI.BeginScrollView(ra, _ScrollPosition, _ScrollViewRect, AlwayShowHorizontal, AlwayShowVertical, HorizontalScrollbarStyle, VerticalScrollbarStyle);
            }
            else
            {
                ScrollPosition = GUI.BeginScrollView(ra, _ScrollPosition, _ScrollViewRect, AlwayShowHorizontal, AlwayShowVertical);
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        protected override void Render()
        {
            if (this.SelectedStyle == null)
                this.SelectedStyle = Resources.Styles.SelectedItem;

            Event e = Event.current;
            if (e != null)
            {
                if (e.isMouse && e.type == EventType.MouseDown && e.button == 0)
                {
                    Vector2 mousePos = e.mousePosition;
                    Vector2 localMouse = mousePos - _ScrollPosition;
                    Rect ra = RenderAreaShrinksByPadding;
                    if (ra.Contains(localMouse))
                    {
                        BaseControl item = null;
                        foreach (var c in Controls)
                        {
                            if (c != null)
                            {
                                item = c.GetControlAtPoint(mousePos);
                                if (item != null)
                                    break;
                            }
                        }
                        SelectedItem = item;
                    }
                }
            }


            if (_SelectedItem != null) // render background of selected item
            {
                Rect ra = this.RenderAreaShrinksByPadding;
                Rect cRa = _SelectedItem.RenderArea;
                cRa.xMin = ra.xMin;
                cRa.xMax = ra.xMax;

                cRa.yMin -= _SelectedItem.Margin.Top;
                cRa.height = _SelectedItem.Height + _SelectedItem.Margin.Bottom;

                if (SelectedStyle != null)
                    GUI.Box(cRa, string.Empty, SelectedStyle);
                else
                    GUI.Box(cRa, string.Empty);
            }

            base.Render(); // render rest of controls
        }

        /// <summary> End Render control's content </summary>
        protected override void EndRender()
        {
            GUI.EndScrollView(HandleScrollWheel);
            base.EndRender();
        }


        /// <summary>
        /// Optional parameter to handle ScrollWheel
        /// </summary>
        public bool HandleScrollWheel { get; set; }

        /// <summary>
        /// Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.
        /// </summary>
        public bool AlwayShowHorizontal { get; set; }
        /// <summary>
        /// Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.
        /// </summary>
        public bool AlwayShowVertical { get; set; }

        /// <summary>
        /// Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.
        /// </summary>
        public GUIStyle HorizontalScrollbarStyle { get; set; }
        /// <summary>
        /// Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.
        /// </summary>
        public GUIStyle VerticalScrollbarStyle { get; set; }

        private Vector2 _ScrollPosition;
        /// <summary>
        /// The pixel distance that the view is scrolled in the X and Y directions.
        /// </summary>
        public Vector2 ScrollPosition
        {
            get
            {
                return _ScrollPosition;
            }
            set
            {
                if (_ScrollPosition != value)
                {
                    _ScrollPosition = value;
                    OnScrollPositionChanged();
                }
            }
        }

        public event EventHandler ScrollPositionChanged;
        private void OnScrollPositionChanged()
        {
            if (ScrollPositionChanged != null) ScrollPositionChanged(this, System.EventArgs.Empty);
        }



        private float _ScrollbarThickness = 0;
        /// <summary>
        /// Gets or sets thikness of vertical scrollbar to consider when calculating scrollview area (default is 16)
        /// </summary>
        public float ScrollbarThickness
        {
            get { return _ScrollbarThickness; }
            set
            {
                Thickness padding = Padding;
                _ScrollbarThickness = value;
                Padding = padding;
            }
        }

        private bool _IgnoreScrollbarThickness;
        /// <summary>
        /// Gets or sets the padding inside a control.
        /// </summary>
        /// <returns>
        /// The amount of space between the content of a Panel
        /// and its Margin or Border.
        /// The default is a thickness of 0 on all four sides.
        /// </returns>
        public override Thickness Padding
        {
            get
            {
                if (_IgnoreScrollbarThickness)
                    return base.Padding;
                else
                {
                    Thickness padding = base.Padding;
                    if (Orientation == Orientation.Vertical)
                        padding.Right -= _ScrollbarThickness;
                    else
                        padding.Bottom -= _ScrollbarThickness;
                    return padding;
                }
            }
            set
            {
                if (Orientation == Orientation.Vertical)
                    value.Right += _ScrollbarThickness;
                else
                    value.Bottom += _ScrollbarThickness;
                base.Padding = value;
            }
        }

        private Rect _ScrollViewRect;
        /// <summary>
        /// Gets view rectangle used for inner ScrollView
        /// </summary>
        public Rect ScrollViewRect { get { return _ScrollViewRect; } }

        /// <summary> Tab index of control. </summary>
        public int TabIndex { get; set; }

        private bool _IsFocused;
        /// <summary>
        /// Gets a value that determines whether this element has logical focus. (You must set valid name to enable this behavior)
        /// </summary>
        /// <returns>
        /// true if this element has logical focus; otherwise, false.(You must set valid name to enable this behavior)
        /// </returns>
        /// <remarks>
        /// Set used for internal use
        /// </remarks>
        public bool IsFocused
        {
            get { return _IsFocused; }
            set
            {
                if (_IsFocused != value)
                {
                    _IsFocused = value;
                    if (_IsFocused)
                        OnGotFocus();
                    else
                        OnLostFocus();
                }
            }
        }

        /// <summary> Disable focusable - sometimes in editor it is better to disable focusable </summary>
        public void DisableFocusable() { _IsFocusable = false; }
        /// <summary> Enable focusable </summary>
        public void EnableFocusable() { _IsFocusable = true; }

        private bool _IsFocusable = true;
        /// <summary>
        /// Indicates whether the element can receive focus.(You must set valid name to enable this behavior)
        /// </summary>
        public override bool IsFocusable { get { return _IsFocusable; } }

        /// <summary> it is an extended focusable </summary>
        public bool IsExtendedFocusable { get { return true; } }

        /// <summary>
        /// Occurs when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        public event EventHandler GotFocus;
        /// <summary>
        /// when this element gets logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        protected virtual void OnGotFocus()
        {
            if (GotFocus != null) GotFocus(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when this element loses logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        public event EventHandler LostFocus;
        /// <summary>
        /// when this element loses logical focus.(You must set valid name to enable this behavior)
        /// </summary>
        protected virtual void OnLostFocus()
        {
            if (LostFocus != null) LostFocus(this, EventArgs.Empty);
        }

        /// <summary> Try focuse control </summary>
        public void Focus()
        {
            if (_IsFocusable && OwnerFrame != null)
                OwnerFrame.FocusControl(this);
        }

        /// <summary>
        /// Border and Background.
        /// </summary>
        /// <remarks>
        /// To draw border and background if ListBox set visibility of Background property to true and set valid style
        /// </remarks>
        public Box Background { get; private set; }

        /// <summary>
        /// Border and Background to use when listbox is focused.
        /// </summary>                
        public Box FocusedBackground { get; private set; }



        public void ExpandAll()
        {
            foreach (var item in Controls)
            {
                if (item is FolderView)
                    ((FolderView)item).ExpandAll();
            }
        }

        public void CollapseAll()
        {
            foreach (var item in Controls)
            {
                if (item is FolderView)
                    ((FolderView)item).CollapseAll();
            }
        }
    }
}
