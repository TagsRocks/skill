using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// Represents the method that will handle execution of condition by user
    /// </summary>
    /// <param name="tree">BehaviorTree</param>
    /// <param name="parameters">Parameters for condition</param>
    /// <returns>true for success, false for failure</returns>
    public delegate bool ConditionHandler(BehaviorTree tree, BehaviorParameterCollection parameters);

    /// <summary>
    /// Check that certain actor or game world states hold true.(leaf node)
    /// </summary>
    public class Condition : Behavior
    {
        private ConditionHandler _Handler = null;// handlre

        /// <summary>
        /// Reverse condition. (maybe remove latter)
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// Create an instance of Condition 
        /// </summary>
        /// <param name="name">Name of Behavior</param>
        /// <param name="handler">function to handle execution of action</param>
        public Condition(string name, ConditionHandler handler)
            : base(name, BehaviorType.Condition)
        {
            this._Handler = handler;
            Reverse = false;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <returns></returns>
        protected override BehaviorResult Behave(BehaviorState state)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (_Handler != null)
            {
                bool b = _Handler(state.BehaviorTree, state.Parameters);
                if (Reverse)
                    result = b ? BehaviorResult.Failure : BehaviorResult.Success;
                else
                    result = b ? BehaviorResult.Success : BehaviorResult.Failure;
            }
            return result;
        }
    }
}
