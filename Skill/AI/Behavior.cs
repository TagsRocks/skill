using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// Defines base class for all behavior nodes in BehaviorTree
    /// </summary>
    public abstract class Behavior
    {
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
            //int registerIndex = 
            state.RegisterForExecution(this); // register in execution sequence
            try
            {
                Result = Behave(state);// let subclass behave
            }
            catch (Exception e)
            {
                state.Exception = e;// store exception
                Result = BehaviorResult.Failure; // set result to failure
            }

            if (Result != BehaviorResult.Running)
                state.UnRegisterForExecution(this);
            return Result;
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
        /// <param name="resetChildren">Reset children too</param>
        internal virtual void ResetBehavior(bool resetChildren = false)
        {
            Result = BehaviorResult.Failure;
        }

    }
}
