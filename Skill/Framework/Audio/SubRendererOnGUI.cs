using UnityEngine;
using System.Collections;
namespace Skill.Framework.Audio
{
    public class SubRendererOnGUI : SubtitleRenderer
    {
        public int GUIDepth;
        public Font Font;
        public int FontSize;
        public RectOffset ScreenBorder;

        private Skill.Framework.TimeWatch _ShowTW;
        private string _Text;
        private GUIStyle _Style;
        protected override void Awake()
        {
            base.Awake();
            useGUILayout = false;
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
            this._Style.fontSize = FontSize;

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