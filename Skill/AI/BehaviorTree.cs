using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    #region BehaviorTree
    /// <summary>
    /// Base class of BehaviorTree that manage execution of Behaviors
    /// </summary>
    public abstract class BehaviorTree
    {
        private float _LastUpdateTime;// last update time

        /// <summary>
        /// Root of Tree
        /// </summary>
        public Behavior Root { get; private set; }

        /// <summary> 
        /// To enable update time interval set this to more than zero (default is 0.2f)
        /// still call updates each frame, tree reject it automatically
        /// </summary>
        public float UpdateTimeInterval { get; set; }

        /// <summary>
        /// The controller that using this BbehaviorTree
        /// </summary>
        public Skill.Controllers.Controller Controller { get; private set; }
        /// <summary>
        /// User data
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// State of BehaviorTree
        /// </summary>
        public BehaviorState State { get; private set; }

        /// <summary>
        /// Implement by subclass to create tree hierarchy and return root node.
        /// </summary>
        /// <returns>Root node</returns>
        protected abstract Behavior CreateTree();

             
        /// <summary>
        /// Create an instance of BehaviorTree
        /// </summary>
        /// <param name="controller">The controller that using this BbehaviorTree.</param>
        /// <remarks>
        /// controller reserved for future version
        /// </remarks>
        public BehaviorTree(Skill.Controllers.Controller controller)
        {
            if (controller == null)
                throw new ArgumentNullException("Controller is null");
            this.Controller = controller;
            this.State = new BehaviorState();
            this._LastUpdateTime = 0;
            this.UpdateTimeInterval = 0.2f;
            Root = CreateTree();
            if (Root == null)
                throw new InvalidProgramException("You must provide valid root in subclass");
        }

        /// <summary> Occurs when Behavior Tree updated </summary>
        public event EventHandler Updated;
        /// <summary> Call Updated event </summary>
        protected virtual void OnUpdated()
        {
            if (Updated != null) Updated(this,EventArgs.Empty);
        }

        /// <summary>
        /// Update Tree
        /// </summary>
        public void Update()
        {
            if (UpdateTimeInterval > 0)
            {
                if (UnityEngine.Time.time < (_LastUpdateTime + UpdateTimeInterval))
                    return;
            }
            ForceUpdate();
        }

        /// <summary>
        /// Force update tree even not reach UpdateTimeInterval
        /// </summary>
        public void ForceUpdate()
        {
            this._LastUpdateTime = UnityEngine.Time.time;            
            State.Begin();
            Root.Trace(State);

            if (State.Exception != null)
            {
                UnityEngine.Debug.LogWarning(State.Exception.ToString());
            }

            OnUpdated();
        }

        /// <summary>
        /// Call this when your agent dies, destroyed, or whenever you do not need BehaviorTree
        /// this is important because sometimes maybe one Behavior node locked an AccessKey and could not unlock it before next update
        /// you have to call this to make sure other instance of BehaviorTree can access that AccessKey.
        /// </summary>
        public void Reset()
        {
            Root.ResetBehavior(true);
        }
    }
    #endregion
}
