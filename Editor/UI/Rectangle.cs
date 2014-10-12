using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Skill.Editor.UI
{
    /// <summary>
    /// Draw a fill rectangle
    /// </summary>
    public class Rectangle : EditorControl
    {
        public Color Color { get; set; }

        /// <summary>
        /// Create an instance of TextureImage
        /// </summary>
        public Rectangle()
        {
            this.Color = new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// Render Image
        /// </summary>
        protected override void Render()
        {
            UnityEditor.EditorGUI.DrawRect(RenderArea, Color);
        }

    }

}
