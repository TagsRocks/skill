    using System;
using System.Collections.Generic;
using UnityEngine;

namespace Skill.UI
{
    #region SelectionGridItem
    /// <summary>
    /// An item to show on the grid buttons
    /// </summary>
    public class SelectionGridItem
    {
        /// <summary>
        /// text, image and tooltips for the grid button.
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary> Optional name </summary>
        public String Name { get; set; }

        /// <summary> Index of item in collection</summary>
        public int Index { get; internal set; }

        private bool _IsSelected;
        /// <summary>
        /// is item selected
        /// </summary>
        public bool IsSelected
        {
            get { return _IsSelected; }
            internal set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    if (_IsSelected)
                        OnSelected();
                    else
                        OnUnselected();
                }
            }
        }

        /// <summary>
        /// Occurs When item first got selected
        /// </summary>
        public event EventHandler Selected;
        /// <summary>
        /// When item first got selected
        /// </summary>
        protected virtual void OnSelected()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs When item first time item lost selected
        /// </summary>
        public event EventHandler Unselected;
        /// <summary>
        /// When item first time item lost selected
        /// </summary>
        protected virtual void OnUnselected()
        {
            if (Unselected != null)
                Unselected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a SelectionGridItem
        /// </summary>
        public SelectionGridItem()
        {
            Content = new GUIContent();
            Index = -1;
        }
    } 
    #endregion

    #region SelectionGridItemCollection
    /// <summary>
    /// A collection of SelectionGridItem
    /// </summary>
    public sealed class SelectionGridItemCollection : ICollection<SelectionGridItem>
    {
        private bool _ArrayChanged;
        private GUIContent[] _Contents;
        private List<SelectionGridItem> _Items;

        /// <summary>
        /// Create a SelectionGridItemCollection
        /// </summary>
        internal SelectionGridItemCollection()
        {
            _ArrayChanged = true;
            _Items = new List<SelectionGridItem>();
        }

        /// <summary>
        /// Array of item's contents
        /// </summary>
        public GUIContent[] Contents
        {
            get
            {
                if (_ArrayChanged)
                {
                    _ArrayChanged = false;
                    RefreshContents();
                }
                return _Contents;
            }
        }

        /// <summary>
        /// Gets a value that indicates the current item within a SelectionGridItemCollection.
        /// </summary>
        /// <param name="index"> The current item in the collection.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is not a valid index position in the collection.</exception>
        public SelectionGridItem this[int index]
        {
            get { return _Items[index]; }
        }

        private void SetIndex()
        {
            for (int i = 0; i < _Items.Count; i++)
            {
                _Items[i].Index = i;
            }
        }

        /// <summary>
        /// Meka sure that Contents are valid. call this when change content of an item after add in collection
        /// </summary>
        public void RefreshContents()
        {
            _ArrayChanged = false;
            _Contents = new GUIContent[_Items.Count];
            for (int i = 0; i < _Items.Count; i++)
                _Contents[i] = _Items[i].Content;
        }

        /// <summary>
        /// Adds a SelectionGridItem element to a SelectionGridItemCollection.
        /// </summary>
        /// <param name="item">Identifies the SelectionGridItem to add to the collection.</param>
        public void Add(SelectionGridItem item)
        {
            if (item == null)
                throw new ArgumentNullException("SelectionGridItem is null");
            if (item.Index != -1)
                throw new InvalidOperationException("Can not add SelectionGridItem to more than one collection");

            _ArrayChanged = true;
            item.Index = _Items.Count;
            item.IsSelected = false;
            _Items.Add(item);
        }

        /// <summary>
        /// Clears the content of the SelectionGridItemCollection.
        /// </summary>
        public void Clear()
        {
            _ArrayChanged = true;
            for (int i = 0; i < _Items.Count; i++)
            {
                _Items[i].IsSelected = false;
                _Items[i].Index = -1;
            }
            _Items.Clear();
        }

        /// <summary>
        /// Determines whether a given RowDefinition exists within a SelectionGridItemCollection.
        /// </summary>
        /// <param name="item"> Identifies the SelectionGridItem that is being tested. </param>
        /// <returns>true if the SelectionGridItem exists within the collection; otherwise false.</returns>
        public bool Contains(SelectionGridItem item)
        {
            return _Items.Contains(item);
        }

        /// <summary>
        ///  Copies an array of SelectionGridItem objects to a given index position within a SelectionGridItemCollection.
        /// </summary>
        /// <param name="array"> An array of SelectionGridItem objects. </param>
        /// <param name="arrayIndex"> Identifies the index position within array to which the SelectionGridItem objects are copied. </param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentException">array is multidimensional.-or- The number of elements in the source System.Collections.ICollection is greater
        /// than the available space from index to the end of the destination array.
        /// </exception>
        public void CopyTo(SelectionGridItem[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the total number of items within this instance of SelectionGridItemCollection.
        /// </summary>
        public int Count
        {
            get { return _Items.Count; }
        }
        /// <summary>
        /// Gets a value that indicates whether a SelectionGridItemCollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Removes a SelectionGridItem from a SelectionGridItemCollection.
        /// </summary>
        /// <param name="item"> The SelectionGridItem to remove from the collection. </param>
        /// <returns> true if the SelectionGridItem was found in the collection and removed; otherwise, false. </returns>
        public bool Remove(SelectionGridItem item)
        {
            if (_Items.Remove(item))
            {
                item.Index = -1;
                item.IsSelected = false;
                _ArrayChanged = true;
                SetIndex();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator;ltSelectionGridItem;gt that can be used to iterate through the collection.</returns>
        public IEnumerator<SelectionGridItem> GetEnumerator()
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
        /// return first occurence index of specified SelectionGridItem
        /// </summary>
        /// <param name="item">item to find</param>
        /// <returns>
        /// > 0 index of SelectionGridItem; otherwise -1.
        /// </returns>
        public int IndexOf(SelectionGridItem item)
        {
            return _Items.IndexOf(item);
        }
    } 
    #endregion


}