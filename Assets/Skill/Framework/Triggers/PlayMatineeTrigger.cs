using System;
using System.Collections.Generic;
using System.Text;

namespace Skill.Framework.Triggers
{
    public class PlayMatineeTrigger : Trigger
    {
        public Skill.Framework.Sequence.Matinee Matinee;
        public bool Seek = false;
        public float SeekTime = 0;
        /// <summary> Delay before active or deactive objects </summary>
        public float Delay = 0.0f;


        private TimeWatch _DelayTW;

        protected override bool OnEnter(UnityEngine.Collider other)
        {
            _DelayTW.Begin(Delay);
            enabled = true;
            return true;
        }


        /// <summary>
        /// Update
        /// </summary>
        protected virtual void Update()
        {
            if (_DelayTW.IsEnabledAndOver)
            {
                _DelayTW.End();
                if (Matinee != null)
                {
                    if (Seek)
                        Matinee.Seek(SeekTime);
                    Matinee.Play();                    
                }
            }
        }
    }
}
