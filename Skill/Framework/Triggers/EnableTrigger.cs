using UnityEngine;
using System.Collections;

namespace Skill.Framework.Triggers
{
    /// <summary>
    /// Enable or desable a spawner on trigger enter
    /// </summary>    
    public class EnableTrigger : Trigger
    {
        /// <summary> Spawners </summary>
        public MonoBehaviour[] Behaviours;
        /// <summary> Enable spawners or disable spawners </summary>
        public bool Enable = true;
        /// <summary> Delay before enable or disable spawners </summary>
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
                if (Behaviours != null)
                {
                    foreach (var item in Behaviours)
                    {
                        if (item != null)
                        {
                            item.enabled = Enable;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Name of file in Gizmos folder
        /// </summary>
        protected override string GizmoFilename { get { return "Enable.png"; } }
    }
}