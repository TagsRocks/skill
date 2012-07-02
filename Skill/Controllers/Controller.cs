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

        public Spawner Spawner { get; internal set; }


        protected virtual void OnDie(object userData)
        {
            if (Behavior != null)
                Behavior.Reset();
        }

        protected virtual void OnDestroy()
        {
            if (Behavior != null)
                Behavior.Reset();
        }
    }
}
