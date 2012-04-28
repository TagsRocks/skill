using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{

    public delegate bool ConditionHandler(Skill.Controllers.IController controller);

    public class Condition : Behavior
    {
        private ConditionHandler _Handler = null;

        public bool Reverse { get; set; }

        public Condition(string name, ConditionHandler handler)
            : base(name, BehaviorType.Condition)
        {
            this._Handler = handler;
            Reverse = false;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (_Handler != null)
            {
                bool b = _Handler(state.BehaviorTree.Controller);
                if (Reverse)
                    result = b ? BehaviorResult.Failure : BehaviorResult.Success;
                else
                    result = b ? BehaviorResult.Success : BehaviorResult.Failure;
            }
            return result;
        }
    }
}
