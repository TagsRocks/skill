using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    #region LoopSelector
    /// <summary>
    /// Loops are like sequences but they loop around when reaching their last child during their traversal.
    /// if reach last child and it returns Success. this node returns Running and continue from first child at next update.
    /// we do this to avoid fall into infinit loop
    /// </summary>
    public class LoopSelector : Composite
    {
        private int _LoopCounter;// execution number of current node

        /// <summary>
        /// CompositeType
        /// </summary>
        public override CompositeType CompositeType { get { return AI.CompositeType.Loop; } }

        /// <summary> number of loop (-1 for infinit)</summary>
        public int LoopCount { get; set; }

        /// <summary>
        /// Create an instance of LoopSelector
        /// </summary>
        /// <param name="name">Name of Behavior node</param>
        public LoopSelector(string name)
            : base(name)
        {
            _LoopCounter = -1;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="status">Status of BehaviorTree</param>
        /// <returns>esult</returns>
        protected override BehaviorResult Behave(BehaviorTreeStatus status)
        {

            BehaviorResult result = BehaviorResult.Failure;
            if (RunningChildIndex < 0) RunningChildIndex = 0;
            for (int i = RunningChildIndex; i < ChildCount; i++)
            {
                if (status.IsInterrupted) break;
                BehaviorContainer node = this[i];
                status.Parameters = node.Parameters;
                node.Execute(status);
                result = node.Result;
                if (result == BehaviorResult.Running)
                {
                    RunningChildIndex = i;
                    break;
                }
                else
                    RunningChildIndex = -1;
                if (result == BehaviorResult.Failure)
                    break;
            }
            if (result == BehaviorResult.Success) // cause loop next update and begin from child 0
            {
                _LoopCounter++;
                if (LoopCount > 0 && _LoopCounter >= LoopCount)
                {
                    result = BehaviorResult.Success;
                    _LoopCounter = 0;
                }
                else
                    result = BehaviorResult.Running;
                RunningChildIndex = 0;
            }
            else if (result == BehaviorResult.Failure)
                _LoopCounter = 0;
            return result;
        }

        /// <summary>
        /// Reset behavior
        /// </summary>        
        /// <param name="status">Status of BehaviorTree</param>                
        public override void ResetBehavior(BehaviorTreeStatus status)
        {
            if (Result == BehaviorResult.Running && status.UpdateId != LastUpdateId)
            {
                _LoopCounter = 0;
            }
            base.ResetBehavior(status);
        }
    }
    #endregion
}
