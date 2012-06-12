using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.AI
{
    #region Action
    /// <summary>
    /// Defines an action node in behavior tree
    /// </summary>
    public class Action : Behavior
    {
        /// <summary> Type of node </summary>
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.Action; } }

        /// <summary>
        /// Create an instance of action
        /// </summary>
        public Action()
            : base("NewAction")
        {
        }
    }
    #endregion
}
