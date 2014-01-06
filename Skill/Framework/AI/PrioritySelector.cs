using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    #region PrioritySelector
    /// <summary>
    /// Defines behavior of PrioritySelector
    /// </summary>
    public enum PriorityType
    {
        /// <summary>
        /// Allways  begin by high priority node
        /// </summary>
        HighestPriority,
        /// <summary>
        /// Continue by last running node
        /// </summary>
        RunningNode,

    }

    /// <summary>
    /// On each traversal priority selectors check which child to run in priority order until the first one succeeds or returns that it is running.
    /// One option is to call the last still running node again during the next behavior tree update. The other option is to always restart traversal
    /// from the highest priority child and implicitly cancel the last running child behavior if it isn’t chosen immediately again.
    /// </summary>
    public class PrioritySelector : Composite
    {
        /// <summary>
        /// Behavior of PrioritySelector (default : HighestPriority)
        /// </summary>
        public PriorityType Priority { get; set; }

        /// <summary>
        /// CompositeType
        /// </summary>
        public override CompositeType CompositeType { get { return AI.CompositeType.Priority; } }

        /// <summary>
        /// Create an instance of PrioritySelector
        /// </summary>
        /// <param name="name">Name of Behavior</param>
        public PrioritySelector(string name)
            : base(name)
        {
            Priority = PriorityType.HighestPriority;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="status">Status of BehaviorTree</param>
        /// <returns>Result</returns>
        protected override BehaviorResult Behave(BehaviorTreeStatus status)
        {
            if (Priority == PriorityType.HighestPriority || RunningChildIndex < 0)
                RunningChildIndex = 0;
            BehaviorResult result = BehaviorResult.Failure;
            for (int i = RunningChildIndex; i < ChildCount; i++)
            {
                if (status.IsInterrupted) break;
                BehaviorContainer node = this[i];
                status.Parameters = node.Parameters;
                result = node.Behavior.Execute(status);
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
    #endregion    
}
