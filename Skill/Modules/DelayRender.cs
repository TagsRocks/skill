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
        private float _SpawnTime = 0.0f;

        void OnEnable()
        {
            _SpawnTime = Time.time;
            this.renderer.enabled = false;
        }

        void Update()
        {
            if (this.renderer.enabled) return;
            if (Time.time > (_SpawnTime + Delay))
                this.renderer.enabled = true;
        }
    }

}