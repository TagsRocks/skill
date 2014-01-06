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
        public BehaviorTreeState CurrentState
        {
            get
            {
                if (_BehaviorTree.Root.Debug != null)
                    return _BehaviorTree.Root.Debug.Behavior as BehaviorTreeState;
                return null;
            }
        }

        /// <summary>
        /// Status of BehaviorTree
        /// </summary>
        public BehaviorTreeStatus Status { get; private set; }

        /// <summary>
        /// Default state of behavior tree
        /// </summary>
        public string DefaultState { get { return _BehaviorTree.DefaultState; } }

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
        public void Update()
        {
            this._Reset = false;
            if (_NextState != null && _NextState != _BehaviorTree.Root)
            {
                string preState = string.Empty;
                if (_BehaviorTree.Root != null)
                {
                    _BehaviorTree.Root.Debug.Behavior.ResetBehavior(this.Status);
                    preState = _BehaviorTree.Root.Name;
                }
                _BehaviorTree.ChangeState(_NextState.Name);
                OnStateChanged(preState, _NextState.Name);
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
