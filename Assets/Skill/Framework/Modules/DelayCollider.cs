using UnityEngine;
using System.Collections;

namespace Skill.Framework.Modules
{
    /// <summary>
    /// Object will be enabled at specific time after instantiate
    /// </summary>    
    public class DelayCollider : DynamicBehaviour
    {
        /// <summary> Delay time </summary>
        public float Delay = 0.05f;

        private TimeWatch _StartTW;
        private Collider _Collider;

        protected override void GetReferences()
        {
            base.GetReferences();
            _Collider = GetComponent<Collider>();
        }
        /// <summary>
        /// OnEnable
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Reschedule();
        }

        public void Reschedule()
        {
            _StartTW.Begin(Delay);
            if (this._Collider != null)
                this._Collider.enabled = false;
            enabled = true;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (_StartTW.IsEnabledAndOver)
            {
                if (this._Collider != null)
                    this._Collider.enabled = true;                
                _StartTW.End();
            }
            base.Update();
        }
    }

}