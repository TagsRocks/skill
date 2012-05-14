using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{

    #region BehaviorEventHandler
    /// <summary>
    /// Represents the method that will handle behavior events.
    /// </summary>
    /// <param name="sender">Sender behavior</param>
    /// <param name="result">Result of behavior after execution</param>
    /// <param name="tree">BehaviorTree</param>
    public delegate void BehaviorEventHandler(Behavior sender, BehaviorResult result, BehaviorTree tree);
    #endregion


    /// <summary>
    /// Defines base class for all behavior nodes in BehaviorTree
    /// </summary>
    public abstract class Behavior
    {
        /// <summary>
        /// Occurs when execution result of behavior is BehaviorResult.Success
        /// </summary>
        public event BehaviorEventHandler Success;
        /// <summary>
        /// Occurs when execution result of behavior is BehaviorResult.Failure
        /// </summary>
        public event BehaviorEventHandler Failure;
        /// <summary>
        /// Occurs when execution result of behavior is BehaviorResult.Running
        /// </summary>
        public event BehaviorEventHandler Running;

        /// <summary>
        /// Name of Behavior
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Type of Behavior
        /// </summary>
        public BehaviorType Type { get; private set; }
        /// <summary>
        /// Last ecexution result of behavior
        /// </summary>
        public BehaviorResult Result { get; private set; }
        /// <summary>
        /// Wheight or chance of behavior when is behavior is child of a RandomSelector
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Create an instance of Behavior
        /// </summary>
        /// <param name="name">Name of Behavior</param>
        /// <param name="behaviorType">Type of behavior (specified by subclass)</param>
        protected Behavior(string name, BehaviorType behaviorType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid Behavior name");
            this.Name = name;
            this.Type = behaviorType;
            this.Weight = 1;
        }

        /// <summary>
        /// handle execution of behavior and call appropriate events
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <returns>Result of execution</returns>
        internal BehaviorResult Trace(BehaviorState state)
        {
            int registerIndex = state.RegisterForExecution(this); // register in execution sequence
            try
            {
                Result = Behave(state);// let subclass behave
            }
            catch (Exception e)
            {
                state.Exception = e;// store exception
                Result = BehaviorResult.Failure; // set result to failure
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

        /// <summary>
        /// On Success
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        protected virtual void OnSuccess(BehaviorState state)
        {
            if (Success != null) Success(this, Result, state.BehaviorTree);
        }
        /// <summary>
        /// On Failure
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        protected virtual void OnFailure(BehaviorState state)
        {
            if (Failure != null) Failure(this, Result, state.BehaviorTree);
        }
        /// <summary>
        /// On Running
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        protected virtual void OnRunning(BehaviorState state)
        {
            if (Running != null) Running(this, Result, state.BehaviorTree);
        }

        /// <summary>
        /// Let subclass implement behaveior
        /// </summary>
        /// <param name="state">State of BehaviorTre</param>
        /// <returns>Result of behavior</returns>
        protected abstract BehaviorResult Behave(BehaviorState state);

        /// <summary>
        /// Represent Behavior as string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString() { return Name; }

        
        /// <summary>
        /// Reset behavior. For internal use. when a branch with higher priority executed, let nodes in previous branch reset
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <param name="resetChildren">Reset children too</param>
        public virtual void Reset(BehaviorState state, bool resetChildren = false)
        {
            Result = BehaviorResult.Failure;
        }

    }
}
