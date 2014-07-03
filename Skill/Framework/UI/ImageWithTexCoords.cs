using UnityEngine;
using System.Collections;
using System;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Draw a texture within a rectangle.
    /// </summary>
    public class ImageWithTexCoords : Control, IImage
    {
        /// <summary>
        /// Texture to display.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.
        /// </summary>
        public bool AlphaBlend { get; set; }

        private Rect _TextureCoordinate;
        /// <summary>
        /// Draw a texture within a rectangle with the given texture coordinates.
        /// </summary>
        public Rect TextureCoordinate { get { return _TextureCoordinate; } set { _TextureCoordinate = value; } }

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
        public ImageWithTexCoords()
        {
            this.TextureCoordinate = new Rect(0, 0, 1, 1);
            this.AlphaBlend = true;
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
                GUI.DrawTextureWithTexCoords(RenderArea, Texture, _TextureCoordinate, AlphaBlend);

                GUI.color = preCoor;
            }
        }

    }

}