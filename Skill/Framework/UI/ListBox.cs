﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary> Defines the selection behavior for a ListBox. </summary>
    public enum SelectionMode
    {
        /// <summary> The user can select only one item at a time. </summary>
        Single = 0,
        /// <summary> The user can select multiple items without holding down a modifier key. </summary>
        Multiple = 1,
        /// <summary> The user can select multiple consecutive items while holding down the SHIFT key. </summary>
        Extended = 2,
    }

    //public class SelectionChangedEventArgs
    //{

    //    /// <summary>
    //    /// Initializes a new instance of the SelectionChangedEventArgs class.
    //    /// </summary>
    //    /// <param name="removedItems"> The items that were unselected during this event. </param>
    //    /// <param name="addedItems"> The items that were selected during this event. </param>
    //    public SelectionChangedEventArgs(IList removedItems, IList addedItems)
    //    {
    //        this.RemovedItems = removedItems;
    //        this.AddedItems = addedItems;
    //    }

    //    /// <summary> Gets a list that contains the items that were selected. </summary>
    //    /// <returns> The items that were selected since the last time the SelectionChanged event occurred. </returns>
    //    public IList AddedItems { get; private set; }

    //    /// <summary> Gets a list that contains the items that were unselected. </summary>
    //    /// <returns>The items that were unselected since the last time the SelectionChanged event occurred.</returns>
    //    public IList RemovedItems { get; private set; }
    //}

    //// <summary>
    //// Represents the method that will handle the SelectionChanged event.
    //// </summary>
    //// <param name="sender">The object where the event handler is attached.</param>
    //// <param name="e">The event data.</param>
    //public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    /// <summary>
    /// Contains a list of selectable items.
    /// </summary>
    public class ListBox : StackPanel
    {

        // variables
        private List<BaseControl> _SelectedItems;
        private int _SelectedIndex = -1;


        /// <summary> Gets the currently selected items. </summary>
        /// <returns> Returns a collection of the currently selected items. </returns>
        /// <exception cref="System.InvalidOperationException"> The ListBox.SelectionMode property is set to SelectionMode.Single. </exception>
        public IList<BaseControl> SelectedItems
        {
            get
            {
                if (SelectionMode == SelectionMode.Single)
                    throw new System.InvalidOperationException("The ListBox.SelectionMode property is set to SelectionMode.Single");
                return _SelectedItems;
            }
        }

        /// <summary> Gets or sets the first item in the current selection or returns null if the selection is empty </summary>
        /// <returns>The first item in the current selection or null if the selection is empty.</returns>
        public BaseControl SelectedItem
        {
            get
            {
                if (SelectionMode == SelectionMode.Single && _SelectedItems.Count > 0)
                {
                    return _SelectedItems[0];
                }
                else return null;
            }
            set
            {
                if (SelectionMode == SelectionMode.Single)
                {
                    if (value == null && _SelectedItems.Count > 0)
                    {
                        _SelectedIndex = -1;
                        _SelectedItems.Clear();
                        OnSelectionChanged();
                    }
                    else
                    {
                        int index = Controls.IndexOf(value);
                        if (index >= 0 && index != _SelectedIndex)
                        {
                            _SelectedItems.Clear();
                            _SelectedItems.Add(value);
                            _SelectedIndex = index;
                            OnSelectionChanged();
                        }
                    }
                }
            }
        }

        /// <summary> 
        /// Gets or sets the index of the first item in the current selection or returns
        /// negative one (-1) if the selection is empty.
        /// </summary>
        /// <returns>
        /// The index of first item in the current selection. The default value is negative one (-1).
        /// </returns>
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (SelectionMode == SelectionMode.Single)
                {
                    if (value < 0) value = -1;
                    else if (value > Controls.Count - 1) value = Controls.Count - 1;

                    if (_SelectedIndex != value)
                    {
                        if (value == -1)
                            SelectedItem = null;
                        else
                            SelectedItem = Controls[value];
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selection behavior for a ListBox.
        /// </summary>
        /// <returns>One of the SelectionMode values. The default is SelectionMode.Single selection.</returns>
        public SelectionMode SelectionMode { get; set; }

        /// <summary>
        /// Occurs when the selection of ListBox changes.
        /// </summary>
        public event System.EventHandler SelectionChanged;
        /// <summary>
        /// when the selection of ListBox changes.
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            if (SelectionChanged != null) SelectionChanged(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Style to use for Box used for background of selected items
        /// </summary>
        public GUIStyle SelectedStyle { get; set; }


        /// <summary>
        /// Border and Background.
        /// </summary>
        /// <remarks>
        /// To draw border and background if ListBox set visibility of Background property to true and set valid style
        /// </remarks>
        public Box Background { get; private set; }

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
                    //if (_ScrollPosition.x < 0) _ScrollPosition.x = 0;
                    //else if (_ScrollPosition.x > _ViewRect.x) _ScrollPosition.x = _ViewRect.x;

                    //if (_ScrollPosition.y < 0) _ScrollPosition.y = 0;
                    //else if (_ScrollPosition.y > _ViewRect.y) _ScrollPosition.y = _ViewRect.y;
                }
            }
        }

        /// <summary>
        /// The pixel distance that the view is scrolled in the X direction.
        /// </summary>
        public float ScrollX
        {
            get { return _ScrollPosition.x; }
            set
            {
                if (_ScrollPosition.x != value)
                {
                    _ScrollPosition.x = value;
                    //if (_ScrollPosition.x < 0) _ScrollPosition.x = 0;
                    //else if (_ScrollPosition.x > _ViewRect.x) _ScrollPosition.x = _ViewRect.x;
                }
            }
        }

        /// <summary>
        /// The pixel distance that the view is scrolled in the Y direction.
        /// </summary>
        public float ScrollY
        {
            get { return _ScrollPosition.y; }
            set
            {
                if (_ScrollPosition.y != value)
                {
                    _ScrollPosition.y = value;
                    //if (_ScrollPosition.y < 0) _ScrollPosition.y = 0;
                    //else if (_ScrollPosition.y > _ViewRect.y) _ScrollPosition.y = _ViewRect.y;
                }
            }
        }

        /// <summary>
        /// Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.
        /// </summary>
        public bool AlwayShowHorizontal { get; set; }
        /// <summary>
        /// Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.
        /// </summary>
        public bool AlwayShowVertical { get; set; }

        /// <summary>
        /// Optional parameter to handle ScrollWheel
        /// </summary>
        public bool HandleScrollWheel { get; set; }

        private Rect _ScrollViewRect;
        /// <summary>
        /// Gets view rectangle used for inner ScrollView
        /// </summary>
        public Rect ScrollViewRect { get { return _ScrollViewRect; } }


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

        /// <summary> Gets or sets a value that indicates the dimension by which child elements are stacked. </summary>
        public override Orientation Orientation
        {
            get { return base.Orientation; }
            set
            {
                Thickness padding = Padding;
                base.Orientation = value;
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
                    if (Orientation == UI.Orientation.Vertical)
                        padding.Right -= ScrollbarThickness;
                    else
                        padding.Bottom -= ScrollbarThickness;
                    return padding;
                }
            }
            set
            {
                if (Orientation == UI.Orientation.Vertical)
                    value.Right += ScrollbarThickness;
                else
                    value.Bottom += ScrollbarThickness;
                base.Padding = value;
            }
        }

        /// <summary>
        /// Create an instance of ScrollView
        /// </summary>
        public ListBox()
        {
            this.ScrollbarThickness = 16;
            this.HandleScrollWheel = true;
            this._SelectedItems = new List<BaseControl>();
            this.Background = new Box() { Parent = this, Visibility = Skill.Framework.UI.Visibility.Hidden };
            this.Padding = new Thickness(0);
        }

        /// <summary> Begin render control's content </summary>
        protected override void BeginRender()
        {
            _IgnoreScrollbarThickness = true;
            base.BeginRender();
            _IgnoreScrollbarThickness = false;
            Rect ra = RenderArea;
            _ScrollViewRect = ra;
            Size ds = DesiredSize;
            _ScrollViewRect.width = Mathf.Max(ra.width, ds.Width) - ScrollbarThickness;
            _ScrollViewRect.height = Mathf.Max(ra.height, ds.Height) - ScrollbarThickness;

            if (this.Background.Visibility == Skill.Framework.UI.Visibility.Visible)
            {
                this.Background.RenderArea = RenderArea;
                this.Background.OnGUI();
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

        private bool CheckForRemovedSelectedItems()
        {
            int index = 0;
            bool selectionChanged = false;
            while (index < _SelectedItems.Count)
            {
                if (!Controls.Contains(_SelectedItems[index]))
                {
                    _SelectedItems.RemoveAt(index);
                    selectionChanged = true;
                    continue;
                }
                index++;
            }
            if (selectionChanged)
            {
                _SelectedIndex = -1;
                if (SelectionMode == SelectionMode.Single && _SelectedItems.Count > 0)
                    _SelectedIndex = Controls.IndexOf(_SelectedItems[0]);
            }
            return selectionChanged;
        }

        /// <summary>
        /// Render ListBox
        /// </summary>
        protected override void Render()
        {

            //
            bool selectionChange = CheckForRemovedSelectedItems();

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
                        for (int i = 0; i < Controls.Count; i++)
                        {
                            var c = Controls[i];
                            Rect cRA = c.RenderArea;
                            if (cRA.Contains(mousePos))
                            {
                                if (SelectionMode == SelectionMode.Single)
                                {
                                    if (!_SelectedItems.Contains(c))
                                    {
                                        _SelectedItems.Clear();
                                        _SelectedItems.Add(c);
                                        _SelectedIndex = i;
                                        selectionChange = true;
                                    }
                                }
                                else if (SelectionMode == SelectionMode.Multiple)
                                {
                                    if (_SelectedItems.Contains(c))
                                        _SelectedItems.Remove(c);
                                    else
                                        _SelectedItems.Add(c);
                                    selectionChange = true;
                                }
                                else //if (SelectionMode == SelectionMode.Extended)
                                {
                                    if (e.modifiers == EventModifiers.Shift)
                                    {
                                        int firstSelectedIndex = i;
                                        // find first selected item after this item
                                        for (int j = i + 1; j < Controls.Count; j++)
                                        {
                                            if (_SelectedItems.Contains(Controls[j]))
                                            {
                                                firstSelectedIndex = j;
                                                break;
                                            }
                                        }

                                        if (firstSelectedIndex > i)
                                        {
                                            _SelectedItems.Clear();
                                            for (int k = i; k <= firstSelectedIndex; k++)
                                                _SelectedItems.Add(Controls[k]);
                                            selectionChange = true;
                                            break;
                                        }
                                        else
                                        {
                                            int lastSelectedIndex = i;
                                            // find last selected item before this item
                                            for (int j = i - 1; j >= 0; j--)
                                            {
                                                if (_SelectedItems.Contains(Controls[j]))
                                                {
                                                    lastSelectedIndex = j;
                                                    break;
                                                }
                                            }

                                            if (lastSelectedIndex < i)
                                            {
                                                _SelectedItems.Clear();
                                                for (int k = i; k >= lastSelectedIndex; k--)
                                                    _SelectedItems.Add(Controls[k]);
                                                selectionChange = true;
                                                break;
                                            }
                                        }
                                    }
                                    else if (e.modifiers == EventModifiers.Control)
                                    {
                                        if (_SelectedItems.Contains(c))
                                            _SelectedItems.Remove(c);
                                        else
                                            _SelectedItems.Add(c);
                                        selectionChange = true;
                                    }
                                    else
                                    {
                                        if (!_SelectedItems.Contains(c))
                                        {
                                            _SelectedItems.Clear();
                                            _SelectedItems.Add(c);
                                            selectionChange = true;
                                        }
                                        else if (_SelectedItems.Count > 1)
                                        {
                                            _SelectedItems.Clear();
                                            _SelectedItems.Add(c);
                                            selectionChange = true;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (selectionChange) OnSelectionChanged();

            if (_SelectedItems.Count > 0)
            {
                if (SelectedStyle == null)
                    SelectedStyle = GUI.skin.box;

                Rect boxRA = RenderAreaShrinksByPadding;
                if (Orientation == UI.Orientation.Vertical)
                {
                    boxRA.x += 1;
                    boxRA.width -= 2;
                }
                else
                {
                    boxRA.y += 1;
                    boxRA.height -= 2;
                }
                foreach (var c in _SelectedItems)
                {
                    Rect cRA = c.RenderArea;
                    Thickness cMargin = c.Margin;

                    if (Orientation == UI.Orientation.Vertical)
                    {
                        cRA.x = boxRA.x;
                        cRA.width = boxRA.width;

                        cRA.y -= cMargin.Top;
                        cRA.height += cMargin.Vertical;
                    }
                    else
                    {
                        cRA.x -= cMargin.Left;
                        cRA.width += cMargin.Horizontal;

                        cRA.y = boxRA.y;
                        cRA.height = boxRA.height;
                    }

                    GUI.Box(cRA, string.Empty, SelectedStyle);
                }
            }

            base.Render();
        }

        /// <summary> End Render control's content </summary>
        protected override void EndRender()
        {
            GUI.EndScrollView(HandleScrollWheel);
            base.EndRender();
        }

        /// <summary>
        /// Handle specified command. (Up and Down command to switch selected index)
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>        
        public override bool HandleCommand(UICommand command)
        {
            if (base.HandleCommand(command))
                return true;
            if (SelectionMode == UI.SelectionMode.Single)
            {
                if (command.Key == KeyCommand.Up)
                {
                    // try to select previous item
                    if (_SelectedIndex > 0)
                    {
                        SelectedIndex--;
                        OnSelectionChanged();
                        return true;
                    }
                }
                else if (command.Key == KeyCommand.Down)
                {
                    // try to select next item
                    if (_SelectedIndex < Controls.Count - 1)
                    {
                        SelectedIndex++;
                        OnSelectionChanged();
                        return true;
                    }
                }
            }
            else if (command.Shift)
            {
                if (command.Key == KeyCommand.Up)
                {
                    // try to add previous item to selection
                    int firstSelectedIndex = -1;
                    for (int i = 0; i < Controls.Count; i++)
                    {
                        if (_SelectedItems.Contains(Controls[i]))
                        {
                            firstSelectedIndex = i;
                            break;
                        }
                    }
                    if (firstSelectedIndex != -1 && firstSelectedIndex > 0)
                    {
                        _SelectedItems.Add(Controls[firstSelectedIndex - 1]);
                        OnSelectionChanged();
                        return true;
                    }
                }
                else if (command.Key == KeyCommand.Down)
                {
                    // try to add next item to selection
                    int lastSelectedIndex = -1;
                    // find first selected item after this item
                    for (int i = Controls.Count - 1; i >= 0; i--)
                    {
                        if (_SelectedItems.Contains(Controls[i]))
                        {
                            lastSelectedIndex = i;
                            break;
                        }
                    }
                    if (lastSelectedIndex != -1 && lastSelectedIndex < Controls.Count - 1)
                    {
                        _SelectedItems.Add(Controls[lastSelectedIndex + 1]);
                        OnSelectionChanged();
                        return true;
                    }
                }
                else if (_SelectedItems.Count == 1)
                {
                    if (command.Key == KeyCommand.Home)
                    {
                        // try to add previous items to selection
                        int selectedIndex = Controls.IndexOf(_SelectedItems[0]);
                        if (selectedIndex > 0)
                        {
                            bool change = false;
                            for (int k = selectedIndex - 1; k >= 0; k--)
                            {
                                _SelectedItems.Add(Controls[k]);
                                change = true;
                            }
                            if (change)
                                OnSelectionChanged();
                            return true;
                        }
                    }
                    else if (command.Key == KeyCommand.End)
                    {
                        // try to add next items to selection
                        int selectedIndex = Controls.IndexOf(_SelectedItems[0]);
                        if (selectedIndex < Controls.Count - 1)
                        {
                            bool change = false;
                            for (int k = selectedIndex + 1; k < Controls.Count; k++)
                            {
                                _SelectedItems.Add(Controls[k]);
                                change = true;
                            }
                            if (change)
                                OnSelectionChanged();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
