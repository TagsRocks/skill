using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    /// <summary>
    /// Object vill be visible at specific time after instantiate
    /// </summary>    
    public class DelayRender : DynamicBehaviour
    {
        /// <summary> Delay time </summary>
        public float Delay = 0.05f;

        private TimeWatch _StartTW;

        /// <summary>
        /// OnEnable
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _StartTW.Begin(Delay);
            if (this.renderer != null)
                this.renderer.enabled = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (_StartTW.IsEnabledAndOver)
            {
                if (this.renderer != null)
                    this.renderer.enabled = true;
                enabled = false;
                _StartTW.End();
            }
            base.Update();
        } 
    }

}