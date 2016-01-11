using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.Modules
{
    /// <summary>
    /// Attach this to a GUIText or UnityEngine.UI.Text to make a frames/second indicator.   
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
    public class ShowFPS : Skill.Framework.DynamicBehaviour
    {
        /// <summary> </summary>
        public float Frequency = 0.5F;
        /// <summary> Format of fps text </summary>
        public string Format = "FPS {0:F2}";
        /// <summary> Change color depending of fps </summary>
        public bool MaterialColor = true;
        /// <summary> Use coroutine for high precision</summary>
        public bool Coroutine = true;

        private GUIText _GUIText;
        private UnityEngine.UI.Text _UIText;
        private TimeWatch _UpdateTW;
        private int _LastFrameCount;
        private float _LastTime;

        public int FPS { get; private set; }

        /// <summary>
        /// Get required references
        /// </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            _GUIText = GetComponent<GUIText>();
            _UIText = GetComponent<UnityEngine.UI.Text>();
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _LastFrameCount = Time.frameCount;
            _LastTime = Time.realtimeSinceStartup;
            if (Coroutine)
                StartCoroutine(FPSCoroutine());
            else
                _UpdateTW.Begin(Frequency);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (_UpdateTW.IsEnabledAndOver)
            {
                float timeSpan = Time.realtimeSinceStartup - _LastTime;
                int frameCount = Time.frameCount - _LastFrameCount;

                FPS = Mathf.RoundToInt(frameCount / timeSpan);

                _UpdateTW.Begin(Frequency);
                _LastFrameCount = Time.frameCount;
                _LastTime = Time.realtimeSinceStartup;


                UpdateUI();
            }
        }

        private System.Collections.IEnumerator FPSCoroutine()
        {
            for (; ; )
            {
                // Capture frame-per-second
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds(Frequency);
                float timeSpan = Time.realtimeSinceStartup - lastTime;
                int frameCount = Time.frameCount - lastFrameCount;

                FPS = Mathf.RoundToInt(frameCount / timeSpan);

                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            string txt = System.String.Format(Format, FPS);

            if (_GUIText != null) _GUIText.text = txt;
            if (_UIText != null) _UIText.text = txt;


            if (MaterialColor)
            {
                Color color = Color.green;
                if (FPS < 10)
                    color = Color.red;
                else if (FPS < 30)
                    color = Color.yellow;

                if (_GUIText != null && _GUIText.material != null)
                    _GUIText.material.color = color;
                if (_UIText != null)
                    _UIText.color = color;
            }
        }
    }
}
