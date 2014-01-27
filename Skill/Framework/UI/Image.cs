using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Draw a texture within a rectangle.
    /// </summary>
    public class Image : Control
    {
        /// <summary>
        /// Texture to display.
        /// </summary>
        public Texture Texture { get; set; }
        /// <summary>
        /// How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.
        /// </summary>
        public ScaleMode Scale { get; set; }
        /// <summary>
        /// Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.
        /// </summary>
        public bool AlphaBlend { get; set; }
        /// <summary>
        /// Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.
        /// Pass in w/h for the desired aspect ratio.
        /// This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.
        /// </summary>
        public float ImageAspect { get; set; }

        /// <summary>
        /// Getsor sets tinting color
        /// </summary>
        public Color TintColor { get; set; }

        /// <summary>
        /// If not null Image use alpha value of referenced Fading
        /// </summary>
        public Fading AlphaFading { get; set; }

        /// <summary>
        /// Create an instance of Image
        /// </summary>
        public Image()
        {
            this.AlphaBlend = true;
            this.Scale = ScaleMode.ScaleToFit;
            this.ImageAspect = 0;
            this.TintColor = Color.white;
        }

        /// <summary>
        /// Render Image
        /// </summary>
        protected override void Render()
        {
            if (Texture != null)
            {
                Color preCoor = GUI.color;
                if (AlphaFading != null)
                    GUI.color = AlphaFading.ApplyAlpha(TintColor);
                else
                    GUI.color = TintColor;
                GUI.DrawTexture(RenderArea, Texture, Scale, AlphaBlend, ImageAspect);

                GUI.color = preCoor;
            }
        }

    }

}