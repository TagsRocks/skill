using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    #region BehaviorTree

    public delegate void StateTransitionHandler(object sender, string stateName);

    public interface IBehaviorTree
    {
        event EventHandler Updated;

        void ChangeState(string destinationState);
        Behavior CurrentState { get; }
        BehaviorTreeStatus Status { get; }
        /// <summary>
        /// Default state of behavior tree
        /// </summary>
        string DefaultState { get; }
        void Update(Controller controller);
        void Reset();
    }

    /// <summary>
    /// Base class of BehaviorTree that manage execution of Behaviors
    /// </summary>
    public abstract class BehaviorTree : IBehaviorTree
    {
        private float _LastUpdateTime;// last update time
        private List<Action> _FinishedActions;
        private bool _Reset;
        private Dictionary<string, Behavior> _States;
        private Behavior _CurrentState;
        private Behavior _NextState;

        public void ChangeState(string destinationState)
        {
            if (string.IsNullOrEmpty(destinationState))
                throw new ArgumentException("Invalid state");

            if (_CurrentState != null && _CurrentState.Name == destinationState) return;

            _NextState = null;

            if (!_States.TryGetValue(destinationState, out _NextState))
                throw new ArgumentException("Invalid state");
        }

        /// <summary>
        /// Current state of behavior tree.
        /// </summary>
        public Behavior CurrentState { get { return _CurrentState; } }

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
        /// Status of BehaviorTree
        /// </summary>
        public BehaviorTreeStatus Status { get; private set; }

        /// <summary>
        /// Implement by subclass to create tree hierarchy and return state nodes.
        /// </summary>
        /// <returns>States of tree</returns>
        protected abstract Behavior[] CreateTree();

        /// <summary>
        /// Default state of behavior tree
        /// </summary>
        public abstract string DefaultState { get; }

        /// <summary>
        /// Occurs when leave current state
        /// </summary>
        public event StateTransitionHandler LeaveState;
        /// <summary>
        /// Called when leave current state
        /// </summary>
        protected virtual void OnLeaveState()
        {
            if (_CurrentState != null && LeaveState != null)
                LeaveState(this, _CurrentState.Name);
        }

        /// <summary>
        /// Occurs when enter new state
        /// </summary>
        public event StateTransitionHandler EnterState;
        /// <summary>
        /// Called when enter new state
        /// </summary>
        protected virtual void OnEnterState()
        {
            if (_CurrentState != null && EnterState != null)
                EnterState(this, _CurrentState.Name);
        }

        /// <summary>
        /// Create an instance of BehaviorTree
        /// </summary>
        public BehaviorTree()
        {
            this._Reset = false;
            this._LastUpdateTime = 0;
            this.UpdateTimeInterval = 0.5f;
            this._FinishedActions = new List<Action>();

            Behavior[] states = CreateTree();
            if (states == null || states.Length == 0)
                throw new InvalidProgramException("You must provide at least one valid state");

            _States = new Dictionary<string, Behavior>();
            foreach (var s in states)
                _States.Add(s.Name, s);
            ChangeState(DefaultState);
            this.Status = new BehaviorTreeStatus(this);
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
            Status.Exception = null;
            if (UpdateTimeInterval > 0)
            {
                if (UnityEngine.Time.time < (_LastUpdateTime + UpdateTimeInterval))
                {
                    // just update running actions
                    foreach (var runningAction in Status.RunningActions)
                    {
                        Status.Parameters = runningAction.Parameters;
                        runningAction.Action.UpdateImmediately(Status);
                        if (runningAction.Action.Result != BehaviorResult.Running)
                            _FinishedActions.Add(runningAction.Action);
                    }
                    if (_FinishedActions.Count == 0) // we does not need to update BehaviorTree
                    {
                        if (Status.Exception != null)
                        {
                            UnityEngine.Debug.LogWarning(Status.Exception.ToString());
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
                        Status.RunningActions.Remove(action);
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

            if (_NextState != null && _NextState != _CurrentState)
            {
                if (_CurrentState != null)
                {
                    _CurrentState.ResetBehavior(this.Status);
                    OnLeaveState();
                }
                _CurrentState = _NextState;
                OnEnterState();
            }
            _NextState = null;
            Status.Begin();
            CurrentState.Execute(Status);

            if (Status.Exception != null)
            {
                UnityEngine.Debug.LogWarning(Status.Exception.ToString());
            }

            CurrentState.ResetBehavior(this.Status);

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
            Status.Begin();
            CurrentState.ResetBehavior(this.Status);
            this._Reset = true;
            ChangeState(DefaultState);
        }
    }
    #endregion
}
