using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// Defines a stack that contains last running behaviors sequence
    /// </summary>
    class RunningStack
    {
        private BehaviorState _State;

        // defines two stact, one holds running actions at current update , other holds running actions at previous update
        Behavior[] _Stack1;
        Behavior[] _Stack2;

        Behavior[] _Stack; // active stack
        Behavior[] _PreStack; // active stack
        private int _TopIndex;// top index of stack , -1 means stack is empty

        /// <summary>
        /// Create an instance of RunningStack
        /// </summary>
        /// <param name="length">Maximum lenght of stack</param>
        public RunningStack(BehaviorState state, int length)
        {
            _State = state;
            _Stack1 = new Behavior[length];
            _Stack2 = new Behavior[length];
            _Stack = _Stack1;
            _PreStack = _Stack2;
            _TopIndex = -1;
        }

        /// <summary>
        /// Retrieves top of the stack
        /// </summary>
        public Behavior Top
        {
            get
            {
                if (_TopIndex >= 0)
                    return _Stack[_TopIndex];
                else
                    return null;
            }
        }

        /// <summary>
        /// Push new behavior to stack
        /// </summary>
        /// <param name="b">Behavior to push</param>
        public void Push(Behavior b)
        {
            _TopIndex++;
            _Stack[_TopIndex] = b;
        }

        /// <summary>
        /// Pop a Behavior from stack
        /// </summary>
        /// <returns>Popped Behavior</returns>
        public Behavior Pop()
        {
            if (_TopIndex < 0)
                throw new InvalidOperationException("Stack is empty : Invalid pop operation");
            Behavior b = _Stack[_TopIndex];
            _Stack[_TopIndex] = null;
            if (_PreStack[_TopIndex] == b)
                _PreStack[_TopIndex] = null;
            _TopIndex--;
            return b;
        }

        /// <summary>
        /// Swap current stack with previous stack and prepare for new update
        /// </summary>
        public void Swap()
        {
            if (_Stack == _Stack1)
            {
                _Stack = _Stack2;
                _PreStack = _Stack1;
            }
            else
            {
                _Stack = _Stack1;
                _PreStack = _Stack2;
            }
            _TopIndex = -1;
        }

        /// <summary>
        /// Call Reset behavior for running behaviors in previous stack
        /// </summary>
        public void ResetPreviousStack()
        {
            if (_Stack == _Stack1)
                ResetStack(_Stack1, _Stack2);
            else
                ResetStack(_Stack2, _Stack1);
        }

        /// <summary>
        /// Reset previous stack
        /// </summary>
        /// <param name="current">Current stack</param>
        /// <param name="previous">Previous stack</param>
        private void ResetStack(Behavior[] current, Behavior[] previous)
        {
            bool same = true;// this value is true untile current[i] == previous[i]. 
            for (int i = 0; i < previous.Length; i++)
            {
                Behavior b = previous[i];
                if (b == null) break;
                if (b != current[i] || !same) // when current and previous become unsame we should reset behavior
                {
                    same = false;
                    b.ResetBehavior();
                    if (b.Type == BehaviorType.Action)
                        _State.RunningActions.Remove((Action)b);
                }
                previous[i] = null;
            }
        }
    }
}
