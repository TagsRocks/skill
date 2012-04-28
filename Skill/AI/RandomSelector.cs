using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public class RandomSelector : Composite
    {
        private float _TotalWeight;
        public override CompositeType CompositeType { get { return AI.CompositeType.Random; } }

        public RandomSelector(string name)
            : base(name)
        {
            _TotalWeight = 0;
        }

        public override void Add(Behavior item)
        {
            base.Add(item);
            if (Contains(item))
                _TotalWeight += item.Weight;
        }

        public override void Clear()
        {
            base.Clear();
            _TotalWeight = 0;
        }

        public override bool Remove(Behavior item)
        {
            bool r = base.Remove(item);
            if (r)
            {
                _TotalWeight -= item.Weight;
                if (_TotalWeight < 0) _TotalWeight = 0;
            }
            return r;
        }

        protected override BehaviorResult Behave(BehaviorState state)
        {
            if (RunningChildIndex < 0)
                RunningChildIndex = GetRandomIndex();
            BehaviorResult result = BehaviorResult.Failure;
            Behavior node = this[RunningChildIndex];
            result = node.Trace(state);
            if (result != BehaviorResult.Running)
                RunningChildIndex = -1;
            return result;
        }

        private int GetRandomIndex()
        {
            float rnd = UnityEngine.Random.Range(0.0f, _TotalWeight);
            float sum = 0;
            for (int i = 0; i < Count; i++)
            {
                sum += this[i].Weight;
                if (sum >= rnd) return i;
            }
            return 0;
        }
    }
}
