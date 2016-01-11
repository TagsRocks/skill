using UnityEngine;
using System.Collections;
using Skill.Framework;


namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    [ExecuteInEditMode]
    public class DynamicTextFontSize : MonoBehaviour
    {
        [System.Serializable]
        public class LanguageOverride
        {
            public string Language;
            public float ScaleFactor;
        }

        public float ScreenFactor = 0.2f;
        public float ScreenWidthFactor = 1.0f;
        public float ScreenHeightFactor = 1.0f;
        public LanguageOverride[] Overrides;

        private UnityEngine.UI.Text _Text;
        private ScreenSizeChange _ScreenSizeChange;
        private float _PreScreenWidthFactor;
        private float _PreScreenHeightFactor;
        private float _PreScreenFactor;
        void Awake()
        {
            _Text = GetComponent<UnityEngine.UI.Text>();
            if (_Text == null)
                throw new MissingComponentException("DynamicTextFontSize needs a UnityEngine.UI.Text component");
        }

        // Update is called once per frame
        void Update()
        {
            if (_ScreenSizeChange.IsChanged ||
                _PreScreenFactor != ScreenFactor ||
                _PreScreenWidthFactor != ScreenWidthFactor ||
                _PreScreenHeightFactor != ScreenHeightFactor)
            {
                _PreScreenFactor = ScreenFactor;
                _PreScreenWidthFactor = ScreenWidthFactor;
                _PreScreenHeightFactor = ScreenHeightFactor;
                if (_Text == null)
                    _Text = GetComponent<UnityEngine.UI.Text>();
                if (_Text != null)
                {
                    float factor = ((Screen.width * ScreenWidthFactor) + (Screen.height * ScreenHeightFactor)) * 0.1f;
                    if (Overrides != null && Overrides.Length > 0)
                    {
                        if (Localization.Instance != null)
                        {
                            foreach (var o in Overrides)
                            {
                                if (Localization.Instance.SelectedLanguage.Name == o.Language)
                                {
                                    factor *= o.ScaleFactor;
                                    break;
                                }
                            }
                        }
                    }
                    _Text.fontSize = Mathf.FloorToInt(factor * ScreenFactor);
                }
            }
        }
    }
}
