using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    /// <summary>
    /// Represents the method to handle execution of condition by user
    /// </summary>        
    /// <param name="sender"> Sender </param>
    /// <param name="parameters">Parameters for condition</param>
    /// <returns>true for success, false for failure</returns>
    public delegate bool ConditionHandler(object sender, BehaviorParameterCollection parameters);

    /// <summary>
    /// Check that certain actor or game world statuss hold true.(leaf node)
    /// </summary>
    public class Condition : Behavior
    {
        private ConditionHandler _Handler = null;// handlre

        /// <summary>
        /// Reverse condition. (maybe remove latter)
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// Is loaf of tree? 
        /// </summary>
        public override bool IsLeaf { get { return true; } }

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
        /// <param name="status">Status of BehaviorTree</param>
        /// <returns></returns>
        protected override BehaviorResult Behave(BehaviorTreeStatus status)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (_Handler != null)
            {
                bool b = _Handler(this,status.Parameters);
                if (Reverse)
                    result = b ? BehaviorResult.Failure : BehaviorResult.Success;
                else
                    result = b ? BehaviorResult.Success : BehaviorResult.Failure;
            }
            return result;
        }
    }
}
