using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.UI
{
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
        Panel
    }
    #endregion

    #region BaseControl
    /// <summary>
    /// Defines base class for all controls
    /// </summary>
    public abstract class BaseControl
    {

        #region Properties

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
                    OnPositionChange();
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
                    OnPositionChange();
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
                    OnPositionChange();
                }
            }
        }


        /// <summary>
        /// Gets or sets Position.width
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
                    OnPositionChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets Position.height
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
                    OnPositionChange();
                }
            }
        }

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

        private Rect _PaintArea;
        /// <summary>
        /// The area that used to paint control.
        /// </summary>
        /// <remarks>
        /// if it is child of Scrollview or Group PaintArea is relative, otherwise it is absolute
        /// </remarks>
        public Rect PaintArea
        {
            get { return _PaintArea; }
            set
            {
                if (_PaintArea != value)
                {
                    _PaintArea = value;
                    OnPaintAreaChanged();
                }
            }
        }

        /// <summary> Parent Panel that host this control.(do not modify it manually) </summary>
        public BaseControl Parent { get; set; }

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
                    OnLayoutChanged();
                }
            }
        }

        #endregion

        #region Events
        /// <summary> Occurs when position of control changed </summary>
        public event EventHandler PositionChange;
        protected virtual void OnPositionChange()
        {
            if (PositionChange != null)
                PositionChange(this, EventArgs.Empty);
            OnLayoutChanged();
        }

        /// <summary> Occurs when PaintArea of control changed </summary>
        public event EventHandler PaintAreaChanged;
        protected virtual void OnPaintAreaChanged()
        {
            if (PaintAreaChanged != null) PaintAreaChanged(this, EventArgs.Empty);
        }
        /// <summary> Occurs when layout of control changed and parent panel needs to update layout of it's children</summary>
        public event EventHandler LayoutChanged;
        protected virtual void OnLayoutChanged()
        {
            if (LayoutChanged != null) LayoutChanged(this, EventArgs.Empty);
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
        }
        #endregion


        #region Abstract methods

        /// <summary> Specify type of Control  </summary>
        public abstract ControlType ControlType { get; }
        /// <summary> Paint control's content </summary>
        protected abstract void Paint(PaintParameters paintParams);
        /// <summary> Begin Paint control's content </summary>
        protected virtual void BeginPaint(PaintParameters paintParams) { }
        /// <summary> End Paint control's content </summary>
        protected virtual void EndPaint(PaintParameters paintParams) { }

        #endregion

        #region Public methods
        /// <summary>
        /// to paint control you have to call this method in OnGUI method of MonoBehavior.(call this for Frame class)
        /// </summary>
        public void OnGUI(PaintParameters paintParams)
        {
            if (Visibility == UI.Visibility.Visible)
            {
                BeginPaint(paintParams);
                Paint(paintParams);
                EndPaint(paintParams);
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
        /// Returns true if the x and y components of mousePosition is inside PaintArea.
        /// </summary>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="screenOffset">ScreenOffset (specified by PaintParams)</param>
        /// <returns>true if the x and y components of mousePosition is inside PaintArea, otherwise false</returns>
        public bool Containes(Vector2 mousePosition , Vector2 screenOffset)
        {
            mousePosition.x -= screenOffset.x;
            mousePosition.y -= screenOffset.y;
            return _PaintArea.Contains(mousePosition);
        }

        /// <summary>
        /// Returns true if the x and y components of point is inside PaintArea.
        /// </summary>
        /// <param name="point">Mouse position</param>        
        /// <returns>true if the x and y components of point is inside PaintArea, otherwise false</returns>
        public bool Containes(Vector2 point)
        {
            return _PaintArea.Contains(point);
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

        // Events

        /// <summary>
        ///  Occurs when any Control insid collection needs to update it's layout
        /// </summary>
        public event EventHandler LayoutChange;
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
            _Items = new List<BaseControl>();
            _LayoutChangeHandler = Control_LayoutChange;

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
            _Items.Add(control);
            control.Parent = Panel;
            control.LayoutChanged += _LayoutChangeHandler;
            OnLayoutChange();
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
        //     copied from collection. The System.Array must have zero-based indexing.</param>
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
            if (_Items.Remove(control))
            {
                control.Parent = null;
                control.LayoutChanged -= _LayoutChangeHandler;
                OnLayoutChange();
                return true;
            }
            return false;
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
    }
    #endregion

}