using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework
{
    /// <summary>
    /// Behavioural object
    /// </summary>
    public interface IBehavioural
    {
        /// <summary> Behavior </summary>
        Skill.Framework.AI.BehaviorTree Behavior { get; }
    }
}
