using UnityEngine;
using System.Collections;
namespace Skill.Framework.Audio
{
    public abstract class SubtitleRenderer : Skill.Framework.DynamicBehaviour
    {
        public Color FontColor = Color.white;
        public FontStyle FontStyle = FontStyle.Normal;
        public TextAlignment Alignment = TextAlignment.Center;
        
        public void ShowTitle(string text, float duration)
        {
            ShowTitle(text, duration, this.FontColor, this.FontStyle, this.Alignment);
        }

        public abstract void ShowTitle(string text, float duration, Color color, FontStyle style, TextAlignment alignment);
    }
}