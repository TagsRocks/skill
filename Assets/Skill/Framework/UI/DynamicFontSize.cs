using UnityEngine;
using System.Collections;
using Skill.Framework;
using System.Collections.Generic;

namespace Skill.Framework.UI
{
    public class DynamicFontSize
    {
        class StyleTag
        {
            public GUIStyle Style;
            public float Factor;
        }

        private List<StyleTag> _Styles;
        private ScreenSizeChange _ScreenSizeChange;
        private bool _Changed;

        private float _ScreenFactor;
        public float ScreenFactor { get { return _ScreenFactor; } set { _ScreenFactor = value; _Changed = true; } }

        private float _ScreenWidthFactor;
        public float ScreenWidthFactor { get { return _ScreenWidthFactor; } set { _ScreenWidthFactor = value; _Changed = true; } }

        private float _ScreenHeightFactor;
        public float ScreenHeightFactor { get { return _ScreenHeightFactor; } set { _ScreenHeightFactor = value; _Changed = true; } }


        public float Factor { get; private set; }

        public event System.EventHandler Changed;
        private void OnChanged()
        {
            if (Changed != null) Changed(this, System.EventArgs.Empty);
        }

        public DynamicFontSize()
        {
            _Styles = new List<StyleTag>();
            ScreenFactor = 0.2f;
            ScreenWidthFactor = 1.0f;
            ScreenHeightFactor = 1.0f;
            _Changed = true;
        }

        public void Add(GUIStyle style, float fontSizeFactor)
        {
            _Styles.Add(new StyleTag() { Style = style, Factor = fontSizeFactor });
        }

        public void Update()
        {
            if (_ScreenSizeChange.IsChanged || _Changed)
            {
                ForceUpdate();
                OnChanged();
                _Changed = false;
            }
        }
        public void ForceUpdate()
        {
            Factor = ((Screen.width * ScreenWidthFactor) + (Screen.height * ScreenHeightFactor)) * 0.1f;

            if (_Styles.Count > 0)
            {
                foreach (var s in _Styles)
                    s.Style.fontSize = Mathf.FloorToInt(Factor * s.Factor);
            }
        }

        public void Remove(GUIStyle style)
        {
            for (int i = 0; i < _Styles.Count; i++)
            {
                if (_Styles[i].Style == style)
                {
                    _Styles.RemoveAt(i);
                    break;
                }
            }
        }

        public void Clear()
        {
            _Styles.Clear();
        }
    }
}
