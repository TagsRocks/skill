using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public enum CompositeType
    {
        Sequence,
        Concurrent,
        Random,
        Priority,
        Loop,
    }

    public abstract class Composite : Behavior, ICollection<Behavior>
    {
        private List<Behavior> _Children;
        public abstract CompositeType CompositeType { get; }
        public int RunningChildIndex { get; protected set; }

        public Composite(string name)
            : base(name, BehaviorType.Composite)
        {
            _Children = new List<Behavior>();
        }

        public Behavior this[int index] { get { return _Children[index]; } }

        public virtual void Add(Behavior item)
        {
            if (item != null)
            {
                if (_Children.Contains(item)) return;
                _Children.Add(item);
            }
        }

        public virtual void Clear()
        {
            _Children.Clear();
        }

        public bool Contains(Behavior item)
        {
            if (item != null)
            {
                return _Children.Contains(item);
            }
            return false;
        }

        public void CopyTo(Behavior[] array, int arrayIndex)
        {
            _Children.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Children.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool Remove(Behavior item)
        {
            if (item != null)
            {
                return _Children.Remove(item);
            }
            return false;
        }

        public IEnumerator<Behavior> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_Children as System.Collections.IEnumerable).GetEnumerator();
        }        
    }
}
