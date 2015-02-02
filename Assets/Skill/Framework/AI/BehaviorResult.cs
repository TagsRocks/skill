using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.AI
{
    /// <summary>
    /// Defines result of Behavior execution
    /// </summary>
    public enum BehaviorResult
    {        
        /// <summary>
        /// Failure
        /// </summary>
        Failure,
        /// <summary>
        /// Success
        /// </summary>
        Success,
        /// <summary>
        /// Running. needs to run next update
        /// </summary>
        Running,        
    }
}
