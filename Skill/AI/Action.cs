﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{

    #region ActionResetEventHandler
    /// <summary>
    /// Represents the method to handle Action reset events.
    /// </summary>
    /// <param name="action">Sender behavior</param>        
    public delegate void ActionResetEventHandler(Action action);
    #endregion

    #region Action

    /// <summary>
    /// Represents the method that will handle execution of action by user
    /// </summary>    
    /// <param name="parameters">Parameters for action</param>
    /// <returns>State of action</returns>
    public delegate BehaviorResult ActionHandler(BehaviorParameterCollection parameters);

    /// <summary>
    /// Actions which finally implement an actors or game world state changes, for example to plan a path and move on it, to sense for the nearest enemies,
    /// to show certain animations, switch weapons, or run a specified sound. Actions will typically coordinate and call into different game systems.
    /// They might run for one simulation tick – one frame – or might need to be ticked for multiple frames to finish their work.
    /// Action is leaf node.
    /// </summary>
    public class Action : Behavior
    {
        private ActionHandler _Handler = null;// handler        

        /// <summary>
        /// Occurs when behavior is reset
        /// </summary>
        public event ActionResetEventHandler Reset;

        /// <summary>
        /// Create an instance of Action
        /// </summary>
        /// <param name="name">Name of action</param>
        /// <param name="handler">the function to call at execution time</param>
        public Action(string name, ActionHandler handler)
            : base(name, BehaviorType.Action)
        {
            this._Handler = handler;
        }


        // Let action know that we update it manually
        internal bool AlreadyUpdated { get; set; }

        // update action manually when action is running
        internal void UpdateImmediately(BehaviorState state)
        {            
            try
            {
                // execute handler and get result back
                if (this._Handler != null)
                    this.Result = this._Handler(state.Parameters);
                else
                    this.Result = BehaviorResult.Failure;// by default failure

                this.AlreadyUpdated = true;

                // we do not remove action from RunningActions here because this method called inside a foreach of RunningActions 
                // and we can not modify list inside foreach
            }
            catch (Exception ex)
            {
                state.Exception = ex;
                this.Result = BehaviorResult.Failure;
                return;
            }                        
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <returns>Result of action</returns>
        protected override BehaviorResult Behave(BehaviorState state)
        {
            if (AlreadyUpdated)
            {
                AlreadyUpdated = false;
                return Result;
            }

            BehaviorResult result = BehaviorResult.Failure;// by default failure

            // execute handler and get result back            
            if (_Handler != null) result = _Handler(state.Parameters);

            // if action needs to run next frame store it's reference
            if (result == BehaviorResult.Running)
                state.RunningActions.Add(this, state.Parameters);
            else
                state.RunningActions.Remove(this);
            return result;
        }

        /// <summary>
        /// On Reset
        /// </summary>        
        protected virtual void OnReset()
        {
            AlreadyUpdated = false;
            if (Reset != null) Reset(this);
        }

        /// <summary>
        /// Reset behavior. For internal use. when a branch with higher priority executed, let nodes in previous branch reset
        /// </summary>        
        /// <param name="resetChildren">Reset children too</param>
        public override void ResetBehavior(bool resetChildren = false)
        {
            OnReset();
            base.ResetBehavior();
        }


        /// <summary>
        /// for internal use when behavior tree let action continue (when result is BehaviorResult.Running)
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <returns>Result</returns>
        internal BehaviorResult Continue(BehaviorState state)
        {
            return Behave(state);
        }
    }
    #endregion
}
