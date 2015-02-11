using UnityEngine;
using System.Collections;
namespace Skill.Framework.Audio
{
    public class SubRendererOnGUI : SubtitleRenderer
    {
        public int GUIDepth;
        public Font Font;        
        public float FontSizeFactor = 0.24f;
        public RectOffset ScreenBorder;

        private Skill.Framework.TimeWatch _ShowTW;
        private string _Text;
        private GUIStyle _Style;
        private Skill.Framework.ScreenSizeChange _ScreenSizeChange;
        private int _FontSize;
        protected override void Awake()
        {
            base.Awake();
            _Style = new GUIStyle();
        }


        public override void ShowTitle(string text, float duration, Color color, FontStyle fontStyle, TextAlignment alignment)
        {
            if (string.IsNullOrEmpty(text)) return;

            this._Text = text;
            this._Style.normal.textColor = color;
            this._Style.fontStyle = fontStyle;

            if (alignment == TextAlignment.Center) this._Style.alignment = TextAnchor.LowerCenter;
            else if (alignment == TextAlignment.Left) this._Style.alignment = TextAnchor.LowerLeft;
            else this._Style.alignment = TextAnchor.LowerRight;

            this._Style.font = Font;

            if (_ScreenSizeChange.IsChanged)
            {
                float factor = (_ScreenSizeChange.Width + _ScreenSizeChange.Height) * 0.1f;
                _FontSize = Mathf.FloorToInt(factor * FontSizeFactor);
            }
            this._Style.fontSize = _FontSize;

            _ShowTW.Begin(duration);
        }

        void OnGUI()
        {
            if (_ShowTW.IsEnabledButNotOver)
            {
                GUI.depth = GUIDepth;
                Rect rect = new Rect(ScreenBorder.left,
                                     ScreenBorder.top,
                                     Screen.width - (ScreenBorder.left + ScreenBorder.right),
                                     Screen.height - (ScreenBorder.top + ScreenBorder.bottom));
                GUI.Label(rect, _Text, _Style);
            }
        }

    }
}