using UnityEngine;
using System.Collections;

namespace Skill.Triggers
{
    /// <summary>
    /// Enable or desable a spawner on trigger enter
    /// </summary>
    [AddComponentMenu("Skill/Triggers/Spawner")]
    public class SpawnerTrigger : Trigger
    {
        /// <summary> Spawners </summary>
        public Skill.Spawner[] Spawners;
        /// <summary> Enable spawners or disable spawners </summary>
        public bool Enable = true;
        /// <summary> Delay before enable or disable spawners </summary>
        public float Delay = 0.0f;

        private Skill.TimeWatch _DelayTW;

        /// <summary>
        /// called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">other Collider</param>
        protected override void OnEnter(Collider other)
        {
            _DelayTW.Begin(Delay);
            enabled = true;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected virtual void Update()
        {
            if (_DelayTW.EnabledAndOver)
            {
                _DelayTW.End();
                if (Spawners != null)
                {
                    foreach (var item in Spawners)
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
        protected override string GizmoFilename { get { return "Spawner.png"; } }
    }
}