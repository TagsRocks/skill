using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.DataModels.AI
{
    #region Condition
    /// <summary>
    /// Defines condition in behavior tree
    /// </summary>
    public class Condition : Behavior
    {
        public override BehaviorType BehaviorType { get { return AI.BehaviorType.Condition; } }

        public Condition()
            : base("NewCondition")
        {

        }
    }
    #endregion
}
