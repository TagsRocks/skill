using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    #region DecoratorHandler
    /// <summary>
    /// Represents the method that will handle execution of Decorator by user
    /// </summary>
    /// <param name="tree">BehaviorTree</param>
    /// <param name="parameters">Parameters for condition</param>
    /// <returns>true for success, false for failure</returns>
    public delegate bool DecoratorHandler(BehaviorTree tree, BehaviorParameterCollection parameters);
    #endregion

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
    /// Typically have only one child and are used to enforce a certain return state 
    /// or to implement timers to restrict how often the child will run in a given amount of time
    /// or how often it can be executed without a pause.(none leaf node)
    /// </summary>
    public class Decorator : Behavior
    {
        private bool _IsChildRunning;// whether child state is running
        private DecoratorHandler _Handler = null;// handler

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
        /// set to true if only BehaviorResult.Running is important for you (default is true)
        /// </summary>
        public bool NeverFail { get; set; }

        /// <summary>
        /// Create an instance of Decorator
        /// </summary>
        /// <param name="name">Name of behavior</param>
        /// <param name="handler">user provided function to handle execution of Decorator</param>
        public Decorator(string name, DecoratorHandler handler)
            : base(name, BehaviorType.Decorator)
        {
            this._Handler = handler;
            this.NeverFail = true;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">State od BehaviorTree</param>
        /// <returns></returns>
        protected override BehaviorResult Behave(BehaviorState state)
        {
            BehaviorResult result = BehaviorResult.Failure;
            if (Child != null)
            {
                if (_Handler != null)
                {
                    state.Parameters = Child.Parameters;
                    if (_IsChildRunning)// continue execution of child
                        result = Child.Behavior.Trace(state);
                    else if (_Handler(state.BehaviorTree, state.Parameters))
                        result = Child.Behavior.Trace(state);
                }
                else
                    result = Child.Behavior.Trace(state);
            }
            if (result == BehaviorResult.Running)
                _IsChildRunning = true;
            else
                _IsChildRunning = false;

            if (NeverFail && result == BehaviorResult.Failure)
                result = BehaviorResult.Success;
            return result;
        }

        /// <summary>
        /// Reset behavior
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <param name="resetChildren">Reset children too</param>
        public override void ResetBehavior(BehaviorState state, bool resetChildren = false)
        {
            base.ResetBehavior(state);
            if (resetChildren)
            {
                if (Child != null)
                {
                    Child.Behavior.ResetBehavior(state, resetChildren);
                }
            }
        }
    }
    #endregion
}
