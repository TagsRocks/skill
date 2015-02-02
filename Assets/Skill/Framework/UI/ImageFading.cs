using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Skill.Framework.UI
{

    /// <summary>
    /// A helper class to simulate zoom in ImageWithTexCoords
    /// </summary>
    public class ImageFading : DynamicBehaviour
    {
        class FadeData
        {
            public IImage Image;
            public float SourceAlpha;
            public float DestAlpha;
            public TimeWatch Timer;
        }

        private List<FadeData> _Fades;
        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _Fades = new List<FadeData>();
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            if (Global.IsGamePaused) return;

            if (_Fades.Count > 0)
            {
                int index = 0;
                while (index < _Fades.Count)
                {
                    FadeData fd = _Fades[index];
                    float percent = fd.Timer.Percent;
                    if (percent >= 1.0f)
                    {
                        ApplyAlpha(fd.Image, fd.DestAlpha);
                        _Fades.RemoveAt(index);
                        continue;
                    }
                    else
                    {
                        float alpha;
                        if (fd.SourceAlpha < fd.DestAlpha)
                            alpha = Mathf.Lerp(fd.SourceAlpha, fd.DestAlpha, fd.Timer.Percent);
                        else
                            alpha = Mathf.Lerp(fd.DestAlpha, fd.SourceAlpha, 1.0f - fd.Timer.Percent);
                        ApplyAlpha(fd.Image, alpha);
                    }
                    index++;
                }
            }
            base.Update();
        }


        private void ApplyAlpha(IImage image, float alpha)
        {
            if (image != null)
            {
                Color color = image.TintColor;
                color.a = alpha;
                image.TintColor = color;
            }
        }


        public void Fade(IImage image, float duration, float sourceAlpha, float destAlpha)
        {
            if (image == null) throw new System.ArgumentNullException("Invalid image");
            FadeData fd = new FadeData();
            fd.Image = image;
            fd.SourceAlpha = sourceAlpha;
            fd.DestAlpha = destAlpha;
            fd.Timer.Begin(Mathf.Max(0, duration));
            ApplyAlpha(image, sourceAlpha);
            _Fades.Add(fd);
        }

        public void FadeIn(IImage image, float duration)
        {
            Fade(image, duration, 0.0f, 1.0f);
        }

        public void FadeOut(IImage image, float duration)
        {
            Fade(image, duration, 1.0f, 0.0f);
        }
    }
}