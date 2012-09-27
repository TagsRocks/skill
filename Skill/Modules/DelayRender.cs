using UnityEngine;
using System.Collections;

namespace Skill.Modules
{
    /// <summary>
    /// Object vill be visible at specific time after instantiate
    /// </summary>
    public class DelayRender : MonoBehaviour
    {
        /// <summary> Delay time </summary>
        public float Delay = 0.05f;

        private Skill.TimeWatch _StartTW;

        /// <summary>
        /// OnEnable
        /// </summary>
        protected void OnEnable()
        {
            _StartTW.Begin(Delay);
            if (this.renderer != null)
                this.renderer.enabled = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected void Update()
        {
            if (_StartTW.EnabledAndOver)
            {
                if (this.renderer != null)
                    this.renderer.enabled = true;
                enabled = false;
                _StartTW.End();
            }
        }
    }

}