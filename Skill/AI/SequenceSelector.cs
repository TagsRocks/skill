using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public class SequenceSelector : Composite
    {

        public override CompositeType CompositeType { get { return AI.CompositeType.Sequence; } }

        public SequenceSelector(string name)
            : base(name)
        {

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
                {
                    break;
                }
            }
            if (result != BehaviorResult.Running)
                RunningChildIndex = -1;
            return result;
        }
    }
}
