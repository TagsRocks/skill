using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public class LoopSelector : Composite
    {
        private int _LoopCounter;

        public override CompositeType CompositeType { get { return AI.CompositeType.Loop; } }

        /// <summary> number of loop (-1 for infinit)</summary>
        public int LoopCount { get; set; }

        public LoopSelector(string name)
            : base(name)
        {
            _LoopCounter = -1;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {

            BehaviorResult result = BehaviorResult.Failure;
            if (RunningChildIndex < 0) RunningChildIndex = 0;
            for (int i = RunningChildIndex; i < Count; i++)
            {
                Behavior node = this[i];
                result = node.Trace(state);
                if (result == BehaviorResult.Running)
                {
                    RunningChildIndex = i;
                    break;
                }
                else
                    RunningChildIndex = -1;
                if (result == BehaviorResult.Failure)
                    break;
            }
            if (result == BehaviorResult.Success) // cause loop next update and begin from child 0
            {
                _LoopCounter++;
                if (LoopCount != -1 && LoopCount >= _LoopCounter)
                {
                    result = BehaviorResult.Success;
                    _LoopCounter = 0;
                }
                else
                    result = BehaviorResult.Running;
                RunningChildIndex = 0;
            }
            else if (result == BehaviorResult.Failure)
                _LoopCounter = 0;
            return result;
        }
    }
}
