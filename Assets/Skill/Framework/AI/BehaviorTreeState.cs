using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    public class BehaviorTreeState : PrioritySelector
    {
        public override CompositeType CompositeType { get { return AI.CompositeType.State; } }

        /// <summary>
        /// Create an instance of BehaviorTreeState
        /// </summary>
        /// <param name="name">Name of Behavior</param>
        public BehaviorTreeState(string name)
            : base(name)
        {
        }
    }
}
