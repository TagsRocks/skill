using UnityEngine;
using System.Collections;
using System;

namespace Skill.UI
{
    /// <summary>
    /// Draw a wrapped texture
    /// </summary>
    public class WrapImage : Control
    {
        /// <summary> Texture to display. </summary>
        public Texture Texture { get; set; }
        /// <summary> Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display. </summary>
        public bool AlphaBlend { get; set; }
        /// <summary> Enable or Disable Wrap U</summary>
        public bool WrapU { get; set; }
        /// <summary> Enable or Disable Wrap V</summary>
        public bool WrapV { get; set; }

        /// <summary>
        /// Create an instance of WrapImage
        /// </summary>
        public WrapImage()
        {
            this.AlphaBlend = true;
            this.WrapU = true;
            this.WrapV = true;
        }

        /// <summary> Render Image </summary>
        protected override void Render()
        {
            if (Texture != null)
            {
                Rect renderArea = RenderArea; // copy RenderArea to access faster

                float width = WrapU ? Texture.width : renderArea.width;      // if WrapU is disable, scale with to entire RenderArea.width
                float height = WrapV ? Texture.height : renderArea.height;   // if WrapV is disable, scale height to entire RenderArea.height

                Rect drawRect = renderArea;  // this rect traverse through RenderArea until entire RenderArea drawn
                drawRect.width = width;     // begin by top left corner
                drawRect.height = height;   // begin by top left corner

                Rect texRect = new Rect();  // texture coordinate 
                while (drawRect.x < renderArea.xMax) // continue drawing until x exit RenderArea
                {
                    // set to entire texture
                    texRect.width = 1.0f;
                    drawRect.width = width;
                    if (drawRect.xMax > renderArea.xMax) // if right of drawRect exit RenderArea
                    {
                        // corp texture to fit inside drawRect
                        drawRect.width = width - (drawRect.xMax - renderArea.xMax);
                        texRect.width = drawRect.width / width;

                    }
                    while (drawRect.y < renderArea.yMax) // continue drawing until y exit RenderArea
                    {
                        // set to entire texture
                        texRect.height = 1.0f;
                        drawRect.height = height;
                        if (drawRect.yMax > renderArea.yMax) // if bottom of drawRect exit RenderArea
                        {
                            // corp texture to fit inside drawRect
                            drawRect.height = height - (drawRect.yMax - renderArea.yMax);
                            texRect.height = drawRect.height / height;

                        }
                        texRect.x = 0;
                        texRect.y = 1.0f - texRect.height; // in unity origin of texture is left bottom corner of texture ... !!! ???
                        GUI.DrawTextureWithTexCoords(drawRect, Texture, texRect, AlphaBlend); // draw texture
                        drawRect.y += height; // advance height
                    }
                    drawRect.y = renderArea.y; // set to top of renderArea
                    drawRect.x += width; // advance width
                }
            }
        }
    }

}