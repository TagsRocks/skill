using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// Defines a collection of actions.
    /// </summary>
    public class ActionCollection : ICollection<Action>
    {
        private List<Action> _Actions; // internal list

        /// <summary>
        /// Retrieves Actions by index
        /// </summary>
        /// <param name="index">Zero based index of action</param>
        /// <returns>Action at given index</returns>
        public Action this[int index]
        {
            get { return _Actions[index]; }
        }

        /// <summary>
        /// Retrieves umber of actions in collection
        /// </summary>
        public int Count { get { return _Actions.Count; } }

        /// <summary>
        /// Create an ActionCollection
        /// </summary>
        internal ActionCollection()
        {
            _Actions = new List<Action>();
        }

        /// <summary>
        /// Add new action to collection
        /// </summary>
        /// <param name="action">Action to add</param>
        public void Add(Action action)
        {
            if (!_Actions.Contains(action))
                _Actions.Add(action);
        }

        /// <summary>
        /// Remove specified action from collection
        /// </summary>
        /// <param name="action">action to remove</param>
        /// <returns>True if action removed, otherwise false</returns>
        public bool Remove(Action action)
        {
            return _Actions.Remove(action);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.IEnumerator;ltAction;gt that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Action> GetEnumerator()
        {
            return _Actions.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.IEnumerator lt;Action gt; that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Actions).GetEnumerator();
        }

        /// <summary>
        /// Remove all actions from collection
        /// </summary>
        public void Clear()
        {
            _Actions.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific Action.
        /// </summary>
        /// <param name="item">The Action to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise,false.</returns>
        public bool Contains(Action item)
        {
            return _Actions.Contains(item);
        }

        //
        // Summary:
        //     
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from System.Collections.Generic.ICollection<T>. The System.Array must
        //     have zero-based indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     arrayIndex is less than 0.
        //
        //   System.ArgumentException:
        //     array is multidimensional.  -or- arrayIndex is equal to or greater than the
        //     length of array.  -or- The number of elements in the source System.Collections.Generic.ICollection<T>
        //     is greater than the available space from arrayIndex to the end of the destination
        //     array.  -or- Type T cannot be cast automatically to the type of the destination
        //     array.


        /// <summary>
        /// Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional System.Array that is the destination of the elements
        /// copied from collection. The System.Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="System.ArgumentException">
        /// array is multidimensional.  -or- arrayIndex is equal to or greater than the
        /// length of array.  -or- The number of elements in the source collection
        /// greater than the available space from arrayIndex to the end of the destination
        /// array. -or- Action cannot be cast automatically to the type of the destination array.
        /// </exception>
        public void CopyTo(Action[] array, int arrayIndex)
        {
            _Actions.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
