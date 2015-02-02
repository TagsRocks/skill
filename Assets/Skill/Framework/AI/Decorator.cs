using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{    
    #region DecoratorType
    /// <summary>
    /// Defines type of  Decorator
    /// </summary>
    public enum DecoratorType
    {
        /// <summary>
        /// Access of child specified by user function
        /// </summary>
        Default,
        /// <summary>
        /// Limit execution of child node on access key.
        /// </summary>
        AccessLimit
    }
    #endregion

    #region Decorator
    /// <summary>
    /// Typically have only one child and are used to enforce a certain return status 
    /// or to implement timers to restrict how often the child will run in a given amount of time
    /// or how often it can be executed without a pause.(none leaf node)
    /// </summary>
    public class Decorator : Behavior
    {        
        private ConditionHandler _Handler = null;// handler

        /// <summary>
        /// Single child node
        /// </summary>
        public BehaviorContainer Child { get; private set; }

        /// <summary>
        /// DecoratorType
        /// </summary>
        public virtual DecoratorType DecoratorType { get { return AI.DecoratorType.Default; } }

        /// <summary>
        /// Set child of decorator
        /// </summary>
        /// <param name="child">Child behavior</param>
        /// <param name="parameters">Optional parameters for child behavior at this position of tree</param>
        public virtual void SetChild(Behavior child, BehaviorParameterCollection parameters = null)
        {
            if (child != null)
            {
                Child = new BehaviorContainer(child, parameters);
            }
        }

        /// <summary>
        /// set to true if only BehaviorResult.Running is important for you (default is false)
        /// </summary>
        public bool NeverFail { get; set; }

        /// <summary>
        /// Create an instance of Decorator
        /// </summary>
        /// <param name="name">Name of behavior</param>
        /// <param name="handler">user provided function to handle execution of Decorator</param>
        public Decorator(string name, ConditionHandler handler)
            : base(name, BehaviorType.Decorator)
        {
            this._Handler = handler;
            this.NeverFail = false;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="status">Status od BehaviorTree</param>
        /// <returns></returns>
        protected override BehaviorResult Behave(BehaviorTreeStatus status)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (Child != null)
            {
                if (_Handler != null)
                {
                    if (Result == BehaviorResult.Running)// continue execution of child
                    {
                        result = TraceChild(status);
                    }
                    else if (_Handler(this,status.Parameters))
                    {
                        result = TraceChild(status);
                    }
                }
                else
                {
                    result = TraceChild(status);
                }
            }

            if (NeverFail && result == BehaviorResult.Failure)
                result = BehaviorResult.Success;
            return result;
        }

        /// <summary>
        /// Trace Child - subclasses should implement this method
        /// </summary>
        /// <param name="status">Status od BehaviorTree</param>
        /// <returns></returns>
        protected virtual BehaviorResult TraceChild(BehaviorTreeStatus status)
        {
            if (Child != null)
            {
                status.Parameters = Child.Parameters;
                return Child.Behavior.Execute(status);
            }
            return BehaviorResult.Failure;
        }


        /// <summary>
        /// Reset behavior
        /// </summary>        
        /// <param name="status">Status of BehaviorTree</param>                
        public override void ResetBehavior(BehaviorTreeStatus status)
        {
            if (Result == BehaviorResult.Running)
            {
                if (Child != null)
                {
                    Child.Behavior.ResetBehavior(status);
                }
            }
            base.ResetBehavior(status);
        }
    }
    #endregion
}
