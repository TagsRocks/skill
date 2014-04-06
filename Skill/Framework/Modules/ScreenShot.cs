using UnityEngine;
using System.Collections;


namespace Skill.Framework.Modules
{
    /// <summary>
    /// Take screenshots during game
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ScreenShot : Skill.Framework.DynamicBehaviour
    {
        /// <summary> Where to save screen shots </summary>
        public string Directory = "C:/ScreenShots";
        /// <summary> Combined keys as shortcut to take screenshot </summary>
        public KeyCode[] Shortcut = new KeyCode[] { KeyCode.F10, KeyCode.LeftShift, KeyCode.LeftAlt };
        /// <summary> Use custom size for shots? </summary>
        [HideInInspector]
        public bool CustomSize = true;
        /// <summary> Custom width of screenshot </summary>
        [HideInInspector]
        public int Width = 2048;
        /// <summary> Custom height of screenshot </summary>
        [HideInInspector]
        public int Height = 2048;
        /// <summary> Scale of screenshot relative to screen size </summary>
        [HideInInspector]
        public float Scale = 1;

        private bool _TakeShot = false;
        private bool _ShotTaked = false;
        private RenderTexture _RenderTexture;
        private Texture2D _ScreenShot;
        private int _PreWidth, _PreHeight;

        /// <summary>
        /// Create new png file path to save new screenshot
        /// </summary>
        /// <param name="width"> Width of screenshot</param>
        /// <param name="height"> Height of screenshot</param>
        /// <returns>new png file path</returns>
        protected virtual string GetNewFilePath(int width, int height)
        {
            return System.IO.Path.Combine(Directory, string.Format("screen_{0}x{1}_{2}.png", width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
        }

        public void TakeShot()
        {
            _TakeShot = true;
        }

        private void ValidateRenderTexture(int width, int height)
        {
            if (_PreWidth != width || _PreHeight != height)
            {
                if (_RenderTexture != null) Destroy(_RenderTexture);
                if (_ScreenShot != null) Destroy(_ScreenShot);

                _PreWidth = width;
                _PreHeight = height;
                _RenderTexture = new RenderTexture(_PreWidth, _PreHeight, 24);
                _ScreenShot = new Texture2D(_PreWidth, _PreHeight, TextureFormat.RGB24, false);
            }
        }

        void LateUpdate()
        {
            if (_ShotTaked)
            {
                if (!Input.anyKey)// avoid taking shot every frame
                    _ShotTaked = false;
            }
            else
            {
                if (!_TakeShot && Shortcut != null && Shortcut.Length > 0)
                {
                    _TakeShot = true;
                    for (int i = 0; i < Shortcut.Length; i++)
                    {
                        if (!Input.GetKey(Shortcut[i]))
                        {
                            _TakeShot = false;
                            break;
                        }
                    }
                }
                if (_TakeShot)
                {
                    int width, height;
                    if (CustomSize)
                    {
                        width = Width;
                        height = Height;
                    }
                    else
                    {
                        width = (int)(camera.pixelWidth * Scale);
                        height = (int)(camera.pixelHeight * Scale);
                    }

                    ValidateRenderTexture(width, height);

                    camera.targetTexture = _RenderTexture;
                    camera.Render();
                    RenderTexture.active = _RenderTexture;
                    _ScreenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    camera.targetTexture = null;
                    RenderTexture.active = null;

                    byte[] bytes = _ScreenShot.EncodeToPNG();
                    string filename = GetNewFilePath(width, height);

                    string dir = System.IO.Path.GetDirectoryName(filename);
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);

                    System.IO.File.WriteAllBytes(filename, bytes);
                    Debug.Log(string.Format("Took screenshot to: {0}", filename));
                    _TakeShot = false;
                    _ShotTaked = true;
                }
            }
        }

        protected override void OnDestroy()
        {
            if (_RenderTexture != null) Destroy(_RenderTexture);
            if (_ScreenShot != null) Destroy(_ScreenShot);
            base.OnDestroy();
        }
    }
}