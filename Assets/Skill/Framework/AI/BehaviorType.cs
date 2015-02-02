using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    #region BehaviorType
    /// <summary>
    /// Defines types of behavior nodes in BehaviorTree
    /// </summary>
    public enum BehaviorType
    {
        /// <summary>
        /// Implement an actors or game world status changes.(leaf node) 
        /// </summary>
        Action,
        /// <summary>
        /// Check that certain actor or game world statuss hold true.(leaf node)
        /// </summary>
        Condition,                
        /// <summary>
        /// Typically have only one child and are used to enforce a certain return status 
        /// or to implement timers to restrict how often the child will run in a given amount of time
        /// or how often it can be executed without a pause.(none leaf node)
        /// </summary>
        Decorator,
        /// <summary>
        /// Contains childeren and execute them in specific order.(none leaf node)
        /// </summary>
        Composite,
        /// <summary>
        /// Change state of behavior tree
        /// </summary>
        ChangeState
    }
    #endregion
}
