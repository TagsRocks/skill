using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Modules
{
    /// <summary>
    /// Attach this to a GUIText to make a frames/second indicator.   
    /// </summary>
    /// <remarks>
    /// It calculates frames/second over each updateInterval,
    /// so the display does not keep changing wildly.
    ///
    /// It is also fairly accurate at very low FPS counts (less than 10).
    /// We do this not by simply counting frames per interval, but
    /// by accumulating FPS for each frame. This way we end up with
    /// correct overall FPS even if the interval renders something like
    /// 5.5 frames.
    /// </remarks>
    [RequireComponent(typeof(GUIText))]
    public class ShowFPS : Skill.Framework.DynamicBehaviour
    {
        /// <summary> </summary>
        public float UpdateInterval = 0.5F;
        /// <summary> Format of fps text </summary>
        public string Format = "{0:F2} FPS";
        /// <summary> Change material.color depending of fps </summary>
        public bool MaterialColor = true;

        private GUIText _Text;
        private float _Accum = 0; // FPS accumulated over the interval
        private int _Frames = 0; // Frames drawn over the interval
        private float _Timeleft; // Left time for current interval

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _Text = GetComponent<GUIText>();
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _Timeleft = UpdateInterval;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (_Text != null)
            {
                _Timeleft -= Time.deltaTime;
                _Accum += Time.timeScale / Time.deltaTime;
                ++_Frames;

                // Interval ended - update GUI text and start new interval
                if (_Timeleft <= 0.0)
                {
                    float fps = _Accum / _Frames;
                    _Text.text = System.String.Format(Format, fps);
                    if (MaterialColor)
                    {
                        if (fps < 10)
                            _Text.material.color = Color.red;
                        else if (fps < 30)
                            _Text.material.color = Color.yellow;
                        else
                            _Text.material.color = Color.green;
                    }
                    _Timeleft = UpdateInterval;
                    _Accum = 0.0F;
                    _Frames = 0;
                }
            }
        }

    }
}
