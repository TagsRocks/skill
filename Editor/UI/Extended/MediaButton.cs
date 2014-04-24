using UnityEngine;
using System.Collections;
using Skill.Editor.UI;
using Skill.Framework.Sequence;

namespace Skill.Editor.UI.Extended
{
    /// <summary>
    /// a multistate button to use in playback
    /// </summary>
    public class MediaButton : Button
    {
        private GUIStyle _NormalStyle;
        private GUIStyle _PressedStyle;
        private Texture2D _OnTexture;
        private Texture2D _NormalTexture;

        /// <summary>
        /// Toggle pressed OnClisk ?
        /// </summary>
        public bool TogglePressed { get; set; }

        private bool _IsPressed;
        /// <summary>
        /// Is Pressed
        /// </summary>
        public bool IsPressed
        {
            get { return _IsPressed; }
            set
            {
                if (this._IsPressed != value)
                {
                    this._IsPressed = value;
                    UpdateStyle();
                }
            }
        }

        public Texture2D OnTexture
        {
            get { return _OnTexture; }
            set
            {
                _OnTexture = value;
                UpdateStyle();
            }
        }
        public Texture2D NormalTexture
        {
            get { return _NormalTexture; }
            set
            {
                _NormalTexture = value;
                UpdateStyle();
            }
        }


        private bool _IsPlayMode;
        /// <summary>
        /// Is playmode? style changed to blue
        /// </summary>
        public bool IsPlayMode
        {
            get { return _IsPlayMode; }
            set
            {
                if (this._IsPlayMode != value)
                {
                    this._IsPlayMode = value;
                    UpdateStyle();
                }
            }
        }

        /// <summary>
        /// Create a media button
        /// </summary>        
        public MediaButton()
            : this(null, null)
        {
        }

        /// <summary>
        /// Create a media button
        /// </summary>
        /// <param name="normal">Normal image</param>
        /// <param name="on">Playmode image</param>
        public MediaButton(Texture2D normal, Texture2D on)
        {

            this._NormalTexture = normal;
            this._OnTexture = on;

            _IsPressed = true;
            IsPressed = false;

            _IsPlayMode = true;
            IsPlayMode = false;
        }

        /// <summary>
        /// OnClick
        /// </summary>
        protected override void OnClick()
        {
            if (TogglePressed) IsPressed = !_IsPressed;
            base.OnClick();
        }

        /// <summary>
        /// Change style
        /// </summary>
        /// <param name="style">Style of button</param>
        public void SetStyle(GUIStyle style)
        {
            this._NormalStyle = new GUIStyle(style);
            this._NormalStyle.padding = new RectOffset(2, 2, 2, 2);
            this._PressedStyle = new GUIStyle(_NormalStyle);
            this._PressedStyle.normal = style.active;
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            this.Style = _IsPressed ? this._PressedStyle : this._NormalStyle;
            this.Content.image = _IsPlayMode ? this._OnTexture : this._NormalTexture;
        }
    }
}