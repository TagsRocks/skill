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
        /// <summary> Optional image to apply alpha </summary>
        public UnityEngine.UI.Graphic UIElement;

        /// <summary> Is in fadein mode  </summary>
        public bool IsFadeIn { get; private set; }
        /// <summary>  Is in fadeout mode </summary>
        public bool IsFadeOut { get; private set; }
        /// <summary> Is in fadein or fadeout  </summary>
        public bool IsFading { get { return IsFadeIn || IsFadeOut; } }

        /// <summary> Current alpha between (0.0f - 1.0f)  </summary>
        public float Alpha
        {
            get { return _Alpha; }
            set
            {
                _Alpha = Mathf.Clamp01(value);
                if (UIElement != null)
                    UIElement.color = ApplyAlpha(UIElement.color);
            }
        }

        private float _FadeStartTime;
        private bool _FadeInAfterFadeOut;
        private float _Alpha = 1.0f;
        private float _OffsetTime;
        private float _Speed;
        private float _DestAlpha;

        private const float EpsilonAlpha = 0.001f;
        private const float EpsilonTime = 0.01f;

        /// <summary> Awake </summary>
        protected override void Awake()
        {
            base.Awake();
            _OffsetTime = 0;
            Alpha = 1.0f;
            if (FadeInOnAwake) FadeToZero(true);
        }

        /// <summary> Fadeout and fadein  </summary>
        public void Fade()
        {
            FadeToOne();
            _FadeInAfterFadeOut = true;
        }

        /// <summary> FadeIn (to 0.0f)  </summary>
        /// <param name="resetAlpha"> Reset alpha or continue from last alpha value </param>
        public void FadeToZero(bool resetAlpha = false)
        {
            if (resetAlpha) Alpha = 1.0f;
            FadeInTo(0.0f);
        }
        /// <summary>  FadeOut (to 1.0f)</summary>
        /// <param name="resetAlpha"> Reset alpha or continue from last alpha value </param>
        public void FadeToOne(bool resetAlpha = false)
        {
            if (resetAlpha) Alpha = 0.0f;
            FadeOutTo(1.0f);
        }

        private void FadeInTo(float destAlpha)
        {
            _DestAlpha = destAlpha;
            if (FadeInTime < EpsilonTime) FadeInTime = EpsilonTime;
            _OffsetTime = FadeInTime * (1.0f - Alpha);
            _Speed = 1.0f / FadeInTime;
            _FadeStartTime = Time.time;
            IsFadeIn = true;
            IsFadeOut = false;
            enabled = true;
        }

        private void FadeOutTo(float destAlpha)
        {
            _DestAlpha = destAlpha;
            if (FadeOutTime < EpsilonTime) FadeOutTime = EpsilonTime;
            _OffsetTime = FadeOutTime * Alpha;
            _Speed = 1.0f / FadeOutTime;
            _FadeStartTime = Time.time;
            IsFadeIn = false;
            IsFadeOut = true;
            enabled = true;
        }


        /// <summary>
        /// Fade to specified alpha
        /// </summary>
        /// <param name="alpha">Alpha value to fade to</param>
        public void FadeTo(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);
            if (Alpha < alpha) FadeOutTo(alpha);
            else if (Alpha > alpha) FadeInTo(alpha);
        }

        /// <summary> Apply alpha channel to color </summary>
        /// <param name="color">Color to apply alpha</param>
        /// <returns>Color</returns>
        public Color ApplyAlpha(Color color)
        {
            color.a = Alpha;
            return color;
        }

        /// <summary> Apply alpha channel to color </summary>
        /// <param name="color">Color to apply alpha</param>        
        public void ApplyAlpha(ref Color color)
        {
            color.a = Alpha;
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;
            if (IsFadeOut)
            {
                if (SmoothStep)
                    Alpha = Mathf.SmoothStep(0, 1, (Time.time + _OffsetTime - _FadeStartTime) / FadeOutTime);
                else
                    Alpha += Time.deltaTime * _Speed;

                if (Alpha > _DestAlpha - EpsilonAlpha)
                {
                    IsFadeOut = false;
                    Alpha = _DestAlpha;
                    if (_FadeInAfterFadeOut)
                    {
                        FadeToZero();
                        _FadeInAfterFadeOut = false;
                    }
                    else
                        enabled = false;
                }
            }
            else if (IsFadeIn)
            {
                if (SmoothStep)
                    Alpha = Mathf.SmoothStep(1, 0, (Time.time + _OffsetTime - _FadeStartTime) / FadeInTime);
                else
                    Alpha -= Time.deltaTime * _Speed;
                if (Alpha < _DestAlpha + EpsilonAlpha)
                {
                    IsFadeIn = false;
                    Alpha = _DestAlpha;
                    enabled = false;
                }
            }
            base.Update();
        }
    }
}
