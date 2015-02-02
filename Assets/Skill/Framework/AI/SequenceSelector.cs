using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    #region SequenceSelector
    /// <summary>
    /// run one child to finish after the other. If one or multiple fail the whole sequence fails, too.
    /// Without a reset or without finishing the last child node a sequence stores the last running child to immediately return to it on the next update.
    /// </summary>
    public class SequenceSelector : Composite
    {
        /// <summary>
        /// CompositeType
        /// </summary>
        public override CompositeType CompositeType { get { return AI.CompositeType.Sequence; } }

        /// <summary>
        /// Create an instance of SequenceSelector
        /// </summary>
        /// <param name="name">Name of Behavior node</param>
        public SequenceSelector(string name)
            : base(name)
        {

        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="status">Status od BehaviorTree</param>
        /// <returns>Result</returns>
        protected override BehaviorResult Behave(BehaviorTreeStatus status)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (RunningChildIndex < 0) RunningChildIndex = 0;
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
    #endregion
}
