using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// select a random child by chance for execution.
    /// </summary>
    public class RandomSelector : Composite
    {
        private float _TotalWeight;

        /// <summary>
        /// CompositeType
        /// </summary>
        public override CompositeType CompositeType { get { return AI.CompositeType.Random; } }

        /// <summary>
        /// Create an instance of RandomSelector
        /// </summary>
        /// <param name="name">Name of Behavior node</param>
        public RandomSelector(string name)
            : base(name)
        {
            _TotalWeight = 0;
        }

        /// <summary>
        /// Add child . Remember to set weight of child before call this function
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parameters"></param>
        public override void Add(Behavior child, BehaviorParameterCollection parameters = null)
        {
            _TotalWeight += child.Weight;
            base.Add(child, parameters);            
        }

        /// <summary>
        /// Remove all children
        /// </summary>
        public override void RemoveAll()
        {
            base.RemoveAll();
            _TotalWeight = 0;
        }

        /// <summary>
        /// Remove specified child from children
        /// </summary>
        /// <param name="child">behavior child to remove</param>
        /// <returns>true for success, otherwise false</returns>
        public override bool Remove(Behavior child)
        {
            bool r = base.Remove(child);
            if (r)
            {
                _TotalWeight -= child.Weight;
                if (_TotalWeight < 0) _TotalWeight = 0;
            }
            return r;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">State od BehaviorTre</param>
        /// <returns>Result</returns>
        protected override BehaviorResult Behave(BehaviorState state)
        {
            if (RunningChildIndex < 0)
                RunningChildIndex = GetRandomIndex();// pick random node
            BehaviorResult result = BehaviorResult.Failure;
            BehaviorContainer node = this[RunningChildIndex];            
            state.Parameters = node.Parameters;
            result = node.Behavior.Trace(state);            
            if (result != BehaviorResult.Running)
                RunningChildIndex = -1;
            return result;
        }

        /// <summary>
        /// Select random child by chance
        /// </summary>
        /// <returns>Index of selected child</returns>
        private int GetRandomIndex()
        {
            float rnd = UnityEngine.Random.Range(0.0f, _TotalWeight);
            float sum = 0;
            for (int i = 0; i < ChildCount; i++)
            {
                sum += this[i].Behavior.Weight;
                if (sum >= rnd) return i;
            }
            return 0;
        }
    }
}
