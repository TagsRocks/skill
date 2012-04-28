using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Editor.AI
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

    #region ConditionViewModel
    public class ConditionViewModel : BehaviorViewModel
    {
        public override string ImageName { get { return Images.Condition; } }

        public ConditionViewModel(BehaviorViewModel parent, Skill.Editor.AI.Condition condition)
            : base(parent, condition)
        {

        }
    } 
    #endregion
}
