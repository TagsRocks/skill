using UnityEngine;
using System.Collections;
namespace Skill.Framework.Audio
{
    public abstract class SubtitleRenderer : Skill.Framework.DynamicBehaviour
    {
        public abstract void ShowTitle(string text, float duration, Color color, FontStyle style, TextAlignment alignment);
    }
}