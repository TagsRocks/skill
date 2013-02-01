using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.AI
{
    /// <summary>
    /// Represent State of behaviorTree and send data between nodes
    /// </summary>
    public class BehaviorTreeState
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
        /// Update id (counter)
        /// </summary>
        public uint UpdateId { get; private set; }

        private Behavior _Root;

        /// <summary>
        /// Create a BehaviorState
        /// </summary>
        /// <param name="root">root of BehaviorTree</param>
        public BehaviorTreeState(Behavior root)
        {
            this._Root = root;
            this.Exception = null;
            this.RunningActions = new RunningActionCollection();
        }

        private Behavior[] _ExecutionSequence = new Behavior[MaxSequenceLength];// 200 is maximum node trace in tree (i hope).
        private int _CurrnetExecutionIndex = -1;        


        /// <summary>
        /// BehaviorTree call this method at begin of each update (internal use)
        /// </summary>
        public void Begin()
        {
            _CurrnetExecutionIndex = -1;
            UpdateId++;
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
        /// <param name="copyToClipboard">Copy all text to clipboard</param>
        public void LogExecutionSequenceTree()
        {
            int sqIndex = 0;
            LogExecutionSequenceTree(_Root, ref sqIndex, 0);
        }

        private void LogExecutionSequenceTree(Behavior b, ref int sqIndex, int tabCount)
        {
            if (b == null || _ExecutionSequence[sqIndex] == null || sqIndex >= _CurrnetExecutionIndex) return;
            if (b == _ExecutionSequence[sqIndex])
            {
                string log = string.Format("{0} - {1}{2} : {3}", sqIndex, GetTabSpace(tabCount), b.ToString(), b.Result);
                UnityEngine.Debug.Log(log);
                sqIndex++;
            }
            if (b.Type == BehaviorType.Decorator)
            {
                LogExecutionSequenceTree(((Decorator)b).Child.Behavior, ref sqIndex, tabCount + 1);
            }
            else if (b.Type == BehaviorType.Composite)
            {
                foreach (var child in ((Composite)b))
                {
                    LogExecutionSequenceTree(child.Behavior, ref sqIndex, tabCount + 1);
                }
            }
        }
    }
}
