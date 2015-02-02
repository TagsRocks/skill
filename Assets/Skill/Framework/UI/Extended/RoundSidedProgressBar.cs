using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Skill.Framework.UI.Extended
{
    /// <summary>
    /// a ProgressBar with round corners at left/right or top/down
    /// </summary>
    public class RoundSidedProgressBar : CanvasPanel
    {
        private ImageWithTexCoords _ImgLeft;
        private ImageWithTexCoords _ImgCenter;
        private ImageWithTexCoords _ImgRight;


        /// <summary>
        /// Occurs when value of ProgressBar changed
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// when value of ProgressBar changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            UpdateUVCoordinates();
            if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Value to show between 0.0f - 1.0f
        /// </summary>
        private float _Value;
        public float Value
        {
            get { return _Value; }
            set
            {
                if (value < 0) value = 0;
                else if (value > 1.0f) value = 1.0f;
                _Value = value;
                OnValueChanged();
            }
        }

        /// <summary>
        /// Is corners at left/right or top/down
        /// </summary>
        public Orientation Orientation { get; private set; }

        /// <summary>
        /// Texture
        /// </summary>
        public Texture Texture
        {
            get { return _ImgLeft.Texture; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Invalid texture for RoundSidedImage");
                _ImgLeft.Texture = _ImgRight.Texture = _ImgCenter.Texture = value;
                UpdateUVCoordinates();
            }
        }

        private float _RoundSize;
        /// <summary>
        /// Size of round side in pixel
        /// </summary>
        public float RoundSize
        {
            get { return _RoundSize; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("BorderInPixels can not be lower than i pixel");
                _RoundSize = value;
                UpdateUVCoordinates();
            }
        }

        /// <summary>
        /// When RenderArea changed
        /// </summary>
        protected override void OnRenderAreaChanged()
        {
            this.Width = RenderArea.width;
            this.Height = RenderArea.height;
            UpdateUVCoordinates();
            base.OnRenderAreaChanged();
        }

        /// <summary>
        /// Create a RoundSidedProgressBar
        /// </summary>
        /// <param name="texture">Texture of ProgressBar</param>
        /// <param name="orientation"> left/right or top/down </param>
        /// <param name="roundSize">Size of round side in pixel</param>
        public RoundSidedProgressBar(Texture texture, Orientation orientation, float roundSize)
        {
            if (texture == null)
                throw new ArgumentNullException("Invalid texture for RoundSidedImage");
            if (roundSize < 0)
                throw new ArgumentException("BorderInPixels can not be lower than i pixel");

            this.Orientation = orientation;
            this._ImgLeft = new ImageWithTexCoords();
            this._ImgCenter = new ImageWithTexCoords();
            this._ImgRight = new ImageWithTexCoords();

            this.Controls.Add(this._ImgLeft);
            this.Controls.Add(this._ImgCenter);
            this.Controls.Add(this._ImgRight);

            this._RoundSize = roundSize;
            this.Texture = texture;
        }


        private void UpdateUVCoordinates()
        {
            if (Orientation == Skill.Framework.UI.Orientation.Horizontal)
            {
                float borderPercent = _RoundSize / Texture.width;

                this._ImgLeft.TextureCoordinate = new Rect(0, 0, Mathf.Min(_Value, borderPercent), 1.0f);
                this._ImgCenter.TextureCoordinate = new Rect(borderPercent, 0, Mathf.Max(borderPercent, Mathf.Min(_Value, 1.0f - borderPercent)) - borderPercent, 1.0f);
                this._ImgRight.TextureCoordinate = new Rect(1.0f - borderPercent, 0, Mathf.Max(1.0f - borderPercent, _Value) - (1.0f - borderPercent), 1.0f);

                this._ImgLeft.Visibility = (_Value > Mathf.Epsilon) ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
                this._ImgCenter.Visibility = (_Value > borderPercent) ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
                this._ImgRight.Visibility = (_Value > 1.0f - borderPercent) ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;

                float borderWidthPercent = _RoundSize / Width;

                if (_Value <= borderWidthPercent && _Value > Mathf.Epsilon)
                {
                    _ImgLeft.Position = new Rect(0, 0, _Value * Width, Height);
                }
                if (_Value <= 1.0f - borderWidthPercent)
                {
                    _ImgLeft.Position = new Rect(0, 0, _RoundSize, Height);
                    _ImgCenter.Position = new Rect(_RoundSize, 0, (_Value - borderWidthPercent) * Width, Height);
                }
                else if (_Value > 1.0f - borderWidthPercent)
                {
                    _ImgLeft.Position = new Rect(0, 0, _RoundSize, Height);
                    _ImgCenter.Position = new Rect(_RoundSize, 0, Width - (2 * _RoundSize), Height);
                    _ImgRight.Position = new Rect(Width - _RoundSize, 0, (_Value - (1.0f - borderWidthPercent)) * Width, Height);
                }
            }
            else
            {
                float borderPercent = Mathf.Max(Mathf.Epsilon, _RoundSize / Texture.height);

                this._ImgLeft.TextureCoordinate = new Rect(0, 1.0f, 1.0f, 1.0f - Mathf.Min(_Value, borderPercent));
                this._ImgCenter.TextureCoordinate = new Rect(0, 1.0f - borderPercent, 1.0f, 1.0f - Mathf.Max(borderPercent, Mathf.Min(_Value, 1.0f - borderPercent)) - borderPercent);
                this._ImgRight.TextureCoordinate = new Rect(0, borderPercent, 1.0f, borderPercent - Mathf.Max(1.0f - borderPercent, _Value));

                this._ImgLeft.Visibility = (_Value > Mathf.Epsilon) ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
                this._ImgCenter.Visibility = (_Value > borderPercent) ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
                this._ImgRight.Visibility = (_Value > 1.0f - borderPercent) ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;

                float borderHeightPercent = _RoundSize / Height;

                if (_Value <= borderHeightPercent && _Value > Mathf.Epsilon)
                {
                    _ImgLeft.Position = new Rect(0, 0, Width, _Value * Height);
                }
                if (_Value <= 1.0f - borderHeightPercent)
                {
                    _ImgLeft.Position = new Rect(0, 0, Width, _RoundSize);
                    _ImgCenter.Position = new Rect(0, _RoundSize, Width, (_Value - borderHeightPercent) * Height);
                }
                else if (_Value > 1.0f - borderHeightPercent)
                {
                    _ImgLeft.Position = new Rect(0, 0, Width, _RoundSize);
                    _ImgCenter.Position = new Rect(0, _RoundSize, Width, Height - (2 * borderHeightPercent));
                    _ImgRight.Position = new Rect(0, Width - _RoundSize, Width, (_Value - (1.0f - borderHeightPercent)) * Height);
                }
            }
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
