using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    /// <summary>
    /// Running Action and it's parameters
    /// </summary>
    public class RunningAction
    {
        /// <summary>
        /// Action that is running
        /// </summary>
        public Action Action { get; private set; }
        
        /// <summary>
        /// Parameters of Action for internal use        
        /// </summary>
        public BehaviorParameterCollection Parameters { get; private set; }

        internal RunningAction(Action action, BehaviorParameterCollection parameters)
        {
            this.Action = action;
            this.Parameters = parameters;
        }
    }

    /// <summary>
    /// Defines a collection of actions.
    /// </summary>
    public class RunningActionCollection : IEnumerable<RunningAction>
    {
        private List<RunningAction> _Actions; // internal list

        /// <summary>
        /// Retrieves Actions by index
        /// </summary>
        /// <param name="index">Zero based index of action</param>
        /// <returns>Action at given index</returns>
        public RunningAction this[int index]
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
        internal RunningActionCollection()
        {
            _Actions = new List<RunningAction>();
        }

        /// <summary>
        /// Add new action to collection
        /// </summary>
        /// <param name="action">Action to add</param>
        /// <param name="parameters"> parameters of action </param>
        internal void Add(Action action, BehaviorParameterCollection parameters)
        {
            // the action already added
            if (!Contains(action))
                _Actions.Add(new RunningAction(action, parameters));
        }

        /// <summary>
        /// Remove specified action from collection
        /// </summary>
        /// <param name="action">action to remove</param>
        /// <returns>True if action removed, otherwise false</returns>
        internal bool Remove(Action action)
        {
            int removeIndex = -1;
            for (int i = 0; i < _Actions.Count; i++)
            {
                if (_Actions[i].Action == action)
                {
                    removeIndex = i;
                    break;
                }
            }
            if (removeIndex >= 0)
            {
                _Actions.RemoveAt(removeIndex);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.IEnumerator;ltAction;gt that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<RunningAction> GetEnumerator()
        {
            return _Actions.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.IEnumerator &lt; Action &gt; that can be used to iterate through the collection.
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
        /// <param name="action">The Action to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise,false.</returns>
        public bool Contains(Action action)
        {
            foreach (var ra in _Actions)
            {
                if (ra.Action == action)
                    return true;
            }
            return false;
        }
    }
}
