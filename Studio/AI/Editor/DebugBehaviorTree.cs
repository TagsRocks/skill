using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Framework.AI;

namespace Skill.Studio.AI.Editor
{
    class DebugBehaviorTree
    {
        /// <summary>
        /// Root of Tree
        /// </summary>
        public Behavior Root { get; private set; }

        /// <summary>
        /// User data
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// State of BehaviorTree
        /// </summary>
        public BehaviorTreeState State { get; private set; }

        /// <summary>
        /// Create an instance of BehaviorTree
        /// </summary>
        /// <param name="controller">The controller that using this BbehaviorTree.</param>
        /// <remarks>
        /// controller reserved for future version
        /// </remarks>
        public DebugBehaviorTree(Behavior root)
        {
            this.Root = root;
            this.State = new BehaviorTreeState(root);
        }

        /// <summary>
        /// Update Tree
        /// </summary>
        public void Update()
        {
            State.Begin();
            Root.Trace(State);
            if (State.Exception != null)
            {
                //System.Diagnostics.Debugger.Log(0,"",State.Exception.ToString);
            }
        }


        /// <summary>
        /// Call this when your agent dies, destroyed, or whenever you do not need BehaviorTree
        /// this is important because sometimes maybe one Behavior node locked an AccessKey and could not unlock it before next update
        /// you have to call this to make sure other instance of BehaviorTree can access that AccessKey.
        /// </summary>
        public void Reset()
        {
            Root.ResetBehavior(State);
        }
    }
}
