using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.UI
{
    #region RowDefinition
    /// <summary>
    /// Defines row-specific properties that apply to Grid elements.
    /// </summary>
    public class RowDefinition
    {
        /// <summary>
        /// Initializes a new instance of the RowDefinition class.
        /// </summary>
        public RowDefinition()
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

        private GridLength _Height;
        /// <summary>
        ///  Gets the calculated height of a RowDefinition element,
        ///  or sets the GridLength value of a column that is defined by the RowDefinition.
        /// </summary>
        public GridLength Height
        {
            get { return _Height; }
            set
            {
                if (_Height != value)
                {
                    _Height = value; OnChange();
                }
            }
        }

        private float _MaxHeight;
        /// <summary>
        /// Gets or sets a value that represents the maximum height of a RowDefinition.
        /// </summary>
        public float MaxHeight
        {
            get { return _MaxHeight; }
            set
            {
                if (_MaxHeight != value)
                {
                    _MaxHeight = value;
                    if (_MaxHeight > 0 && _MaxHeight < _MinHeight) _MaxHeight = _MinHeight;
                    OnChange();
                }
            }
        }

        private float _MinHeight;
        /// <summary>
        /// Gets or sets a value that represents the minimum height of a RowDefinition.
        /// </summary>
        public float MinHeight
        {
            get { return _MinHeight; }
            set
            {
                if (_MinHeight != value)
                {
                    _MinHeight = value;
                    if (_MinHeight < 0) _MinHeight = 0;
                    else if (_MinHeight > _MaxHeight) _MinHeight = _MaxHeight;
                    OnChange();
                }
            }
        }


        /// <summary> Height of row after update layout </summary>
        public float RenderHeight { get; internal set; }

    }
    #endregion

    #region RowDefinitionCollection
    /// <summary>
    /// Provides access to an ordered, strongly typed collection of RowDefinition objects.
    /// </summary>
    public class RowDefinitionCollection : ICollection<RowDefinition>
    {
        // Variables
        private List<RowDefinition> _Rows;

        /// <summary>
        /// Gets a value that indicates the current item within a RowDefinitionCollection.
        /// </summary>
        /// <param name="index"> The current item in the collection.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is not a valid index position in the collection.</exception>
        public RowDefinition this[int index] { get { return _Rows[index]; } }

        /// <summary>
        /// Occurs when any changes happens to collection (Add, Remove, Clear)
        /// </summary>
        internal event EventHandler Change;
        private void OnChange()
        {
            if (Change != null) Change(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a instance of RowDefinitionCollection class
        /// </summary>
        public RowDefinitionCollection()
        {
            _Rows = new List<RowDefinition>();
        }

        /// <summary>
        /// Adds a RowDefinition element to a RowDefinitionCollection.
        /// </summary>
        /// <param name="value">Identifies the RowDefinition to add to the collection.</param>
        public void Add(RowDefinition value)
        {
            if (!_Rows.Contains(value))
            {
                _Rows.Add(value);
                value.Change += RowDefinition_Change;
                OnChange();
            }
        }

        /// <summary>
        /// Adds a RowDefinition element to a ColumnDefinitionCollection.
        /// </summary>
        /// <param name="value"> The initial value of this instance of GridLength. </param>
        /// <param name="type"> The GridUnitType held by this instance of GridLength. </param>
        public void Add(float value, GridUnitType type)
        {
            Add(new RowDefinition() { Height = new GridLength(value, type) });
        }

        void RowDefinition_Change(object sender, EventArgs e)
        {
            OnChange();
        }

        /// <summary>
        /// Clears the content of the RowDefinitionCollection.
        /// </summary>
        public void Clear()
        {
            foreach (var d in _Rows)
                d.Change -= RowDefinition_Change;
            _Rows.Clear();
            OnChange();
        }

        /// <summary>
        /// Determines whether a given RowDefinition exists within a RowDefinitionCollection.
        /// </summary>
        /// <param name="value"> Identifies the RowDefinition that is being tested. </param>
        /// <returns>true if the RowDefinition exists within the collection; otherwise false.</returns>
        public bool Contains(RowDefinition value)
        {
            return _Rows.Contains(value);
        }

        /// <summary>
        ///  Copies an array of RowDefinition objects to a given index position within a RowDefinitionCollection.
        /// </summary>
        /// <param name="array"> An array of RowDefinition objects. </param>
        /// <param name="arrayIndex"> Identifies the index position within array to which the RowDefinition objects are copied. </param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentException">array is multidimensional.-or- The number of elements in the source System.Collections.ICollection is greater
        /// than the available space from index to the end of the destination array.
        /// </exception>
        public void CopyTo(RowDefinition[] array, int arrayIndex)
        {
            _Rows.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the total number of items within this instance of RowDefinitionCollection.
        /// </summary>
        public int Count { get { return _Rows.Count; } }

        /// <summary>
        /// Gets a value that indicates whether a RowDefinitionCollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes a RowDefinition from a RowDefinitionCollection.
        /// </summary>
        /// <param name="value"> The RowDefinition to remove from the collection. </param>
        /// <returns> true if the RowDefinition was found in the collection and removed; otherwise, false. </returns>
        public bool Remove(RowDefinition value)
        {
            if (_Rows.Remove(value))
            {
                value.Change -= RowDefinition_Change;
                OnChange();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator;ltRowDefinition;gt that can be used to iterate through the collection.</returns>
        public IEnumerator<RowDefinition> GetEnumerator()
        {
            return _Rows.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection. </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Rows).GetEnumerator();
        }
    }
    #endregion

}
