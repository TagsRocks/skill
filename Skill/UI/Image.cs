using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
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
        /// Draw a texture within a rectangle with the given texture coordinates.
        /// </summary>
        public Rect TextureCoordinate { get; set; }
        /// <summary>
        /// Whether use coordinate. Use this function for clipping or tiling the image within the given rectangle. 
        /// </summary>
        public bool UseTextureCoordinate { get; set; }

        /// <summary>
        /// Create an instance of Image
        /// </summary>
        public Image()
        {
            this.UseTextureCoordinate = false;
            this.AlphaBlend = true;
            this.Scale = ScaleMode.ScaleToFit;
            this.ImageAspect = 0;
        }

        /// <summary>
        /// Render Image
        /// </summary>
        protected override void Render()
        {
            if (Texture != null)
            {
                //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
                if (UseTextureCoordinate)
                    GUI.DrawTextureWithTexCoords(RenderArea, Texture, TextureCoordinate, AlphaBlend);
                else
                    GUI.DrawTexture(RenderArea, Texture, Scale, AlphaBlend, ImageAspect);
            }
        }

    }

}