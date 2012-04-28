using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public enum BehaviorType
    {
        Composite,
        Condition,
        Decorator,
        Action
    }

    public delegate BehaviorResult BehaviorHandler(Skill.Controllers.IController controller);
    public delegate void BehaviorEventHandler(Skill.Controllers.IController controller, EventArgs e);

    public abstract class Behavior
    {
        public event BehaviorEventHandler Success;
        public event BehaviorEventHandler Failure;
        public event BehaviorEventHandler Running;
        
        public string Name { get; private set; }
        public BehaviorType Type { get; private set; }        
        public BehaviorResult Result { get; private set; }
        public float Weight { get; set; }

        protected Behavior(string name, BehaviorType behaviorType)
        {            
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid Behavior name");
            this.Name = name;
            this.Type = behaviorType;
            this.Weight = 1;
        }

        internal BehaviorResult Trace(BehaviorState state)
        {
            try
            {
                Result = Behave(state);
            }
            catch (Exception e)
            {
                state.Exception = e;
                Result = BehaviorResult.Failure;
            }
            switch (Result)
            {
                case BehaviorResult.Failure:
                    OnFailure(state);
                    break;
                case BehaviorResult.Success:
                    OnSuccess(state);
                    break;
                case BehaviorResult.Running:
                    OnRunning(state);
                    break;
                default:
                    break;
            }
            return Result;
        }

        protected virtual void OnSuccess(BehaviorState state)
        {
            if (Success != null) Success(state.BehaviorTree.Controller, EventArgs.Empty);
        }
        protected virtual void OnFailure(BehaviorState state)
        {
            if (Failure != null) Failure(state.BehaviorTree.Controller, EventArgs.Empty);
        }
        protected virtual void OnRunning(BehaviorState state)
        {
            if (Running != null) Running(state.BehaviorTree.Controller, EventArgs.Empty);
        }

        protected abstract BehaviorResult Behave(BehaviorState state);        
        public override string ToString() { return Name; }

    }
}
