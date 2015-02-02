using UnityEngine;
using System.Collections;

namespace Skill.Framework.Audio
{
    [RequireComponent(typeof(GUIText))]
    public class SubRendererGUIText : SubtitleRenderer
    {
        private Skill.Framework.TimeWatch _ShowTW;
        private GUIText _GUIText;

        protected override void GetReferences()
        {
            base.GetReferences();
            _GUIText = guiText;
        }

        public override void ShowTitle(string text, float duration, Color color, FontStyle style, TextAlignment alignment)
        {
            if (_GUIText == null) return;
            if (string.IsNullOrEmpty(text)) return;

            this._GUIText.text = text;
            this._GUIText.color = color;
            this._GUIText.fontStyle = style;
            this._GUIText.alignment = alignment;
            _GUIText.enabled = true;
            _ShowTW.Begin(duration);
        }

        protected override void Update()
        {
            if (Skill.Framework.Global.IsGamePaused) return;
            if (_ShowTW.IsEnabled)
            {
                if (_ShowTW.IsOver)
                {
                    _ShowTW.End();
                    if (_GUIText != null)
                    {
                        _GUIText.text = string.Empty;
                        _GUIText.enabled = false;
                    }
                }
            }
            base.Update();
        }

    }
}
