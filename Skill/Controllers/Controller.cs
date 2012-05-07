using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Skill.Animation;

namespace Skill.Controllers
{
    public class Controller : MonoBehaviour
    {
        public Skill.AI.BehaviorTree Behavior { get; protected set; }

        public Skill.Animation.AnimationTree Animation { get; protected set; }

        public AnimPosture Posture { get; protected set; }
    }
}
