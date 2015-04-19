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

        public float Factor { get; private set; }


        public event System.EventHandler Changed;
        private void OnChanged()
        {
            if (Changed != null) Changed(this, System.EventArgs.Empty);
        }

        public DynamicFontSize()
        {
            _Styles = new List<StyleTag>();
            Factor = 1.0f;
        }

        public void Add(GUIStyle style, float fontSizeFactor)
        {
            _Styles.Add(new StyleTag() { Style = style, Factor = fontSizeFactor });
        }

        public void Update()
        {
            if (_ScreenSizeChange.IsChanged)
            {                
                ForceUpdate();
                OnChanged();
            }
        }
        public void ForceUpdate()
        {
            if (_Styles.Count > 0)
            {
                Factor = (Screen.width + Screen.height) * 0.1f;
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
