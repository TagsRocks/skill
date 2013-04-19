using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Defines behavior of node when is child of a ConcurrentSelector
    /// </summary>
    public enum ConcurrencyMode
    {
        /// <summary>  Execute every update regardless of success or failure </summary>
        Unlimit,
        /// <summary>  Execute until success </summary>
        UntilSuccess,
        /// <summary>  Execute until failure </summary>
        UntilFailure,

    }

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
        private bool[] _ChildrenFirstExecution; // status of children that finish their jobs and result is not BehaviorResult.Running


        /// <summary>
        /// At first time execution, make sure the  _ChildrenResults array is valid
        /// </summary>
        private void CreateChildrenExecution()
        {
            if (_ChildrenFirstExecution == null || _ChildrenFirstExecution.Length != ChildCount)
            {
                _ChildrenFirstExecution = new bool[ChildCount];
                ResetChildrenExecution();
            }
        }

        /// <summary>
        /// Reset _ChildrenResults array
        /// </summary>
        private void ResetChildrenExecution()
        {            
            for (int i = 0; i < _ChildrenFirstExecution.Length; i++)
            {
                _ChildrenFirstExecution[i] = false;
            }
        }

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
            BreakOnConditionFailure = false;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="status">status of BehaviorTree</param>
        /// <returns>Result</returns>
        protected override BehaviorResult Behave(BehaviorTreeStatus status)
        {
            _FailureCount = 0;
            CreateChildrenExecution(); // make sure the  _ChildrenResults array is valid
            BehaviorResult result = BehaviorResult.Success; // by default success
            //if (FirstConditions) // first check conditions
            //result = CheckConditions(status);
            //if (result == BehaviorResult.Failure)// if fails no need to execute other nodes
            //return result;

            // itrate throw children an execute them
            for (int i = 0; i < ChildCount; i++)
            {
                BehaviorContainer node = this[i];
                status.Parameters = node.Parameters;

                if (_ChildrenFirstExecution[i])
                {
                    if (node.Behavior.Concurrency == ConcurrencyMode.UntilFailure && node.Behavior.Result == BehaviorResult.Failure)
                    {
                        _FailureCount++;
                        continue;
                    }
                    else if (node.Behavior.Concurrency == ConcurrencyMode.UntilSuccess && node.Behavior.Result == BehaviorResult.Success)
                    {
                        continue;
                    }
                }

                // if this node executed first ignore it
                //if (FirstConditions && node.Behavior.Type == BehaviorType.Condition) continue;

                BehaviorResult childResult = node.Behavior.Execute(status);// execute child node

                if (childResult == BehaviorResult.Failure)
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
                if (SuccessPolicy == AI.SuccessPolicy.SucceedOnOne && childResult == BehaviorResult.Success)
                {
                    result = BehaviorResult.Success;
                    break;
                }

                // if result of this node is running or result of any previous node is running, set result to running
                if (childResult == BehaviorResult.Running || result != BehaviorResult.Running)
                    result = childResult;

                _ChildrenFirstExecution[i] = true;
            }


            if (result != BehaviorResult.Running)
                ResetChildrenExecution();
            return result;
        }

        /// <summary>
        /// iterate throw children and evaluate conditions
        /// </summary>
        /// <param name="status">Status of BehaviorTree</param>
        /// <returns></returns>
        private BehaviorResult CheckConditions(BehaviorTreeStatus status)
        {
            BehaviorResult result = BehaviorResult.Success;
            for (int i = 0; i < ChildCount; i++)
            {
                BehaviorContainer node = this[i];
                status.Parameters = node.Parameters;

                if (node.Behavior.Type == BehaviorType.Condition)
                {
                    BehaviorResult r = node.Behavior.Execute(status);

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
        /// Reset behavior. For internal use. when a branch with higher priority executed, let nodes in previous branch reset
        /// </summary>        
        /// <param name="status">Status of BehaviorTree</param>   
        public override void ResetBehavior(BehaviorTreeStatus status)
        {
            if (Result == BehaviorResult.Running)
            {
                if (LastUpdateId != status.UpdateId)
                    ResetChildrenExecution();
            }
            base.ResetBehavior(status);
        }
    }
    #endregion
}
