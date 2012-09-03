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

        /// <summary> Paint Image </summary>
        protected override void Paint()
        {
            if (Texture != null)
            {
                Rect paintArea = PaintArea; // copy PaintArea to access faster

                float width = WrapU ? Texture.width : paintArea.width;      // if WrapU is disable, scale with to entire PaintArea.width
                float height = WrapV ? Texture.height : paintArea.height;   // if WrapV is disable, scale height to entire PaintArea.height

                Rect drawRect = paintArea;  // this rect traverse through PaintArea until entire PaintArea drawn
                drawRect.width = width;     // begin by top left corner
                drawRect.height = height;   // begin by top left corner

                Rect texRect = new Rect();  // texture coordinate 
                while (drawRect.x < paintArea.xMax) // continue drawing until x exit PaintArea
                {
                    // set to entire texture
                    texRect.width = 1.0f;
                    drawRect.width = width;
                    if (drawRect.xMax > paintArea.xMax) // if right of drawRect exit PaintArea
                    {
                        // corp texture to fit inside drawRect
                        drawRect.width = width - (drawRect.xMax - paintArea.xMax);
                        texRect.width = drawRect.width / width;

                    }
                    while (drawRect.y < paintArea.yMax) // continue drawing until y exit PaintArea
                    {
                        // set to entire texture
                        texRect.height = 1.0f;
                        drawRect.height = height;
                        if (drawRect.yMax > paintArea.yMax) // if bottom of drawRect exit PaintArea
                        {
                            // corp texture to fit inside drawRect
                            drawRect.height = height - (drawRect.yMax - paintArea.yMax);
                            texRect.height = drawRect.height / height;

                        }

                        GUI.DrawTextureWithTexCoords(drawRect, Texture, texRect, AlphaBlend); // draw texture
                        drawRect.y += height; // advance height
                    }
                    drawRect.y = paintArea.y; // set to top of paintArea
                    drawRect.x += width; // advance width
                }
            }
        }        
    }

}