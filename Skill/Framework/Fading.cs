using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework
{
    /// <summary> Fade in/out  alpha between 0.0f - 1.0f </summary>
    public class Fading : DynamicBehaviour
    {
        /// <summary> Time to FadeIn  </summary>
        public float FadeInTime = 2;
        /// <summary> Time to FadeOut  </summary>
        public float FadeOutTime = 2;
        /// <summary> Use SmoothStep or linear </summary>
        public bool SmoothStep = false;
        /// <summary> FadeIn on awake  </summary>
        public bool FadeInOnAwake = false;

        /// <summary> Is in fadein mode  </summary>
        public bool IsFadeIn { get; private set; }
        /// <summary>  Is in fadeout mode </summary>
        public bool IsFadeOut { get; private set; }
        /// <summary> Current alpha between (0.0f - 1.0f)  </summary>
        public float Alpha { get { return _Alpha; } set { _Alpha = Mathf.Clamp01(value); } }

        private float _FadeStartTime;
        private bool _FadeInAfterFadeOut;
        private float _Alpha = 1.0f;
        private float _OffsetTime;
        private float _Speed;

        private const float EpsilonAlpha = 0.001f;
        private const float EpsilonTime = 0.01f;

        /// <summary> Awake </summary>
        protected override void Awake()
        {
            base.Awake();
            _OffsetTime = 0;
            if (FadeInOnAwake) FadeIn();
        }

        /// <summary> Fadeout and fadein  </summary>
        public void Fade()
        {
            FadeOut();
            _FadeInAfterFadeOut = true;
        }

        /// <summary> FadeIn  </summary>
        /// <param name="resetAlpha"> Reset alpha or continue from last alpha value </param>
        public void FadeIn(bool resetAlpha = false)
        {
            if (resetAlpha) _Alpha = 1.0f;
            if (FadeInTime < EpsilonTime) FadeInTime = EpsilonTime;
            _OffsetTime = FadeInTime * (1.0f - _Alpha);
            _Speed = 1.0f / FadeInTime;
            _FadeStartTime = Time.time;
            IsFadeIn = true;
            IsFadeOut = false;
            enabled = true;
        }
        /// <summary>  FadeOut </summary>
        /// <param name="resetAlpha"> Reset alpha or continue from last alpha value </param>
        public void FadeOut(bool resetAlpha = false)
        {
            if (resetAlpha) _Alpha = 0.0f;
            if (FadeOutTime < EpsilonTime) FadeOutTime = EpsilonTime;
            _OffsetTime = FadeOutTime * _Alpha;
            _Speed = 1.0f / FadeOutTime;
            _FadeStartTime = Time.time;
            IsFadeIn = false;
            IsFadeOut = true;
            enabled = true;
        }

        /// <summary> Apply alpha channel to color </summary>
        /// <param name="color">Color to apply alpha</param>
        /// <returns>Color</returns>
        public Color ApplyAlpha(Color color)
        {
            color.a = _Alpha;
            return color;
        }

        /// <summary> Apply alpha channel to color </summary>
        /// <param name="color">Color to apply alpha</param>        
        public void ApplyAlpha(ref Color color)
        {
            color.a = _Alpha;
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (IsFadeOut)
            {
                if (SmoothStep)
                    _Alpha = Mathf.SmoothStep(0, 1, (Time.time + _OffsetTime - _FadeStartTime) / FadeOutTime);
                else
                    _Alpha += Time.deltaTime * _Speed;

                if (_Alpha > 1 - EpsilonAlpha)
                {
                    IsFadeOut = false;
                    _Alpha = 1.0f;
                    if (_FadeInAfterFadeOut)
                    {
                        FadeIn();
                        _FadeInAfterFadeOut = false;
                    }
                    else
                        enabled = false;
                }
            }
            else if (IsFadeIn)
            {
                if (SmoothStep)
                    _Alpha = Mathf.SmoothStep(1, 0, (Time.time + _OffsetTime - _FadeStartTime) / FadeInTime);
                else
                    _Alpha -= Time.deltaTime * _Speed;
                if (_Alpha < EpsilonAlpha)
                {
                    IsFadeIn = false;
                    _Alpha = 0;
                    enabled = false;
                }
            }
            base.Update();
        }
    }
}
