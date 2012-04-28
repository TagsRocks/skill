using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public enum PriorityType
    {
        RunningNode,
        HighestPriority,        
    }

    public class PrioritySelector : Composite
    {        
        public PriorityType Priority { get; set; }
        public override CompositeType CompositeType { get { return AI.CompositeType.Priority; } }        

        public PrioritySelector(string name)
            : base(name)
        {
            Priority = PriorityType.RunningNode;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {
            if (Priority == PriorityType.HighestPriority || RunningChildIndex < 0)
                RunningChildIndex = 0;
            BehaviorResult result = BehaviorResult.Failure;
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
                if (result == BehaviorResult.Success)
                {                    
                    break;
                }
            }
            return result;
        }
    }
}
