using System;
using System.Collections.Generic;
using Skill.UI;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Drawsa texture within a rectangle.
    /// </summary>
    public class TextureImage : Control
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
        /// Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.
        /// </summary>
        public float ImageAspect { get; set; }
        /// <summary>
        /// Material to be used when drawing the texture.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Draws the alpha channel of a texture within a rectangle.
        /// </summary>
        /// <remarks>
        /// if true :  DrawTextureAlpha
        /// if false : DrawPreviewTexture with Material
        /// </remarks>
        public bool Alpha { get; set; }

        /// <summary>
        /// Create an instance of TextureImage
        /// </summary>
        public TextureImage()
        {
            this.Alpha = false;
            this.Scale = ScaleMode.ScaleToFit;
            this.ImageAspect = 0;
        }

        protected override void Paint()
        {
            if (Texture != null)
            {
                //if (!string.IsNullOrEmpty(Name)) GUI.SetNextControlName(Name);
                if (Alpha)
                    EditorGUI.DrawTextureAlpha(PaintArea, Texture, Scale, ImageAspect);
                else
                    EditorGUI.DrawPreviewTexture(PaintArea, Texture, Material, Scale, ImageAspect);
            }
        }

    }

}
