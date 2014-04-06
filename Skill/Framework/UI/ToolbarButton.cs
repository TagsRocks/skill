using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Framework.UI
{
    #region ToolbarButton
    /// <summary>
    /// item to show on the toolbar buttons.
    /// </summary>
    public class ToolbarButton
    {
        /// <summary>
        ///  text, image and tooltip for the toolbar button.
        /// </summary>
        public GUIContent Content { get; private set; }

        /// <summary> Index of button </summary>
        public int Index { get; internal set; }

        /// <summary> UserData  </summary>
        public object UserData { get; set; }

        private string _Name;
        /// <summary>
        /// Optional name for button
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                _Name = value;
            }
        }


        private bool _IsSelected;
        /// <summary>
        /// is button selected
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
        /// Occurs When item first got selected
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
        /// Occurs When item first time item lost selected
        /// </summary>
        protected virtual void OnUnselected()
        {
            if (Unselected != null)
                Unselected(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a ToolbarButton
        /// </summary>
        public ToolbarButton()
        {
            this.Index = -1;
            this.Content = new GUIContent();
            this.Name = "ToolbarButton";
        }

        
    }
    #endregion


    #region ToolbarButtonCollection
    /// <summary>
    /// A collection of ToolbarButton
    /// </summary>
    public sealed class ToolbarButtonCollection : ICollection<ToolbarButton>
    {
        private bool _ArrayChanged;
        private GUIContent[] _Contents;
        private List<ToolbarButton> _Items;

        /// <summary>
        /// Create a ToolbarButtonCollection
        /// </summary>
        internal ToolbarButtonCollection()
        {
            _ArrayChanged = true;
            _Items = new List<ToolbarButton>();
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
        /// Gets a value that indicates the current item within a ToolbarButtonCollection.
        /// </summary>
        /// <param name="index"> The current item in the collection.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is not a valid index position in the collection.</exception>
        public ToolbarButton this[int index]
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
        /// Adds a ToolbarButton element to a ToolbarButtonCollection.
        /// </summary>
        /// <param name="item">Identifies the ToolbarButton to add to the collection.</param>
        public void Add(ToolbarButton item)
        {
            if (item == null)
                throw new ArgumentNullException("ToolbarButton is null");
            if (item.Index != -1)
                throw new InvalidOperationException("Can not add ToolbarButton to more than one collection");

            _ArrayChanged = true;
            item.Index = _Items.Count;
            item.IsSelected = false;
            _Items.Add(item);
        }

        /// <summary>
        /// Clears the content of the ToolbarButtonCollection.
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
        /// Determines whether a given RowDefinition exists within a ToolbarButtonCollection.
        /// </summary>
        /// <param name="item"> Identifies the ToolbarButton that is being tested. </param>
        /// <returns>true if the ToolbarButton exists within the collection; otherwise false.</returns>
        public bool Contains(ToolbarButton item)
        {
            return _Items.Contains(item);
        }

        /// <summary>
        ///  Copies an array of ToolbarButton objects to a given index position within a ToolbarButtonCollection.
        /// </summary>
        /// <param name="array"> An array of ToolbarButton objects. </param>
        /// <param name="arrayIndex"> Identifies the index position within array to which the ToolbarButton objects are copied. </param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentException">array is multidimensional.-or- The number of elements in the source System.Collections.ICollection is greater
        /// than the available space from index to the end of the destination array.
        /// </exception>
        public void CopyTo(ToolbarButton[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the total number of items within this instance of ToolbarButtonCollection.
        /// </summary>
        public int Count
        {
            get { return _Items.Count; }
        }
        /// <summary>
        /// Gets a value that indicates whether a ToolbarButtonCollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Removes a ToolbarButton from a ToolbarButtonCollection.
        /// </summary>
        /// <param name="item"> The ToolbarButton to remove from the collection. </param>
        /// <returns> true if the ToolbarButton was found in the collection and removed; otherwise, false. </returns>
        public bool Remove(ToolbarButton item)
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
        /// <returns>A IEnumerator;ltToolbarButton;gt that can be used to iterate through the collection.</returns>
        public IEnumerator<ToolbarButton> GetEnumerator()
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
        /// return first occurence index of specified ToolbarButton
        /// </summary>
        /// <param name="item">item to find</param>
        /// <returns>
        /// > 0 index of ToolbarButton; otherwise -1.
        /// </returns>
        public int IndexOf(ToolbarButton item)
        {
            return _Items.IndexOf(item);
        }
    }
    #endregion
}