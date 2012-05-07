using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    #region Action

    /// <summary>
    /// Represents the method that will handle execution of action by user
    /// </summary>
    /// <param name="tree">BehaviorTree</param>
    /// <param name="parameters">Parameters for action</param>
    /// <returns>State of action</returns>
    public delegate BehaviorResult ActionHandler(BehaviorTree tree, BehaviorParameterCollection parameters);

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
        /// Create an instance of Action
        /// </summary>
        /// <param name="name">Name of action</param>
        /// <param name="handler">the function to call at execution time</param>
        public Action(string name, ActionHandler handler)
            : base(name, BehaviorType.Action)
        {
            this._Handler = handler;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">State of BehaviorTree</param>
        /// <returns>Result of action</returns>
        protected override BehaviorResult Behave(BehaviorState state)
        {
            BehaviorResult result = BehaviorResult.Failure;// by default failure

            // execute handler and get result back
            if (_Handler != null) result = _Handler(state.BehaviorTree, state.Parameters);

            // if action needs to run next frame store it's reference
            if (result == BehaviorResult.Running)
                state.RunningAction = this;
            else
                state.RunningAction = null;
            return result;
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
