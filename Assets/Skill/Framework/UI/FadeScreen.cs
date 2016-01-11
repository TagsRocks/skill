using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Draw a texture on entire screen
    /// </summary>    
    [RequireComponent(typeof(Fading))]
    public class FadeScreen : StaticBehaviour
    {
        /// <summary> Texture to draw on screen- usually a black texture </summary>
        public Texture2D FadeTexture;
        /// <summary> TintColor of texture </summary>
        public Color TintColor = Color.white;
        /// <summary> The sorting depth. </summary>
        public int GUIDepth = 11;

        /// <summary> Fading  component </summary>
        public Fading Fading { get; private set; }

        /// <summary> Get references </summary>
        protected override void GetReferences()
        {
            base.GetReferences();
            this.Fading = GetComponent<Fading>();            
        }

        void OnGUI()
        {
            if (FadeTexture != null && Fading != null && Fading.Alpha > 0.001f)
            {
                GUI.depth = GUIDepth;
                Color preColor = GUI.color;
                GUI.color = Fading.ApplyAlpha(TintColor);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture, ScaleMode.StretchToFill, true);
                GUI.color = preColor;
            }
        }
    }
}
