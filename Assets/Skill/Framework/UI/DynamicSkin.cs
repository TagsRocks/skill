using UnityEngine;
using System.Collections;

namespace Skill.Framework
{
    public class DynamicSkin : MonoBehaviour
    {
        public GUISkin Skin;

        public float BoxFontSize = 0.1f;
        public float ButtonFontSize = 0.1f;
        public float ToggleFontSize = 0.1f;
        public float LabelFontSize = 0.1f;
        public float TextFieldFontSize = 0.1f;
        public float TextAreaFontSize = 0.1f;

        public Vector2 HorizontalSliderThumbSize = new Vector2(0.2f, 0.2f);
        public Vector2 VerticalSliderThumbSize = new Vector2(0.2f, 0.2f);
        public Vector2 ScrollbarSize = new Vector2(0.2f, 0.2f);
        public RectOffset HorizontalSliderOverflow = new RectOffset();
        public RectOffset VerticalSliderOverflow = new RectOffset();


#if UNITY_EDITOR
        public bool UpdateEveryFrame = true;
#endif

        private ScreenSizeChange _ScreenSizeChange;

        void Update()
        {
#if UNITY_EDITOR
            if (UpdateEveryFrame || _ScreenSizeChange.IsChanged)
#else
                if (_ScreenSizeChange.IsChanged)
#endif


            {
                if (Skin != null)
                {
                    float factor = (Screen.width + Screen.height) * 0.1f;
                    Skin.box.fontSize = Mathf.FloorToInt(factor * BoxFontSize);
                    Skin.button.fontSize = Mathf.FloorToInt(factor * ButtonFontSize);
                    Skin.toggle.fontSize = Mathf.FloorToInt(factor * ToggleFontSize);
                    Skin.label.fontSize = Mathf.FloorToInt(factor * LabelFontSize);
                    Skin.textField.fontSize = Mathf.FloorToInt(factor * TextFieldFontSize);
                    Skin.textArea.fontSize = Mathf.FloorToInt(factor * TextAreaFontSize);


                    Mul(Skin.horizontalSlider.overflow, HorizontalSliderOverflow, factor * 0.01f);
                    Mul(Skin.verticalSlider.overflow, VerticalSliderOverflow, factor * 0.01f);

                    Skin.horizontalSliderThumb.fixedWidth = factor * HorizontalSliderThumbSize.x;
                    Skin.horizontalSliderThumb.fixedHeight = factor * HorizontalSliderThumbSize.y;
                    Skin.verticalSliderThumb.fixedWidth = factor * VerticalSliderThumbSize.x;
                    Skin.verticalSliderThumb.fixedHeight = factor * VerticalSliderThumbSize.y;

                    Skin.verticalScrollbar.fixedWidth = factor * ScrollbarSize.x;
                    Skin.horizontalScrollbar.fixedHeight = factor * ScrollbarSize.y;
                }
            }
        }

        private void Mul(RectOffset dest, RectOffset rect, float factor)
        {
            dest.left = Mathf.FloorToInt(rect.left * factor);
            dest.right = Mathf.FloorToInt(rect.right * factor);
            dest.top = Mathf.FloorToInt(rect.top * factor);
            dest.bottom = Mathf.FloorToInt(rect.bottom * factor);

            if ((rect.left < 0 && dest.left > 0) || (rect.left > 0 && dest.left < 0)) dest.left = 0;
            if ((rect.right < 0 && dest.right > 0) || (rect.right > 0 && dest.right < 0)) dest.right = 0;
            if ((rect.top < 0 && dest.top > 0) || (rect.top > 0 && dest.top < 0)) dest.top = 0;
            if ((rect.bottom < 0 && dest.bottom > 0) || (rect.bottom > 0 && dest.bottom < 0)) dest.bottom = 0;
        }
    }

}