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

        private Behavior[] _ExecutionSequence = new Behavior[200];// 200 is maximum node trace in tree (i hope).
        private int _CurrnetExecutionIndex = -1;

        private Behavior[] _Stack = new Behavior[200];// 200 is maximum node trace in tree (i hope).
        private int _StackTop = -1;


        /// <summary>
        /// BehaviorTree call this method at begin of each update
        /// </summary>
        internal void Begin()
        {
            _CurrnetExecutionIndex = -1;
            _StackTop = -1;
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
            _ExecutionSequence[_CurrnetExecutionIndex] = behavior;
            _StackTop++;

            // this means : the execution sequence is different from previous execution sequence
            if (_Stack[_StackTop] != null && behavior != _Stack[_StackTop])
            {
                CallResetForSequence(_StackTop);// so call fail method for rest of previous sequence                
            }
            _Stack[_StackTop] = behavior; // register behavior in current sequece
            return _StackTop;

        }

        internal void UnRegisterForExecution(Behavior behavior)
        {
            if (_Stack[_StackTop] == behavior)
            {
                _Stack[_StackTop] = null;
                _StackTop--;
            }
            else
                throw new InvalidProgramException("Invalid Behavior To UnRegister");
        }

        /// <summary>
        /// call fail method for rest of execution sequence
        /// </summary>
        private void CallResetForSequence(int startIndex)
        {
            for (int i = startIndex; i < _Stack.Length; i++)
            {
                if (_Stack[i] != null)
                {
                    _Stack[i].ResetBehavior(this);
                    _Stack[i] = null;
                }
                else
                    break;
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
