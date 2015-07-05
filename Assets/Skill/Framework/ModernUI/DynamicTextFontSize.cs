using UnityEngine;
using System.Collections;
using Skill.Framework;


namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    [ExecuteInEditMode]
    public class DynamicTextFontSize : MonoBehaviour
    {
        public float ScreenFactor = 0.2f;
        public float ScreenWidthFactor = 1.0f;
        public float ScreenHeightFactor = 1.0f;

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
                    _Text.fontSize = Mathf.FloorToInt(factor * ScreenFactor);
                }
            }
        }
    }
}
