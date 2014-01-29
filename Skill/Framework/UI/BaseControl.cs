using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Framework.UI
{

    #region IControl
    /// <summary>
    /// Defines Control interface
    /// </summary>
    public interface IControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether this element is enabled in the user interface (UI).
        /// </summary>
        /// <returns>  true if the element is enabled; otherwise, false. The default value is true. </returns>
        bool IsEnabled { get; set; }

        /// <summary> Position of control relative to parent </summary>
        Rect Position { get; set; }

        /// <summary> Gets or sets Position.X </summary>
        float X { get; set; }

        /// <summary> Gets or sets Position.Y </summary>
        float Y { get; set; }

        /// <summary> Gets or sets the suggested width of the element </summary>
        float Width { get; set; }

        /// <summary> Gets or sets the suggested height of the element. </summary>
        float Height { get; set; }

        /// <summary> Specify type of Control  </summary>
        ControlType ControlType { get; }

        /// <summary> Parent Panel that host this control.(this value should be setted by parent) </summary>
        IControl Parent { get; }
    }
    #endregion

    #region ControlType
    /// <summary>
    /// Types of Controls
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// Controls that hosted by panels
        /// </summary>
        Control,
        /// <summary>
        /// Panels that contains another Controls
        /// </summary>
        Panel,
        /// <summary>
        /// Frame 
        /// </summary>
        Frame
    }
    #endregion

    #region BaseControl
    /// <summary>
    /// Defines base class for all controls
    /// </summary>
    public abstract class BaseControl : IControl
    {
        #region Properties

        /// <summary>
        /// Indicates whether the element can receive focus.
        /// </summary>
        public virtual bool IsFocusable { get { return false; } }

        private bool _IsEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether this element is enabled in the user interface (UI).
        /// </summary>
        /// <returns>  true if the element is enabled; otherwise, false. The default value is true. </returns>
        public bool IsEnabled
        {
            get
            {
                if (Parent != null && Parent.ControlType != UI.ControlType.Frame && !Parent.IsEnabled) // if we reach frame break operation                
                    return false;
                else
                    return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
            }
        }


        private Rect _Position;
        /// <summary>
        /// Position of control relative to parent
        /// </summary>
        public Rect Position
        {
            get { return _Position; }
            set
            {
                if (_Position != value)
                {
                    _Position = value;
                    OnPositionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets Position.X
        /// </summary>
        public float X
        {
            get
            {
                return _Position.x;
            }
            set
            {
                if (_Position.x != value)
                {
                    _Position.x = value;
                    OnPositionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets Position.Y
        /// </summary>
        public float Y
        {
            get
            {
                return _Position.y;
            }
            set
            {
                if (_Position.y != value)
                {
                    _Position.y = value;
                    OnPositionChanged();
                }
            }
        }


        /// <summary>
        /// Gets or sets the suggested width of the element
        /// </summary>
        public float Width
        {
            get
            {
                return _Position.width;
            }
            set
            {
                if (_Position.width != value)
                {
                    _Position.width = value;
                    OnPositionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the suggested height of the element.
        /// </summary>
        public float Height
        {
            get
            {
                return _Position.height;
            }
            set
            {
                if (_Position.height != value)
                {
                    _Position.height = value;
                    OnPositionChanged();
                }
            }
        }

        /// <summary> Gets the final render size of this element. </summary>
        public Size RenderSize { get { return new Size(_RenderArea.width, _RenderArea.height); } }


        /// <summary>
        /// Retrieves Width used in layout. It is dependents on visibility and state of children
        /// </summary>
        public virtual float LayoutWidth
        {
            get
            {
                if (Visibility == UI.Visibility.Collapsed)
                    return 0;
                else
                    return _Position.width;
            }
        }

        /// <summary>
        /// Retrieves Height used in layout. It is dependents on visibility and state of children
        /// </summary>
        public virtual float LayoutHeight
        {
            get
            {
                if (Visibility == UI.Visibility.Collapsed)
                    return 0;
                else
                    return _Position.height;
            }
        }

        /// <summary> Gets the rendered width of this element. </summary>
        public float ActualWidth { get { return _RenderArea.width; } }

        /// <summary> Gets the rendered height of this element. </summary>
        public float ActualHeight { get { return _RenderArea.height; } }

        private Thickness _Margin;
        /// <summary> Gets or sets the outer margin of an element.</summary>        
        /// <returns>
        /// Provides margin values for the element. The default value is a System.Windows.Thickness with all properties equal to 0 (zero).
        /// </returns>
        public Thickness Margin
        {
            get
            {
                if (Visibility == UI.Visibility.Collapsed)
                    return Thickness.Empty;
                else
                    return _Margin;
            }
            set
            {
                if (_Margin != value)
                {
                    _Margin = value;
                    OnLayoutChanged();
                }
            }
        }

        private Rect _RenderArea;
        /// <summary>
        /// The area that used to render control.
        /// </summary>
        /// <remarks>
        /// if it is child of Scrollview or Group RenderArea is relative, otherwise it is absolute
        /// </remarks>
        public Rect RenderArea
        {
            get { return _RenderArea; }
            set
            {
                if (_RenderArea != value)
                {
                    _RenderArea = value;
                    OnRenderAreaChanged();
                }
            }
        }

        /// <summary> Parent Panel that host this control.(this value should be setted by parent) </summary>
        public virtual IControl Parent { get; set; }

        private VerticalAlignment _VerticalAlignment;

        /// <summary>
        /// Gets or sets the vertical alignment characteristics applied to this element
        /// when it is composed within a parent element such as a panel or items control.
        /// </summary>
        /// <returns> A vertical alignment setting. The default is VerticalAlignment.Stretch. </returns>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return _VerticalAlignment;
            }
            set
            {
                if (_VerticalAlignment != value)
                {
                    _VerticalAlignment = value;
                    OnLayoutChanged();
                }
            }
        }

        private HorizontalAlignment _HorizontalAlignment;
        /// <summary>
        /// Gets or sets the horizontal alignment characteristics applied to this element
        /// when it is composed within a parent element, such as a panel or items control.
        /// </summary>
        /// <returns> A horizontal alignment setting, as a value of the enumeration. The default is HorizontalAlignment.Stretch. </returns>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return _HorizontalAlignment;
            }
            set
            {
                if (_HorizontalAlignment != value)
                {
                    _HorizontalAlignment = value;
                    OnLayoutChanged();
                }
            }
        }

        /// <summary>
        /// Name of control. (optional)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The tag of this control. (optional)
        /// </summary>
        public string Tag { get; set; }

        private Dock _Dock;
        /// <summary>
        /// Dock of Control when it is a child of DockPanel
        /// </summary>
        public Dock Dock
        {
            get
            {
                return _Dock;
            }
            set
            {

                if (_Dock != value)
                {
                    _Dock = value;
                    OnLayoutChanged();
                }
            }
        }

        private int _Row;
        /// <summary>
        /// Grid.Row when it is a child of Grid panel
        /// </summary>
        public int Row
        {
            get { return _Row; }
            set
            {
                if (_Row != value)
                {
                    _Row = value;
                    if (_Row < 0) _Row = 0;
                    OnLayoutChanged();
                }
            }
        }

        private int _Column;
        /// <summary>
        /// Grid.Column when it is a child of Grid panel
        /// </summary>
        public int Column
        {
            get { return _Column; }
            set
            {
                if (_Column != value)
                {
                    _Column = value;
                    if (_Row < 0) _Column = 0;
                    OnLayoutChanged();
                }
            }
        }


        private int _RowSpan;
        /// <summary>
        /// Grid.RowSpan when it is a child of Grid panel
        /// </summary>
        public int RowSpan
        {
            get { return _RowSpan; }
            set
            {
                if (_RowSpan != value)
                {
                    _RowSpan = value;
                    if (_RowSpan < 1) _RowSpan = 1;
                    OnLayoutChanged();
                }
            }
        }

        private int _ColumnSpan;
        /// <summary>
        /// Grid.ColumnSpan when it is a child of Grid panel
        /// </summary>
        public int ColumnSpan
        {
            get { return _ColumnSpan; }
            set
            {
                if (_ColumnSpan != value)
                {
                    _ColumnSpan = value;
                    if (_ColumnSpan < 1) _ColumnSpan = 1;
                    OnLayoutChanged();
                }
            }
        }

        private Visibility _Visibility;
        /// <summary>
        /// Gets or sets the user interface (UI) visibility of this element.
        /// </summary>
        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    OnVisibilityChanged();
                }
            }
        }

        private Frame _OwnerFrame;
        /// <summary>
        /// Retrieves Owner frame
        /// </summary>
        public Frame OwnerFrame
        {
            get
            {
                if (_OwnerFrame == null)
                    FindFrame();
                return _OwnerFrame;
            }
        }

        private void FindFrame()
        {
            IControl parent = Parent;
            while (parent != null)
            {
                if (parent.ControlType == UI.ControlType.Frame)
                {
                    _OwnerFrame = parent as Frame;
                    break;
                }
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// Is mouse over the control
        /// </summary>
        public bool IsMouseOver { get; private set; }

        #endregion

        #region Events
        /// <summary> Occurs when position of control changed </summary>
        public event EventHandler PositionChanged;
        /// <summary>
        /// when position of control changed 
        /// </summary>
        protected virtual void OnPositionChanged()
        {
            if (PositionChanged != null)
                PositionChanged(this, EventArgs.Empty);
            OnLayoutChanged();
        }

        /// <summary> Occurs when RenderArea of control changed </summary>
        public event EventHandler RenderAreaChanged;
        /// <summary>
        /// when RenderArea of control changed
        /// </summary>
        protected virtual void OnRenderAreaChanged()
        {
            if (RenderAreaChanged != null) RenderAreaChanged(this, EventArgs.Empty);
        }
        /// <summary> Occurs when layout of control changed and parent control needs to update layout of it's children</summary>
        public event EventHandler LayoutChanged;
        /// <summary>
        /// when layout of control changed and parent control needs to update layout of it's children
        /// </summary>
        protected virtual void OnLayoutChanged()
        {
            if (LayoutChanged != null) LayoutChanged(this, EventArgs.Empty);
        }

        /// <summary> Occurs when Visibility of control changed </summary>
        public event EventHandler VisibilityChanged;
        /// <summary>
        /// when Visibility of control changed
        /// </summary>
        protected virtual void OnVisibilityChanged()
        {
            if (VisibilityChanged != null)
                VisibilityChanged(this, EventArgs.Empty);
            OnLayoutChanged();
        }

        /// <summary> Occurs when mouse enters control.(if WantsMouseEvents = true)</summary>
        public event MouseEventHandler MouseEnter;
        /// <summary>
        /// Occurs when mouse enters control (if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseEventArgs </param>
        protected virtual void OnMouseEnter(MouseEventArgs args)
        {
            if (MouseEnter != null)
                MouseEnter(this, args);
        }

        /// <summary> Occurs when mouse leaves control.(if WantsMouseEvents = true)</summary>
        public event MouseEventHandler MouseLeave;
        /// <summary>
        /// Occurs when mouse leaves control (if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseEventArgs </param>
        protected virtual void OnMouseLeave(MouseEventArgs args)
        {
            if (MouseLeave != null)
                MouseLeave(this, args);
        }

        /// <summary> Occurs when mouse button was pressed.(if WantsMouseEvents = true)</summary>
        public event MouseClickEventHandler MouseDown;
        /// <summary>
        /// Occurs when mouse button was pressed.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseClickEventArgs </param>
        protected virtual void OnMouseDown(MouseClickEventArgs args)
        {
            if (MouseDown != null)
                MouseDown(this, args);
        }

        /// <summary> Occurs when mouse button was released.(if WantsMouseEvents = true) </summary>
        public event MouseClickEventHandler MouseUp;
        /// <summary>
        /// Occurs when mouse button was released.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseClickEventArgs </param>
        protected virtual void OnMouseUp(MouseClickEventArgs args)
        {
            if (MouseUp != null)
                MouseUp(this, args);
        }

        /// <summary> Occurs when mouse was dragged.(if WantsMouseEvents = true) </summary>
        public event MouseMoveEventHandler MouseDrag;
        /// <summary>
        /// Occurs when mouse was dragged.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseMoveEventArgs </param>
        protected virtual void OnMouseDrag(MouseMoveEventArgs args)
        {
            if (MouseDrag != null)
                MouseDrag(this, args);
        }


        /// <summary> Occurs when mouse was dragged.(if WantsMouseEvents = true)(works only in EditorWindow with set wantsMouseMove true) </summary>
        public event MouseMoveEventHandler MouseMove;
        /// <summary>
        /// Occurs when mouse was dragged.(if WantsMouseEvents = true)(works only in EditorWindow with set wantsMouseMove true)
        /// </summary>
        /// <param name="args"> MouseMoveEventArgs </param>
        protected virtual void OnMouseMove(MouseMoveEventArgs args)
        {
            if (MouseMove != null)
                MouseMove(this, args);
        }

        /// <summary> Occurs when The scroll wheel was moved.(if WantsMouseEvents = true) </summary>
        public event MouseMoveEventHandler ScrollWheel;
        /// <summary>
        /// Occurs when The scroll wheel was moved.(if WantsMouseEvents = true)
        /// </summary>
        /// <param name="args"> MouseMoveEventArgs </param>
        protected virtual void OnScrollWheel(MouseMoveEventArgs args)
        {
            if (ScrollWheel != null)
                ScrollWheel(this, args);
        }

        /// <summary> check mouse events? (MouseDown, MouseUp, MouseDrag, MouseMove, ScrollWheel) </summary>        
        public bool WantsMouseEvents { get; set; }

        /// <summary>
        /// Convert mouse to local position
        /// </summary>
        /// <param name="mousePosition">Position of mouse</param>
        protected virtual Vector2 ConvertToLocal(Vector2 mousePosition)
        {
            return mousePosition;
        }

        protected virtual MouseButton ConvertButton(int ebutton)
        {
            MouseButton mb = MouseButton.None;
            if (ebutton >= 0)
            {
                ebutton++;
                if (ebutton <= (int)MouseButton.Other) mb = (MouseButton)ebutton;
                else mb = MouseButton.Other;
            }
            return mb;
        }

        /// <summary> ContextMenu </summary>
        public IContextMenu ContextMenu { get; set; }


        /// <summary>
        /// Check for events
        /// </summary>
        protected virtual void CheckEvents()
        {
            Event e = Event.current;
            if (e != null)
            {
                if (WantsMouseEvents)
                {
                    if (e.isMouse)
                    {
                        EventType type = e.type;
                        Vector2 localMouse = ConvertToLocal(e.mousePosition);
                        if (_RenderArea.Contains(localMouse))
                        {
                            if (!IsMouseOver)
                            {
                                IsMouseOver = true;
                                OnMouseEnter(new MouseEventArgs(e.mousePosition, e.modifiers));
                            }

                            if (type == EventType.MouseDown || type == EventType.MouseUp)
                            {
                                MouseButton mb = ConvertButton(e.button);
                                MouseClickEventArgs args = new MouseClickEventArgs(e.mousePosition, e.modifiers, mb, e.clickCount);
                                if (type == EventType.MouseDown)
                                    OnMouseDown(args);
                                else
                                    OnMouseUp(args);
                                if (args.Handled)
                                    e.Use();
                            }
                            else if (type == EventType.ScrollWheel || type == EventType.MouseMove || type == EventType.MouseDrag)
                            {
                                MouseButton mb = ConvertButton(e.button);
                                MouseMoveEventArgs args = new MouseMoveEventArgs(e.mousePosition, e.modifiers, mb, e.delta);
                                if (type == EventType.ScrollWheel)
                                    OnScrollWheel(args);
                                else if (type == EventType.MouseMove)
                                    OnMouseMove(args);
                                else
                                    OnMouseDrag(args);
                                if (args.Handled)
                                    e.Use();
                            }

                        }
                        else
                        {
                            if (IsMouseOver)
                            {
                                IsMouseOver = false;
                                OnMouseLeave(new MouseEventArgs(e.mousePosition, e.modifiers));
                            }

                        }
                    }
                }
                else if (ContextMenu != null && e.type == EventType.ContextClick)
                {
                    Vector2 localMouse = ConvertToLocal(e.mousePosition);
                    if (_RenderArea.Contains(localMouse))
                    {
                        ContextMenu.Show();
                        e.Use();
                    }
                }
            }
        }


        #endregion

        #region Contstructor
        /// <summary>
        /// Create a BaseControl
        /// </summary>
        protected BaseControl()
        {
            this._VerticalAlignment = UI.VerticalAlignment.Stretch;
            this._HorizontalAlignment = UI.HorizontalAlignment.Stretch;
            this.Dock = UI.Dock.Top;
            this._Row = 0;
            this._Column = 0;
            this._RowSpan = 1;
            this._ColumnSpan = 1;
            this.Width = 100;
            this.Height = 16;
            this.IsEnabled = true;
        }
        #endregion

        #region Abstract methods

        private bool _ApplyGUIEnableCalled;

        /// <summary> Specify type of Control  </summary>
        public abstract ControlType ControlType { get; }
        /// <summary> Render control's content </summary>
        protected abstract void Render();
        /// <summary> Begin Render control's content </summary>
        protected virtual void BeginRender()
        {
            if (Parent != null && Parent.ControlType != UI.ControlType.Frame)
            {
                if (Parent.IsEnabled && !_IsEnabled)
                {
                    ApplyGUIEnable(false);
                    _ApplyGUIEnableCalled = true;
                }
            }
            else
            {
                ApplyGUIEnable(_IsEnabled);
                _ApplyGUIEnableCalled = true;
            }
        }
        /// <summary> End Render control's content </summary>
        protected virtual void EndRender()
        {
            if (_ApplyGUIEnableCalled)
            {
                RestoreGUIEnable();
                _ApplyGUIEnableCalled = false;
            }
        }

        /// <summary> Make control enabled or disabled</summary>
        /// <param name="enable">Enabled value</param>        
        protected virtual void ApplyGUIEnable(bool enable)
        {
            GUI.enabled = enable;
        }

        /// <summary>
        /// Restore previous value of GUI enable
        /// </summary>        
        protected virtual void RestoreGUIEnable()
        {
            GUI.enabled = true;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// to render control you have to call this method in OnGUI method of MonoBehavior.(call this for Frame class)
        /// </summary>
        public void OnGUI()
        {
            if (Visibility == UI.Visibility.Visible)
            {
                BeginRender();
                CheckEvents();
                Render();
                EndRender();
            }
        }


        /// <summary> Attempts to bring this element to front. </summary>
        public void BringToFront()
        {
            if (Parent != null && Parent.ControlType == UI.ControlType.Panel)
                ((Panel)Parent).Controls.BringToFront(this);
        }
        /// <summary> Attempts to bring this element to back. </summary>
        public void BringToBack()
        {
            if (Parent != null && Parent.ControlType == UI.ControlType.Panel)
                ((Panel)Parent).Controls.BringToBack(this);
        }

        /// <summary>
        /// Returns true if the x and y components of mousePosition is inside RenderArea.
        /// </summary>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="screenOffset">ScreenOffset (specified by RenderParams)</param>
        /// <returns>true if the x and y components of mousePosition is inside RenderArea, otherwise false</returns>
        public bool Containes(Vector2 mousePosition, Vector2 screenOffset)
        {
            mousePosition.x -= screenOffset.x;
            mousePosition.y -= screenOffset.y;
            return _RenderArea.Contains(mousePosition);
        }

        /// <summary>
        /// Returns true if the x and y components of point is inside RenderArea.
        /// </summary>
        /// <param name="point">Mouse position</param>        
        /// <returns>true if the x and y components of point is inside RenderArea, otherwise false</returns>
        public bool Containes(Vector2 point)
        {
            return _RenderArea.Contains(point);
        }

        /// <summary>
        /// Handle specified command
        /// </summary>
        /// <param name="command">Command to handle</param>
        /// <returns>True if command is handled, otherwise false</returns>        
        public virtual bool HandleCommand(UICommand command)
        {
            return false;
        }
        #endregion
    }
    #endregion

    #region BaseControlCollection
    /// <summary>
    /// Defines methods to manipulate collection of BaseControls.
    /// </summary>
    public class BaseControlCollection : ICollection<BaseControl>
    {
        // Variables
        private EventHandler _LayoutChangeHandler;
        private List<BaseControl> _Items;


        // the following variables defined to avoid modification of collection during render        
        private bool _IsRenderBegined;
        enum CollectionChangeType
        {
            Add,
            Remove,
            Insert
        }

        class CollectionChange
        {
            public BaseControl Control;
            public CollectionChangeType ChangeType;
            public int Index;
        }

        private List<CollectionChange> _Changes;

        // Events

        /// <summary>
        ///  Occurs when any Control insid collection needs to update it's layout
        /// </summary>
        public event EventHandler LayoutChange;
        /// <summary>
        /// when any Control insid collection needs to update it's layout
        /// </summary>
        protected virtual void OnLayoutChange()
        {
            if (LayoutChange != null) LayoutChange(this, EventArgs.Empty);
        }

        /// <summary>
        /// Panel that use this collection
        /// </summary>
        public Panel Panel { get; private set; }

        /// <summary>
        /// Create a BaseControlCollection
        /// </summary>
        /// <param name="panel"> Panel that use this collection</param>
        internal BaseControlCollection(Panel panel)
        {
            this.Panel = panel;
            _Changes = new List<CollectionChange>();
            _Items = new List<BaseControl>();
            _LayoutChangeHandler = Control_LayoutChange;

        }

        internal void BeginRender()
        {
            _IsRenderBegined = true;
        }

        internal void EndRender()
        {
            _IsRenderBegined = false;
            if (_Changes.Count > 0)
            {
                for (int i = 0; i < _Changes.Count; i++)
                {
                    switch (_Changes[i].ChangeType)
                    {
                        case CollectionChangeType.Add:
                            Add(_Changes[i].Control);
                            break;
                        case CollectionChangeType.Remove:
                            Remove(_Changes[i].Control);
                            break;
                        case CollectionChangeType.Insert:
                            Insert(_Changes[i].Index, _Changes[i].Control);
                            break;
                    }
                }
                _Changes.Clear();
            }
        }

        void Control_LayoutChange(object sender, EventArgs e)
        {
            OnLayoutChange();
        }

        /// <summary>
        /// Retrieves controls by index
        /// </summary>
        /// <param name="index">index of control</param>
        /// <returns>Control at specified index</returns>
        public BaseControl this[int index]
        {
            get { return _Items[index]; }
        }


        /// <summary>
        /// Adds an BaseControl to the Collection.
        /// </summary>
        /// <param name="control"> The BaseControl to add to Collection </param>
        public void Add(BaseControl control)
        {
            if (control == null)
                throw new ArgumentNullException("BaseControl is null");
            if (_Items.Contains(control)) return;

            if (_IsRenderBegined)
            {
                _Changes.Add(new CollectionChange() { ChangeType = CollectionChangeType.Add, Control = control });
            }
            else
            {
                _Items.Add(control);
                control.Parent = Panel;
                control.LayoutChanged += _LayoutChangeHandler;
                OnLayoutChange();
            }
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="control"> The BaseControl to add to Collection </param>                
        public void Insert(int index, BaseControl control)
        {
            if (control == null)
                throw new ArgumentNullException("BaseControl is null");
            if (_Items.Contains(control)) return;
            if (_IsRenderBegined)
            {
                _Changes.Add(new CollectionChange() { ChangeType = CollectionChangeType.Insert, Control = control, Index = index });
            }
            else
            {
                _Items.Insert(index, control);
                control.Parent = Panel;
                control.LayoutChanged += _LayoutChangeHandler;
                OnLayoutChange();
            }
        }

        /// <summary>
        /// Removes all Controls from the collection
        /// </summary>
        public void Clear()
        {
            foreach (var c in _Items)
            {
                c.Parent = null;
                c.LayoutChanged -= _LayoutChangeHandler;
            }
            _Items.Clear();
            OnLayoutChange();
        }

        /// <summary>
        /// Determines whether the collection contains a specific Control
        /// </summary>
        /// <param name="control">The Cotrol to locate in collection</param>
        /// <returns> true if Control is found in the collection; otherwise, false.</returns>
        public bool Contains(BaseControl control)
        {
            return _Items.Contains(control);
        }

        /// <summary>
        /// Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements
        /// copied from collection. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">
        /// array is multidimensional.-or-The number of elements in the source collection
        /// is greater than the available space from arrayIndex to the end of the destination
        /// array.-or-Type T cannot be cast automatically to the type of the destination array.
        /// </exception>
        public void CopyTo(BaseControl[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of Controls contained in the collection.
        /// </summary>
        public int Count
        {
            get { return _Items.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes  the first occurrence of a specific control from the collection.
        /// </summary>
        /// <param name="control"> The BaseControl to remove from the collection </param>
        /// <returns>
        /// true if item was successfully removed from the collection otherwise, false.
        /// This method also returns false if item is not found in the original collection
        /// </returns>
        public bool Remove(BaseControl control)
        {
            if (_IsRenderBegined)
            {
                _Changes.Add(new CollectionChange() { ChangeType = CollectionChangeType.Remove, Control = control });
                return _Items.Contains(control);
            }
            else
            {
                if (_Items.Remove(control))
                {
                    control.Parent = null;
                    control.LayoutChanged -= _LayoutChangeHandler;
                    OnLayoutChange();
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator;ltBaseControl;gt that can be used to iterate through the collection.</returns>
        public IEnumerator<BaseControl> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection. </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Items as System.Collections.IEnumerable).GetEnumerator();
        }


        /// <summary>
        /// Attempts to bring this element to front.
        /// </summary>
        /// <param name="control">BaseControl to bring to front</param>
        internal void BringToFront(BaseControl control)
        {
            if (Remove(control))
            {
                _Items.Insert(Count, control);
            }
        }

        /// <summary>
        /// Attempts to bring this element to back.
        /// </summary>
        /// <param name="control">BaseControl to bring to back</param>
        internal void BringToBack(BaseControl control)
        {
            if (Remove(control))
            {
                _Items.Insert(0, control);
            }
        }


        /// <summary>
        /// Searches for the specified BaseControl and returns the zero-based index of the first occurrence within the entire Controls.
        /// </summary>
        /// <param name="control">The BaseControl to locate in the Controls.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire Controls, if found; otherwise, –1.
        /// </returns>
        public int IndexOf(BaseControl control)
        {
            return _Items.IndexOf(control);
        }
    }
    #endregion

}