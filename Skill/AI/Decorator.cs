using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public delegate bool DecoratorHandler(Skill.Controllers.IController controller);

    public class Decorator : Behavior
    {
        private bool _IsChildRunning;
        private DecoratorHandler _Handler = null;
        public Behavior Child { get; set; }

        public bool SuccessOnFailHandler { get; set; }

        public Decorator(string name, DecoratorHandler handler)
            : base(name, BehaviorType.Decorator)
        {
            this._Handler = handler;
            this.SuccessOnFailHandler = false;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {
            BehaviorResult result = SuccessOnFailHandler ? BehaviorResult.Success : BehaviorResult.Failure;
            if (Child != null)
            {
                if (_Handler != null)
                {
                    if (_IsChildRunning)
                        result = Child.Trace(state);
                    else if (_Handler(state.BehaviorTree.Controller))
                        result = Child.Trace(state);
                }
                else
                    result = Child.Trace(state);
            }
            if (result == BehaviorResult.Running)
                _IsChildRunning = true;
            else
                _IsChildRunning = false;
            return result;
        }
    }
}
