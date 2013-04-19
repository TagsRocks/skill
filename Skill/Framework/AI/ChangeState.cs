using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    public class ChangeState : Behavior
    {

        /// <summary>
        /// State
        /// </summary>
        public string DestinationState { get; private set; }

        /// <summary>
        /// Create an instance of Action
        /// </summary>
        /// <param name="name">Name of action</param>
        /// <param name="destinationState">Name of destination state</param>
        public ChangeState(string name, string destinationState)
            : base(name, BehaviorType.ChangeState)
        {
            this.DestinationState = destinationState;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="status">Status of BehaviorTree</param>
        /// <returns>Result of action</returns>
        protected override BehaviorResult Behave(BehaviorTreeStatus status)
        {
            if (status.Tree != null)
                status.Tree.ChangeState(DestinationState);
            return BehaviorResult.Success;
        }
    }
}
