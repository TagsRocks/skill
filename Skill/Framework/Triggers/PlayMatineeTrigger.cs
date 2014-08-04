using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Triggers
{
    public class PlayMatineeTrigger : Trigger
    {
        public Skill.Framework.Sequence.Matinee Matinee;

        protected override bool OnEnter(UnityEngine.Collider other)
        {
            if (Matinee != null)
            {
                Matinee.Play();
                return true;
            }
            return base.OnEnter(other);

        }

    }
}
