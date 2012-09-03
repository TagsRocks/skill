using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Editor.UI
{
    #region PopupOption
    /// <summary>
    /// Option to display in Popup
    /// </summary>
    public class PopupOption
    {
        /// <summary>
        /// text,Image or tootip of option
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary> Optional name </summary>
        public String Name { get; set; }

        /// <summary> Index of item in collection</summary>
        public int Index { get; internal set; }

        /// <summary> Value of item</summary>
        public int Value { get; private set; }

        private bool _IsSelected;
        /// <summary>
        /// is option selected
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
        /// Occurs When option first got selected
        /// </summary>
        public event EventHandler Selected;
        protected virtual void OnSelected()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs When item first time option lost selected
        /// </summary>
        public event EventHandler Unselected;
        protected virtual void OnUnselected()
        {
            if (Unselected != null)
                Unselected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create an instance of PopupOption
        /// </summary>
        public PopupOption()
            : this(0)
        {
        }

        /// <summary>
        /// Create an instance of PopupOption
        /// </summary>
        /// <param name="value">Value of option</param>
        public PopupOption(int value)
        {
            this.Value = value;
            this.Content = new GUIContent();
            this.Name = string.Empty;
            this.Index = -1;
        }
    }
    #endregion

    #region PopupOptionCollection
    /// <summary>
    /// A collection of PopupOption
    /// </summary>
    public sealed class PopupOptionCollection : ICollection<PopupOption>
    {
        private bool _ArrayChanged;
        private GUIContent[] _Contents;
        private int[] _Values;
        private List<PopupOption> _Items;

        /// <summary>
        /// Create a PopupOptionCollection
        /// </summary>
        internal PopupOptionCollection()
        {
            _ArrayChanged = true;
            _Items = new List<PopupOption>();
        }

        /// <summary>
        /// Array of item's contents
        /// </summary>
        public GUIContent[] Contents
        {
            get
            {
                if (_ArrayChanged)
                    RefreshContents();
                return _Contents;
            }
        }

        /// <summary>
        /// Array of item's values
        /// </summary>
        public int[] Values
        {
            get
            {
                if (_ArrayChanged)
                    RefreshContents();
                return _Values;
            }
        }

        /// <summary>
        /// Gets a value that indicates the current item within a PopupOptionCollection.
        /// </summary>
        /// <param name="index"> The current item in the collection.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is not a valid index position in the collection.</exception>
        public PopupOption this[int index]
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
            _Values = new int[_Items.Count];
            for (int i = 0; i < _Items.Count; i++)
            {
                _Contents[i] = _Items[i].Content;
                _Values[i] = _Items[i].Value;
            }
        }

        /// <summary>
        /// Adds a PopupOption element to a PopupOptionCollection.
        /// </summary>
        /// <param name="item">Identifies the PopupOption to add to the collection.</param>
        public void Add(PopupOption item)
        {
            if (item == null)
                throw new ArgumentNullException("PopupOption is null");
            if (item.Index != -1)
                throw new InvalidOperationException("Can not add PopupOption to more than one collection");

            _ArrayChanged = true;
            item.Index = _Items.Count;
            item.IsSelected = false;
            _Items.Add(item);
        }

        /// <summary>
        /// Clears the content of the PopupOptionCollection.
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
        /// Determines whether a given RowDefinition exists within a PopupOptionCollection.
        /// </summary>
        /// <param name="item"> Identifies the PopupOption that is being tested. </param>
        /// <returns>true if the PopupOption exists within the collection; otherwise false.</returns>
        public bool Contains(PopupOption item)
        {
            return _Items.Contains(item);
        }

        /// <summary>
        ///  Copies an array of PopupOption objects to a given index position within a PopupOptionCollection.
        /// </summary>
        /// <param name="array"> An array of PopupOption objects. </param>
        /// <param name="arrayIndex"> Identifies the index position within array to which the PopupOption objects are copied. </param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentException">array is multidimensional.-or- The number of elements in the source System.Collections.ICollection is greater
        /// than the available space from index to the end of the destination array.
        /// </exception>
        public void CopyTo(PopupOption[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the total number of items within this instance of PopupOptionCollection.
        /// </summary>
        public int Count
        {
            get { return _Items.Count; }
        }
        /// <summary>
        /// Gets a value that indicates whether a PopupOptionCollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Removes a PopupOption from a PopupOptionCollection.
        /// </summary>
        /// <param name="item"> The PopupOption to remove from the collection. </param>
        /// <returns> true if the PopupOption was found in the collection and removed; otherwise, false. </returns>
        public bool Remove(PopupOption item)
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
        /// <returns>A IEnumerator;ltPopupOption;gt that can be used to iterate through the collection.</returns>
        public IEnumerator<PopupOption> GetEnumerator()
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
        /// return first occurence index of specified PopupOption
        /// </summary>
        /// <param name="item">item to find</param>
        /// <returns>
        /// > 0 index of PopupOption; otherwise -1.
        /// </returns>
        public int IndexOf(PopupOption item)
        {
            return _Items.IndexOf(item);
        }
    }
    #endregion
}
