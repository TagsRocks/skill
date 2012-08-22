using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public class ActionCollection : ICollection<Action>
    {
        private List<Action> _Actions;

        public Action this[int index]
        {
            get { return _Actions[index]; }            
        }

        public int Count { get { return _Actions.Count; } }

        internal ActionCollection()
        {
            _Actions = new List<Action>();
        }

        public void Add(Action action)
        {
            if (!_Actions.Contains(action))
                _Actions.Add(action);
        }

        public bool Remove(Action action)
        {
            return _Actions.Remove(action);
        }

        public IEnumerator<Action> GetEnumerator()
        {
            return _Actions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Actions).GetEnumerator();
        }


        public void Clear()
        {
            _Actions.Clear();
        }

        public bool Contains(Action item)
        {
            return _Actions.Contains(item);
        }

        public void CopyTo(Action[] array, int arrayIndex)
        {
            _Actions.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
