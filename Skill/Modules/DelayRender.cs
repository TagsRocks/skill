using UnityEngine;
using System.Collections;

namespace Skill.Modules
{
    /// <summary>
    /// Object vill be visible after delay time
    /// </summary>
    public class DelayRender : MonoBehaviour
    {
        public float Delay = 0.05f;
        private Skill.TimeWatch _StartTW;

        void OnEnable()
        {
            _StartTW.Begin(Delay);
            if (this.renderer != null)
                this.renderer.enabled = false;
        }

        void Update()
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