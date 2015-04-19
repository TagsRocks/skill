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
        private Renderer _Renderer;

        protected override void GetReferences()
        {
            base.GetReferences();
            _Renderer = GetComponent<Renderer>();
        }
        /// <summary>
        /// OnEnable
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _StartTW.Begin(Delay);
            if (this._Renderer != null)
                this._Renderer.enabled = false;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (_StartTW.IsEnabledAndOver)
            {
                if (this._Renderer != null)
                    this._Renderer.enabled = true;
                enabled = false;
                _StartTW.End();
            }
            base.Update();
        } 
    }

}