using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    public class BehaviorState
    {
        public Exception Exception { get; internal set; }
        public Action RunningAction { get; internal set; }
        public BehaviorTree BehaviorTree { get; private set; }

        void Reset()
        {            
            RunningAction = null;
            Exception = null;
        }

        public BehaviorState(BehaviorTree behaviorTree)
        {
            this.BehaviorTree = behaviorTree;
            this.Exception = null;
            this.RunningAction = null;
        }        
    }
}
