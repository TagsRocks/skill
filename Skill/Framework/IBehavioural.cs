using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework
{
    public interface IBehavioural
    {
        Skill.Framework.AI.BehaviorTree Behavior { get; }
    }
}
