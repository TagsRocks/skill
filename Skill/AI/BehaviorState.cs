using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// Represent State of behaviorTree and send data between nodes
    /// </summary>
    public class BehaviorState
    {

        /// <summary>
        /// Used internally to send valid parameters to child behaviors
        /// </summary>
        internal BehaviorParameterCollection Parameters { get; set; }

        /// <summary>
        /// exception occurs in evaluation of tree, otherwise null
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// current running action.
        /// </summary>
        /// <remarks>
        /// action is allways leaf node. after execution of each action, if result is running hold it's reference
        /// </remarks>
        public Action RunningAction { get; internal set; }

        /// <summary>
        /// The BehaviorTree
        /// </summary>
        public BehaviorTree BehaviorTree { get; private set; }

        /// <summary>
        /// The execution sequence after last update call.
        /// </summary>
        /// <remarks>
        /// This Property is for additional info and debug, do not modify this manually.
        /// </remarks>
        public Behavior[] ExecutionSequence { get { return _ExecutionSequence; } }


        public BehaviorState(BehaviorTree behaviorTree)
        {
            this.BehaviorTree = behaviorTree;
            this.Exception = null;
            this.RunningAction = null;
        }

        private Behavior[] _ExecutionSequence = new Behavior[100];// 100 is maximum level of tree (i hope).
        private int _CurrnetExecutionIndex = -1;


        /// <summary>
        /// BehaviorTree call this method at begin of each update
        /// </summary>
        internal void Begin()
        {
            _CurrnetExecutionIndex = -1;
        }

        /// <summary>
        /// each behavior before execution call this method to register in execution sequence.
        /// </summary>
        /// <param name="behavior">Behavior to register</param>
        /// <returns>Return registerd index</returns>
        /// <remarks>
        /// we need to keep last execution sequenece to aviod some mistakes in tree
        /// for example : at LimitAccessDecoratot execution, it must get access to key
        /// if the result be Running then it hold the key until next update (at least)
        /// if in next update a branch before that be executed we lost the key and never unlock it        
        /// </remarks>
        internal int RegisterForExecution(Behavior behavior)
        {
            _CurrnetExecutionIndex++;// move to next place

            // this means : the execution sequence is different from previous execution sequence
            if (_ExecutionSequence[_CurrnetExecutionIndex] != null && behavior != _ExecutionSequence[_CurrnetExecutionIndex])
                CallFailForSequence();// so call fail method for rest of previous sequence
            _ExecutionSequence[_CurrnetExecutionIndex] = behavior; // register behavior in current sequece
            return _CurrnetExecutionIndex;

        }


        /// <summary>
        /// if result of execution be Failur call this at end of Trace method
        /// </summary>
        /// <param name="registerIndex">index of behavior in sequence. (got it by RegisterForExecution method)</param>
        internal void UnregisterForExecution(int registerIndex)
        {
            for (int i = registerIndex; i < _ExecutionSequence.Length; i++)
            {
                if (_ExecutionSequence[i] != null)
                {
                    _ExecutionSequence[i] = null;
                }
                else
                    break;
            }
            _CurrnetExecutionIndex = registerIndex - 1;
        }

        /// <summary>
        /// call fail method for rest of execution sequence
        /// </summary>
        private void CallFailForSequence()
        {
            for (int i = _CurrnetExecutionIndex; i < _ExecutionSequence.Length; i++)
            {
                if (_ExecutionSequence[i] != null)
                {
                    _ExecutionSequence[i].Reset(this);
                    _ExecutionSequence[i] = null;
                }
                else
                    break;
            }
        }
    }
}
