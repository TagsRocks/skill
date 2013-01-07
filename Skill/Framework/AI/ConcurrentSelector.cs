using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Framework.AI
{

    #region FailurePolicy
    /// <summary>
    /// Enumerates the options for when a ConcurrentSelector is considered to have failed.
    /// </summary>
    /// <remarks> If FailOnOne and SuceedOnOne are both active and are both trigerred in the same time step, failure will take precedence. </remarks>
    public enum FailurePolicy
    {
        /// <summary>  indicates that the node will return failure as soon as one of its children fails.</summary>
        FailOnOne,
        /// <summary>  indicates that all of the node's children must fail before it returns failure.</summary>
        FailOnAll
    }
    #endregion

    #region SuccessPolicy
    /// <summary>
    /// Enumerates the options for when a ConcurrentSelector is considered to have succeeded.
    /// </summary>    
    public enum SuccessPolicy
    {
        /// <summary>
        /// indicates that the node will return success as soon as one of its children succeeds.
        /// </summary>
        SucceedOnOne,
        /// <summary>
        /// indicates that all of the node's children must succeed before it returns success.
        /// </summary>
        SucceedOnAll
    }
    #endregion

    #region ConcurrentSelector
    /// <summary>
    /// visit all of their children during each traversal.
    /// A pre-specified number of children needs to fail to make the concurrent node fail, too.
    /// Instead of running its child nodes truly in parallel to each other there might be a specific traversal order which can be exploited when adding conditions
    /// to a concurrent node because an early failing condition prevents its following concurrent siblings from running.
    /// </summary>
    public class ConcurrentSelector : Composite
    {

        private int _FailureCount; // number of children that returns BehaviorResult.Failure
        //private bool[] _ChildrenResults;// state of children that finish their jobs and result is not BehaviorResult.Running


        //// <summary>
        //// At first time execution, make sure the  _ChildrenResults array is valid
        //// </summary>
        //private void CreateChildrenResults()
        //{
        //    if (_ChildrenResults == null || _ChildrenResults.Length != ChildCount)
        //    {
        //        _ChildrenResults = new bool[ChildCount];
        //        ResetChildrenResults();
        //    }
        //}

        ///// <summary>
        ///// Reset _ChildrenResults array
        ///// </summary>
        //private void ResetChildrenResults()
        //{
        //    _FailureCount = 0;
        //    for (int i = 0; i < _ChildrenResults.Length; i++)
        //    {
        //        _ChildrenResults[i] = false;
        //    }
        //}


        /// <summary> First check for conditions then rest of childs (default true)</summary>
        public bool FirstConditions { get; set; }
        /// <summary>
        /// if true, by first condition failure, ConcurrentSelector returns BehaviorResult.Failure
        /// </summary>
        public bool BreakOnConditionFailure { get; set; }

        /// <summary>
        /// CompositeType
        /// </summary>
        public override CompositeType CompositeType { get { return AI.CompositeType.Concurrent; } }

        /// <summary>
        /// FailurePolicy (default : FailOnAll)
        /// </summary>
        public FailurePolicy FailurePolicy { get; set; }
        /// <summary>
        /// SuccessPolicy (default : SucceedOnAll)
        /// </summary>
        public SuccessPolicy SuccessPolicy { get; set; }

        /// <summary>
        /// Create an instance of ConcurrentSelector
        /// </summary>
        /// <param name="name">Name of Behavior node</param>
        public ConcurrentSelector(string name)
            : base(name)
        {
            FailurePolicy = AI.FailurePolicy.FailOnAll;
            SuccessPolicy = AI.SuccessPolicy.SucceedOnAll;
            FirstConditions = true;
            BreakOnConditionFailure = false;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">state of BehaviorTree</param>
        /// <returns>Result</returns>
        protected override BehaviorResult Behave(BehaviorTreeState state)
        {
            _FailureCount = 0;
            //CreateChildrenResults(); // make sure the  _ChildrenResults array is valid
            BehaviorResult result = BehaviorResult.Success; // by default success
            if (FirstConditions) // first check conditions
                result = CheckConditions(state);
            if (result == BehaviorResult.Failure)// if fails no need to execute other nodes
                return result;

            // itrate throw children an execute them
            for (int i = 0; i < ChildCount; i++)
            {
                //if (_ChildrenResults[i]) continue;// if this child already executed ignore it

                BehaviorContainer node = this[i];
                state.Parameters = node.Parameters;

                // if this node executed first ignore it
                if (FirstConditions && node.Behavior.Type == BehaviorType.Condition) continue;

                BehaviorResult r = node.Behavior.Trace(state);// execute child node

                if (r == BehaviorResult.Failure)
                {
                    if (BreakOnConditionFailure && node.Behavior.Type == BehaviorType.Condition)
                    {
                        result = BehaviorResult.Failure;
                        break;
                    }
                    else
                        _FailureCount++;
                }
                // check failure policity
                if ((FailurePolicy == AI.FailurePolicy.FailOnOne && _FailureCount > 0) || (FailurePolicy == AI.FailurePolicy.FailOnAll && _FailureCount == ChildCount))
                {
                    result = BehaviorResult.Failure;
                    break;
                }
                // check success policity
                if (SuccessPolicy == AI.SuccessPolicy.SucceedOnOne && r == BehaviorResult.Success)
                {
                    result = BehaviorResult.Success;
                    break;
                }

                // if result of this node is running or result of any previous node is running, set result to running
                if (r == BehaviorResult.Running || result != BehaviorResult.Running)
                    result = r;
                //if (r != BehaviorResult.Running)
                //    _ChildrenResults[i] = true; // set ; child node at index i executed and finished it's job
            }


            //if (result != BehaviorResult.Running)
            //    ResetChildrenResults();
            return result;
        }

        /// <summary>
        /// iterate throw children and evaluate conditions
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <returns></returns>
        private BehaviorResult CheckConditions(BehaviorTreeState state)
        {
            BehaviorResult result = BehaviorResult.Success;
            for (int i = 0; i < ChildCount; i++)
            {
                BehaviorContainer node = this[i];
                state.Parameters = node.Parameters;

                if (node.Behavior.Type == BehaviorType.Condition)
                {
                    BehaviorResult r = node.Behavior.Trace(state);

                    if (r == BehaviorResult.Failure)
                    {
                        if (BreakOnConditionFailure && node.Behavior.Type == BehaviorType.Condition)
                        {
                            result = BehaviorResult.Failure;
                            break;
                        }
                        else
                            _FailureCount++;
                    }
                    // check failure policity
                    if ((FailurePolicy == AI.FailurePolicy.FailOnOne && _FailureCount > 0) || (FailurePolicy == AI.FailurePolicy.FailOnAll && _FailureCount == ChildCount))
                    {
                        result = BehaviorResult.Failure;
                        break;
                    }
                    // check success policity
                    if (SuccessPolicy == AI.SuccessPolicy.SucceedOnOne && r == BehaviorResult.Success)
                    {
                        result = BehaviorResult.Success;
                        break;
                    }

                    // diable these lines because : conditions returns success or failure as result
                    // if result of this node is running or result of any previous node is running, set result to running
                    //if (r == BehaviorResult.Running || result != BehaviorResult.Running)
                    //result = r;
                }
            }
            return result;
        }

        /// <summary>
        /// Reset behavior
        /// </summary>        
        /// <param name="state">State of BehaviorTree</param>                
        public override void ResetBehavior(BehaviorTreeState state)
        {
            if (Result == BehaviorResult.Running)
            {
                if (!IsTracing)
                    RunningChildIndex = -1;
                foreach (var child in this)
                {
                    if (child != null && !child.Behavior.IsLeaf)
                        child.Behavior.ResetBehavior(state);
                }

            }
            base.ResetBehavior(state);
        }
    }
    #endregion
}
