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
        /// Maximum lenght of visited behaviors in each BehaviorTree update
        /// if your BehaviorTrees is very large increas this value
        /// </summary>
        public static int MaxSequenceLength = 200;

        /// <summary>
        /// Used internally to send valid parameters to child behaviors
        /// </summary>
        internal BehaviorParameterCollection Parameters { get; set; }

        /// <summary>
        /// exception occurs in evaluation of tree, otherwise null
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// current running actions.
        /// </summary>
        /// <remarks>
        /// action is allways leaf node. after execution of each action, if result is running hold it's reference
        /// </remarks>
        public RunningActionCollection RunningActions { get; private set; }

        /// <summary>
        /// The execution sequence after last update call.
        /// </summary>
        /// <remarks>
        /// This Property is for additional info and debug, do not modify this manually.
        /// </remarks>
        public Behavior[] ExecutionSequence { get { return _ExecutionSequence; } }

        /// <summary> Number of valid Behaviors in ExecutionSequence</summary>
        public int SequenceCount { get { return _CurrnetExecutionIndex + 1; } }

        /// <summary>
        /// Create a BehaviorState
        /// </summary>
        public BehaviorState()
        {
            this.Exception = null;
            this.RunningActions = new RunningActionCollection();
            _RunningStack = new RunningStack(this, MaxSequenceLength);
        }

        private Behavior[] _ExecutionSequence = new Behavior[MaxSequenceLength];// 200 is maximum node trace in tree (i hope).
        private int _CurrnetExecutionIndex = -1;
        private RunningStack _RunningStack;


        /// <summary>
        /// BehaviorTree call this method at begin of each update (internal use)
        /// </summary>
        public void Begin()
        {
            _CurrnetExecutionIndex = -1;
            _RunningStack.Swap();
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
        internal void RegisterForExecution(Behavior behavior)
        {
            _CurrnetExecutionIndex++;// move to next place  
            if (_CurrnetExecutionIndex >= _ExecutionSequence.Length)
                throw new IndexOutOfRangeException("ExecutionSequence buffer is low. to avoid this error set higher value to 'BehaviorState.MaxSequenceLength'.");
            _ExecutionSequence[_CurrnetExecutionIndex] = behavior;

            if (behavior.Type == BehaviorType.Action)
            {
                if (!RunningActions.Contains((Action)behavior))
                {
                    _RunningStack.ResetPreviousStack();
                }
            }
            _RunningStack.Push(behavior);
        }

        /// <summary>
        /// Unregister behavior from execution sequence.
        /// </summary>
        /// <param name="behavior">Behavior to unregister</param>
        internal void UnRegisterForExecution(Behavior behavior)
        {
            while (_RunningStack.Count > 0)
            {
                Behavior top = _RunningStack.Pop();
                if (top == behavior)
                    break;
                else
                {
                    if (top.Type == BehaviorType.Action && top.Result == BehaviorResult.Running)
                    {
                        if (!_RunningStack.IsInPreviousStack(top))
                        {
                            // this action is executed in only current update and is not registerd in running stack
                            // the action is executed and still running but we miss to reset it next update
                            // so we reset it here
                            top.ResetBehavior();
                        }
                    }
                }
            }
        }

        private bool IsChildOf(Behavior parent, Behavior child)
        {
            if (parent.Type == BehaviorType.Composite)
            {
                if (((Composite)parent).Count(c => c.Behavior == child) > 0)
                    return true;
            }
            else if (parent.Type == BehaviorType.Decorator)
            {
                if (((Decorator)parent).Child.Behavior == child)
                    return true;
            }
            return false;
        }

        private string GetTabSpace(int tabCount)
        {
            string s = string.Empty;
            for (int i = 0; i < tabCount; i++)
            {
                s += "\t";
            }
            return s;
        }

        /// <summary>
        /// Write ExecutionSequence to UnityEngin.Debug.Log
        /// </summary>
        public void LogExecutionSequence()
        {
            for (int i = 0; i <= _CurrnetExecutionIndex; i++)
            {
                if (_ExecutionSequence[i] == null) break;
                UnityEngine.Debug.Log(string.Format("{0} - {1} : {2}", i + 1, _ExecutionSequence[i].ToString(), _ExecutionSequence[i].Result));
            }
        }


        /// <summary>
        /// Write ExecutionSequence to UnityEngin.Debug.Log in tree style
        /// </summary>
        public void LogExecutionSequenceTree()
        {
            int tabCounter = 0;
            Stack<Behavior> parentStack = new Stack<Behavior>();
            for (int i = 0; i <= _CurrnetExecutionIndex; i++)
            {
                if (_ExecutionSequence[i] == null) break;
                while (parentStack.Count > 0 && !IsChildOf(parentStack.Peek(), _ExecutionSequence[i]))
                {
                    tabCounter = Math.Max(-1, tabCounter - 1);
                    parentStack.Pop();
                }

                tabCounter++;
                UnityEngine.Debug.Log(string.Format("{0} - {1}{2} : {3}", i++, GetTabSpace(tabCounter), _ExecutionSequence[i].ToString(), _ExecutionSequence[i].Result));
                parentStack.Push(_ExecutionSequence[i]);
            }
        }
    }
}
