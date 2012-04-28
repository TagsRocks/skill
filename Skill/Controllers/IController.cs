using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Skill.Animation;

namespace Skill.Controllers
{
    public interface IController
    {
        Skill.AI.BehaviorTree Behavior { get; }
        Skill.Animation.AnimationTree Animation { get; }
        Posture Posture { get; }
    }
}
