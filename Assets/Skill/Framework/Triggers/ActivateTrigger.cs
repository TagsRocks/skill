using UnityEngine;
using System.Collections;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Enable or desable a spawner on trigger enter
    /// </summary>    
    public class ActivateTrigger : Trigger
    {
        /// <summary> Objects </summary>
        public GameObject[] Objects;
        /// <summary> Active or deactive objects</summary>
        public bool Active = true;
        /// <summary> Delay before active or deactive objects </summary>
        public float Delay = 0.0f;

        private TimeWatch _DelayTW;

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        /// <returns>True if event handled, otherwise false</returns>
        protected override bool OnEnter(Collider other)
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
                if (Objects != null)
                {
                    foreach (var item in Objects)
                    {
                        if (item != null)
                        {
                            item.SetActive(Active);
                        }
                    }
                }
            }
        }        
    }
}