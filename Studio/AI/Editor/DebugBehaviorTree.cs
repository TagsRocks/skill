using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Framework.AI;

namespace Skill.Studio.AI.Editor
{
    /// <summary>
    /// Base class of BehaviorTree that manage execution of Behaviors
    /// </summary>
    public class DebugBehaviorTree : Skill.Framework.AI.IBehaviorTree
    {
        private bool _Reset;
        private BehaviorTreeViewModel _BehaviorTree;
        private BehaviorViewModel _NextState;

        public void ChangeState(string destinationState)
        {
            if (string.IsNullOrEmpty(destinationState))
                throw new ArgumentException("Invalid state");

            if (_BehaviorTree.Root != null && _BehaviorTree.Root.Name == destinationState) return;

            _NextState = null;

            foreach (var s in _BehaviorTree.States)
            {
                if (s.Name == destinationState)
                {
                    _NextState = s;
                    return;
                }
            }

            throw new ArgumentException("Invalid state");
        }

        /// <summary>
        /// Current state of behavior tree.
        /// </summary>
        public Behavior CurrentState { get { return _BehaviorTree.Root.Debug.Behavior; } }

        /// <summary>
        /// Status of BehaviorTree
        /// </summary>
        public BehaviorTreeStatus Status { get; private set; }

        /// <summary>
        /// Default state of behavior tree
        /// </summary>
        public string DefaultState { get { return _BehaviorTree.DefaultState; } }

        /// <summary>
        /// Occurs when leave current state
        /// </summary>
        public event StateTransitionHandler LeaveState;
        /// <summary>
        /// Called when leave current state
        /// </summary>
        protected virtual void OnLeaveState()
        {
            if (_BehaviorTree.Root != null && LeaveState != null)
                LeaveState(this, _BehaviorTree.Root.Name);
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
            if (_BehaviorTree.Root != null && EnterState != null)
                EnterState(this, _BehaviorTree.Root.Name);
        }

        /// <summary>
        /// Create an instance of BehaviorTree
        /// </summary>
        public DebugBehaviorTree(BehaviorTreeViewModel behaviorTree)
        {
            this._Reset = false;
            this._BehaviorTree = behaviorTree;            
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
        public void Update(Skill.Framework.Controller controller)
        {
            this._Reset = false;
            if (_NextState != null && _NextState != _BehaviorTree.Root)
            {
                if (_BehaviorTree.Root != null)
                {
                    _BehaviorTree.Root.Debug.Behavior.ResetBehavior(this.Status);
                    OnLeaveState();
                }
                _BehaviorTree.ChangeState(_NextState.Name);
                OnEnterState();
                _NextState = null;
                return; // let ui updated
            }
            _NextState = null;

            Status.Begin();
            if (_BehaviorTree.Root != null)
            {
                CurrentState.Execute(Status);

                if (Status.Exception != null)
                {
                    throw Status.Exception;
                }

                CurrentState.ResetBehavior(this.Status);

                OnUpdated();
            }
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
}
