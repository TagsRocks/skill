using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.UI
{


    #region ColumnDefinition
    /// <summary>
    /// Defines column-specific properties that apply to Grid elements.
    /// </summary>
    public class ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of the ColumnDefinition class.
        /// </summary>
        public ColumnDefinition()
        {
        }


        /// <summary>
        /// Occurs when any changes happens to definition
        /// </summary>
        internal event EventHandler Change;
        private void OnChange()
        {
            if (Change != null) Change(this, EventArgs.Empty);
        }

        private GridLength _Width;
        /// <summary>
        ///  Gets the calculated width of a ColumnDefinition element,
        ///  or sets the GridLength value of a column that is defined by the ColumnDefinition.
        /// </summary>
        public GridLength Width
        {
            get { return _Width; }
            set
            {
                if (_Width != value)
                {
                    _Width = value; OnChange();
                }
            }
        }

        private float _MaxWidth;

        /// <summary>
        /// Gets or sets a value that represents the maximum width of a ColumnDefinition.
        /// </summary>
        public float MaxWidth
        {
            get { return _MaxWidth; }
            set
            {
                if (_MaxWidth != value)
                {
                    _MaxWidth = value;
                    if (_MaxWidth > 0 && _MaxWidth < _MinWidth) _MaxWidth = _MinWidth;
                    OnChange();
                }
            }
        }

        private float _MinWidth;
        /// <summary>
        /// Gets or sets a value that represents the minimum width of a ColumnDefinition.
        /// </summary>
        public float MinWidth
        {
            get { return _MinWidth; }
            set
            {
                if (_MinWidth != value)
                {
                    _MinWidth = value;
                    if (_MinWidth < 0) _MinWidth = 0;
                    else if (_MinWidth > _MaxWidth) _MinWidth = _MaxWidth;
                    OnChange();
                }
            }
        }

        /// <summary> Width of column after update layout </summary>
        public float RenderWidth { get; internal set; }
    }
    #endregion




    #region ColumnDefinitionCollection
    /// <summary>
    /// Provides access to an ordered, strongly typed collection of ColumnDefinition objects.
    /// </summary>
    public class ColumnDefinitionCollection : ICollection<ColumnDefinition>
    {
        // Variables
        private List<ColumnDefinition> _Columns;

        /// <summary>
        /// Gets a value that indicates the current item within a ColumnDefinitionCollection.
        /// </summary>
        /// <param name="index"> The current item in the collection.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is not a valid index position in the collection.</exception>
        public ColumnDefinition this[int index] { get { return _Columns[index]; } }

        /// <summary>
        /// Occurs when any changes happens to collection (Add, Remove, Clear)
        /// </summary>
        internal event EventHandler Change;
        private void OnChange()
        {
            if (Change != null) Change(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a instance of ColumnDefinitionCollection class
        /// </summary>
        public ColumnDefinitionCollection()
        {
            _Columns = new List<ColumnDefinition>();
        }

        /// <summary>
        /// Adds a ColumnDefinition element to a ColumnDefinitionCollection.
        /// </summary>
        /// <param name="value">Identifies the ColumnDefinition to add to the collection.</param>
        public void Add(ColumnDefinition value)
        {
            if (!_Columns.Contains(value))
            {
                _Columns.Add(value);
                value.Change += ColumnDefinition_Change;
                OnChange();
            }
        }

        /// <summary>
        /// Adds a ColumnDefinition element to a ColumnDefinitionCollection.
        /// </summary>
        /// <param name="value"> The initial value of this instance of GridLength. </param>
        /// <param name="type"> The GridUnitType held by this instance of GridLength. </param>
        public void Add(float value, GridUnitType type)
        {
            Add(new ColumnDefinition() { Width = new GridLength(value, type) });
        }

        void ColumnDefinition_Change(object sender, EventArgs e)
        {
            OnChange();
        }

        /// <summary>
        /// Clears the content of the ColumnDefinitionCollection.
        /// </summary>
        public void Clear()
        {
            foreach (var d in _Columns)
                d.Change -= ColumnDefinition_Change;
            _Columns.Clear();
            OnChange();
        }

        /// <summary>
        /// Determines whether a given ColumnDefinition exists within a ColumnDefinitionCollection.
        /// </summary>
        /// <param name="value"> Identifies the ColumnDefinition that is being tested. </param>
        /// <returns>true if the ColumnDefinition exists within the collection; otherwise false.</returns>
        public bool Contains(ColumnDefinition value)
        {
            return _Columns.Contains(value);
        }

        /// <summary>
        ///  Copies an array of ColumnDefinition objects to a given index position within a ColumnDefinitionCollection.
        /// </summary>
        /// <param name="array"> An array of ColumnDefinition objects. </param>
        /// <param name="arrayIndex"> Identifies the index position within array to which the ColumnDefinition objects are copied. </param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentException">array is multidimensional.-or- The number of elements in the source System.Collections.ICollection is greater
        /// than the available space from index to the end of the destination array.
        /// </exception>
        public void CopyTo(ColumnDefinition[] array, int arrayIndex)
        {
            _Columns.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the total number of items within this instance of ColumnDefinitionCollection.
        /// </summary>
        public int Count { get { return _Columns.Count; } }

        /// <summary>
        /// Gets a value that indicates whether a ColumnDefinitionCollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes a ColumnDefinition from a ColumnDefinitionCollection.
        /// </summary>
        /// <param name="value"> The ColumnDefinition to remove from the collection. </param>
        /// <returns> true if the ColumnDefinition was found in the collection and removed; otherwise, false. </returns>
        public bool Remove(ColumnDefinition value)
        {
            if (_Columns.Remove(value))
            {
                value.Change -= ColumnDefinition_Change;
                OnChange();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator;ltColumnDefinition;gt that can be used to iterate through the collection.</returns>
        public IEnumerator<ColumnDefinition> GetEnumerator()
        {
            return _Columns.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection. </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Columns).GetEnumerator();
        }
    }
    #endregion

}
