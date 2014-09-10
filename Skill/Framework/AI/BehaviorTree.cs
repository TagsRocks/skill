using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    #region BehaviorTree

    public class ChangeStateEventArgs : EventArgs
    {
        public string PreviousState { get; private set; }
        public string NextState { get; private set; }

        public ChangeStateEventArgs(string previousState, string nextState)
        {
            this.PreviousState = previousState;
            this.NextState = nextState;
        }
    }

    public delegate void ChangeStateEventHandler(object sender, ChangeStateEventArgs args);

    public interface IBehaviorTree
    {
        event EventHandler Updated;

        void ChangeState(string destinationState);
        BehaviorTreeState CurrentState { get; }
        BehaviorTreeStatus Status { get; }
        /// <summary>
        /// Default state of behavior tree
        /// </summary>
        string DefaultState { get; }
        void Update();
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
        private Dictionary<string, BehaviorTreeState> _States;
        private BehaviorTreeState _CurrentState;
        private BehaviorTreeState _NextState;

        public void ChangeState(string destinationState)
        {
            if (string.IsNullOrEmpty(destinationState))
                throw new ArgumentException("Invalid state");

            if (_CurrentState != null && _CurrentState.Name == destinationState) return;
            if (_NextState != null) return;
            _NextState = null;
            if (!_States.TryGetValue(destinationState, out _NextState))
                throw new ArgumentException("Invalid state");
            Status.Interrupt();
        }

        /// <summary>
        /// If true, the tree updates 'running actions' everyframe and update whole tree if required
        /// </summary>
        public bool ContinuousUpdate { get; set; }

        /// <summary> States of BehaviorTree </summary>
        public BehaviorTreeState[] States { get; private set; }

        /// <summary>
        /// Current state of behavior tree.
        /// </summary>
        public BehaviorTreeState CurrentState { get { return _CurrentState; } }

        /// <summary> 
        /// To enable update time interval set this to more than zero (default is 1.0f). Keep call Update() each frame.
        /// </summary>
        public float UpdateTimeInterval { get; set; }


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
        protected abstract BehaviorTreeState[] CreateTree();

        /// <summary>
        /// Default state of behavior tree
        /// </summary>
        public abstract string DefaultState { get; }

        /// <summary>
        /// Occurs when state of behaviortree changed
        /// </summary>
        public event ChangeStateEventHandler StateChanged;
        /// <summary>
        /// Occurs when state of behaviortree changed
        /// </summary>
        protected virtual void OnStateChanged(string preState, string newState)
        {
            if (StateChanged != null)
            {
                StateChanged(this, new ChangeStateEventArgs(preState, newState));
            }
        }

        /// <summary>
        /// Create an instance of BehaviorTree
        /// </summary>
        public BehaviorTree()
        {
            this._Reset = false;
            this._LastUpdateTime = 0;
            this.UpdateTimeInterval = 1.0f;
            this._FinishedActions = new List<Action>();

            States = CreateTree();
            if (States == null || States.Length == 0)
                throw new InvalidProgramException("You must provide at least one valid state");

            _States = new Dictionary<string, BehaviorTreeState>();
            foreach (var s in States)
                _States.Add(s.Name, s);
            this.Status = new BehaviorTreeStatus(this);
            Reset();
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
        public void Update()
        {
            Status.Exception = null;
            if (UpdateTimeInterval > 0 && _NextState == null)// if state is changed we have to force update and ignore running actions in previous state
            {
                if (UnityEngine.Time.time < (_LastUpdateTime + UpdateTimeInterval))
                {
                    if (ContinuousUpdate)
                    {
                        // just update running actions
                        foreach (var runningAction in Status.RunningActions)
                        {
                            Status.Parameters = runningAction.Parameters;
                            runningAction.Action.UpdateImmediately(Status);
                            if (runningAction.Action.Result != BehaviorResult.Running)
                                _FinishedActions.Add(runningAction.Action);
                        }
                        if (_FinishedActions.Count == 0) // we do not need to update BehaviorTree
                        {
                            if (Status.Exception != null)
                            {
                                UnityEngine.Debug.LogWarning(Status.Exception.ToString());
                            }
                            return;
                        }
                    }
                    else
                        return;
                }
            }
            ForceUpdate();

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
        public void ForceUpdate()
        {
            this._Reset = false;
            this._LastUpdateTime = UnityEngine.Time.time;

            if (_NextState != null && _NextState != _CurrentState)
            {
                foreach (var runningAction in Status.RunningActions)
                    runningAction.Action.Interrupt();
                Status.RunningActions.Clear();

                string preState = string.Empty;
                if (_CurrentState != null)
                {
                    _CurrentState.ResetBehavior(this.Status);
                    preState = _CurrentState.Name;
                }
                _CurrentState = _NextState;
                OnStateChanged(preState, _CurrentState.Name);
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
            if (CurrentState != null)
                CurrentState.ResetBehavior(this.Status);
            this._Reset = true;
            ChangeState(DefaultState);
        }
    }
    #endregion
}
