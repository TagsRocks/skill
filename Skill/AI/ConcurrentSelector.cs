using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{

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


    public class ConcurrentSelector : Composite
    {
        
        private int _FailureCount;
        private bool[] _ChildrenResults;


        private void CreateChildrenResults()
        {
            if (_ChildrenResults == null || _ChildrenResults.Length != Count)
            {
                _ChildrenResults = new bool[Count];
                ResetChildrenResults();
            }
        }

        private void ResetChildrenResults()
        {
            _FailureCount = 0;
            for (int i = 0; i < _ChildrenResults.Length; i++)
            {
                _ChildrenResults[i] = false;
            }
        }


        /// <summary> first check conditions then rest of childs (default true)</summary>
        public bool FirstConditions { get; set; }

        public bool BreakOnConditionFailure { get; set; }

        public override CompositeType CompositeType { get { return AI.CompositeType.Concurrent; } }

        public FailurePolicy FailurePolicy { get; set; }
        public SuccessPolicy SuccessPolicy { get; set; }

        public ConcurrentSelector(string name)
            : base(name)
        {
            FailurePolicy = AI.FailurePolicy.FailOnAll;
            SuccessPolicy = AI.SuccessPolicy.SucceedOnAll;
            FirstConditions = true;
            BreakOnConditionFailure = false;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {
            CreateChildrenResults();
            BehaviorResult result = BehaviorResult.Success;
            if (FirstConditions)
                result = CheckConditions(state);
            if (result == BehaviorResult.Failure)
                return result;

            for (int i = 0; i < Count; i++)
            {
                if (_ChildrenResults[i]) continue;
                Behavior node = this[i];
                if (FirstConditions && node.Type == BehaviorType.Condition) continue;
                BehaviorResult r = node.Trace(state);

                if (r == BehaviorResult.Failure)
                {
                    if (BreakOnConditionFailure && node.Type == BehaviorType.Condition)
                    {
                        result = BehaviorResult.Failure;
                        break;
                    }
                    else
                        _FailureCount++;
                }
                if ((FailurePolicy == AI.FailurePolicy.FailOnOne && _FailureCount > 0) || (FailurePolicy == AI.FailurePolicy.FailOnAll && _FailureCount == Count))
                {
                    result = BehaviorResult.Failure;
                    break;
                }
                if (SuccessPolicy == AI.SuccessPolicy.SucceedOnOne && r == BehaviorResult.Success)
                {
                    result = BehaviorResult.Success;
                    break;
                }

                if (r == BehaviorResult.Running || result != BehaviorResult.Running)
                    result = r;
                if (r != BehaviorResult.Running)
                    _ChildrenResults[i] = true;
            }


            if (result != BehaviorResult.Running)
                ResetChildrenResults();
            return result;
        }

        private BehaviorResult CheckConditions(BehaviorState data)
        {
            BehaviorResult result = BehaviorResult.Success;
            for (int i = 0; i < Count; i++)
            {
                Behavior node = this[i];
                if (node.Type == BehaviorType.Condition)
                {
                    BehaviorResult r = node.Trace(data);

                    if (r == BehaviorResult.Failure)
                    {
                        if (BreakOnConditionFailure && node.Type == BehaviorType.Condition)
                        {
                            result = BehaviorResult.Failure;
                            break;
                        }
                        else
                            _FailureCount++;
                    }
                    if ((FailurePolicy == AI.FailurePolicy.FailOnOne && _FailureCount > 0) || (FailurePolicy == AI.FailurePolicy.FailOnAll && _FailureCount == Count))
                    {
                        result = BehaviorResult.Failure;
                        break;
                    }
                    if (SuccessPolicy == AI.SuccessPolicy.SucceedOnOne && r == BehaviorResult.Success)
                    {
                        result = BehaviorResult.Success;
                        break;
                    }

                    if (r == BehaviorResult.Running || result != BehaviorResult.Running)
                        result = r;
                }
            }
            return result;
        }
    }
}
