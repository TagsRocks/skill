using UnityEngine;
using System.Collections;
using Skill.Editor.UI;
using Skill.Framework.Sequence;

namespace Skill.Editor.Sequence
{
    /// <summary>
    /// a multistate button to use in playback
    /// </summary>
    public class MediaButton : Button
    {
        private GUIStyle _NormalStyle;
        private GUIStyle _PressedStyle;
        private Texture2D _PlayTexture;
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
                    this.Style = (_IsPressed && TogglePressed) ? this._PressedStyle : this._NormalStyle;
                }
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
                    this.Content.image = _IsPlayMode ? this._PlayTexture : this._NormalTexture;
                }
            }
        }

        /// <summary>
        /// Create a media button
        /// </summary>
        /// <param name="normalTexture">Normal image</param>
        /// <param name="playTexture">Playmode image</param>
        public MediaButton(Texture2D normalTexture, Texture2D playTexture)
        {

            this._NormalTexture = normalTexture;
            this._PlayTexture = playTexture;

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
            this.Style = this._NormalStyle;
        }
    }
}