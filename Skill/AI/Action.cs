using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public class Action : Behavior
    {
        private BehaviorHandler _Handler = null;

        public Action(string name, BehaviorHandler handler)
            : base(name, BehaviorType.Action)
        {
            this._Handler = handler;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {            
            BehaviorResult result = BehaviorResult.Failure;
            if (_Handler != null) result = _Handler(state.BehaviorTree.Controller);

            if (result == BehaviorResult.Running)
                state.RunningAction = this;
            else
                state.RunningAction = null;
            return result;
        }        
    }
}
