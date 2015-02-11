using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI
{
    /// <summary>
    /// Simple ProgressBar : use a Box as background and an Image as fill image
    /// </summary>
    public class ProgressBar : CanvasPanel
    {
        private Box _Background;
        private Image _ImgFill;

        /// <summary> Texture to use as fill ProgressBar </summary>
        public Texture FillImage { get { return _ImgFill.Texture; } set { _ImgFill.Texture = value; } }
        /// <summary> Style to use as background Box of ProgressBar </summary>
        public GUIStyle BackgroundStyle { get { return _Background.Style; } set { _Background.Style = value; } }
        /// <summary> TintColor of texture to use as fill ProgressBar </summary>
        public Color FillTintColor { get { return _ImgFill.TintColor; } set { _ImgFill.TintColor = value; } }

        /// <summary>
        /// Occurs when value of ProgressBar changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when value of ProgressBar changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            _Changed = true;
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        private bool _Changed = true;
        private float _Value;
        /// <summary>
        /// Value that is shown (0.0f - 1.0f).
        /// </summary>
        public float Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = Mathf.Clamp01(value);
                    OnValueChanged();
                }
            }
        }

        /// <summary> Margin of fill </summary>
        public Thickness FillMargin { get; set; }

        /// <summary>
        /// Create a ProgressBar
        /// </summary>
        public ProgressBar()
        {
            _Background = new Box();
            _ImgFill = new Image() { Scale = ScaleMode.StretchToFill };

            this.Controls.Add(_Background);
            this.Controls.Add(_ImgFill);
            this._Value = 0;
        }

        /// <summary>
        /// When RenderArea changed
        /// </summary>
        protected override void OnRenderAreaChanged()
        {
            UpdateFillArea();
            base.OnRenderAreaChanged();
        }

        /// <summary>
        /// Render
        /// </summary>
        protected override void Render()
        {
            if (_Changed)
                UpdateFillArea();
            base.Render();
        }

        private void UpdateFillArea()
        {
            Rect rect = this.RenderArea;

            rect.x = rect.y = 0;
            _Background.Position = rect;
            Thickness margin = FillMargin;

            rect.x = margin.Left;
            rect.width -= margin.Horizontal;
            rect.width *= _Value;

            rect.y = margin.Top;
            rect.height -= margin.Vertical;

            _ImgFill.Position = rect;
            _Changed = false;
        }

        /// <summary>
        /// Set value between minimum and maximum
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="value">Value between min and max</param>
        public void SetValue(float min, float max, float value)
        {
            this.Value = (value - min) / (max - min);
        }
    }
}
