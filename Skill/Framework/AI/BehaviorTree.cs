using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    #region BehaviorTree
    /// <summary>
    /// Base class of BehaviorTree that manage execution of Behaviors
    /// </summary>
    public abstract class BehaviorTree
    {
        private float _LastUpdateTime;// last update time
        private List<Action> _FinishedActions;
        private bool _Reset;

        /// <summary>
        /// Root of Tree
        /// </summary>
        public Behavior Root { get; private set; }

        /// <summary> 
        /// To enable update time interval set this to more than zero (default is 0.5f). Keep call Update() each frame.
        /// </summary>
        public float UpdateTimeInterval { get; set; }

        /// <summary>
        /// The controller that using this BbehaviorTree
        /// </summary>
        public Controller Controller { get; private set; }
        /// <summary>
        /// User data
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// State of BehaviorTree
        /// </summary>
        public BehaviorTreeState State { get; private set; }

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
        public BehaviorTree()
        {
            this._Reset = false;
            this._LastUpdateTime = 0;
            this.UpdateTimeInterval = 0.5f;
            this._FinishedActions = new List<Action>();
            this.Root = CreateTree();
            if (Root == null)
                throw new InvalidProgramException("You must provide valid root in subclass");
            this.State = new BehaviorTreeState(Root);
        }

        /// <summary> Occurs when Behavior Tree updated </summary>
        public event EventHandler Updated;
        /// <summary> Call Updated event </summary>
        protected virtual void OnUpdated()
        {
            if (Updated != null) Updated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Update Tree
        /// </summary>
        /// <param name="controller">Controller to update</param>
        /// <remarks>
        /// By providing Controller as parameter, it is possible to reuse BehaviorTree by another Controller when current(previous) Controller is dead.
        /// </remarks>
        public void Update(Controller controller)
        {
            this.Controller = controller;
            if (Controller == null)
                throw new ArgumentNullException("Controller is null");
            State.Exception = null;
            if (UpdateTimeInterval > 0)
            {
                if (UnityEngine.Time.time < (_LastUpdateTime + UpdateTimeInterval))
                {
                    // just update running actions
                    foreach (var runningAction in State.RunningActions)
                    {
                        State.Parameters = runningAction.Parameters;
                        runningAction.Action.UpdateImmediately(State);
                        if (runningAction.Action.Result != BehaviorResult.Running)
                            _FinishedActions.Add(runningAction.Action);
                    }
                    if (_FinishedActions.Count == 0) // we does not need to update BehaviorTree
                    {
                        if (State.Exception != null)
                        {
                            UnityEngine.Debug.LogWarning(State.Exception.ToString());
                        }
                        return;
                    }
                }
            }
            ForceUpdate(controller);

            if (_FinishedActions.Count > 0)
            {
                foreach (var action in _FinishedActions)
                {
                    if (action.Result != BehaviorResult.Running)
                        State.RunningActions.Remove(action);
                    action.AlreadyUpdated = false;
                }
                this._FinishedActions.Clear();
            }
        }

        /// <summary>
        /// Force update tree even not reach UpdateTimeInterval
        /// </summary>
        /// <param name="controller">Controller to update</param>
        /// <remarks>
        /// By providing Controller as parameter, it is possible to reuse BehaviorTree by another Controller when current(previous) Controller is dead.
        /// </remarks>
        public void ForceUpdate(Controller controller)
        {
            this._Reset = false;
            this.Controller = controller;
            if (Controller == null)
                throw new ArgumentNullException("Controller is null");
            this._LastUpdateTime = UnityEngine.Time.time;
            State.Begin();
            Root.Trace(State);

            if (State.Exception != null)
            {
                UnityEngine.Debug.LogWarning(State.Exception.ToString());
            }

            Root.ResetBehavior(this.State);

            OnUpdated();
        }

        /// <summary>
        /// Call this when your agent dies, destroyed, or whenever you do not need BehaviorTree
        /// this is important because sometimes maybe one Behavior node locked an AccessKey and could not unlock it before next update
        /// you have to call this to make sure other instance of BehaviorTree can access that AccessKey.
        /// </summary>
        public void Reset()
        {
            if (_Reset) return;
            State.Begin();
            Root.ResetBehavior(this.State);
            this._Reset = true;
        }
    }
    #endregion
}
