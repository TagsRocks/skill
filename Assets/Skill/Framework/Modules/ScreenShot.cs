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
        /// <summary> name of image </summary>
        public string ImageName = "Screen";
        /// <summary> Create jpg image </summary>
        public bool Jpg = true;

        private bool _TakeShot = false;
        private bool _ShotTaked = false;
        private RenderTexture _RenderTexture;
        private Texture2D _ScreenShot;
        private Camera _Camera;
        private int _PreWidth, _PreHeight;


        public string LastShotFileName { get; private set; }

        public event System.EventHandler Shot;
        private void OnShot()
        {
            Debug.Log(string.Format("Took screenshot to: {0}", LastShotFileName));
            if (Shot != null) Shot(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Create new png file path to save new screenshot
        /// </summary>
        /// <param name="width"> Width of screenshot</param>
        /// <param name="height"> Height of screenshot</param>
        /// <returns>new png file path</returns>
        protected virtual string GetNewFilePath(int width, int height)
        {
            if (string.IsNullOrEmpty(ImageName))
                ImageName = "Screen";
            return System.IO.Path.Combine(Directory, string.Format("{0}_{1}x{2}_{3}.{4}", ImageName, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), Jpg ? "jpg" : "png"));
        }

        public void TakeShot()
        {
            _TakeShot = true;
        }

        private void ValidateRenderTexture(int width, int height)
        {
            if (_PreWidth != width || _PreHeight != height || _RenderTexture == null)
            {
                if (_RenderTexture != null)
                {
                    if (Application.isPlaying)
                        Destroy(_RenderTexture);
                    else
                        DestroyImmediate(_RenderTexture);
                }
                if (_ScreenShot != null)
                {
                    if (Application.isPlaying)
                        Destroy(_ScreenShot);
                    else
                        DestroyImmediate(_ScreenShot);                    
                }

                _PreWidth = width;
                _PreHeight = height;
                _RenderTexture = new RenderTexture(_PreWidth, _PreHeight, 24);
                _ScreenShot = new Texture2D(_PreWidth, _PreHeight, TextureFormat.RGB24, false);
            }
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            _Camera = GetComponent<Camera>();
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
                    SaveShot();

                    _TakeShot = false;
                    _ShotTaked = true;
                    OnShot();
                }
            }
        }

        private void SaveShot()
        {
            int width, height;
            if (CustomSize)
            {
                width = Width;
                height = Height;
            }
            else
            {
                width = (int)(_Camera.pixelWidth * Scale);
                height = (int)(_Camera.pixelHeight * Scale);
            }

            ValidateRenderTexture(width, height);

            _Camera.targetTexture = _RenderTexture;
            _Camera.Render();
            RenderTexture.active = _RenderTexture;
            _ScreenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            _Camera.targetTexture = null;
            RenderTexture.active = null;

            byte[] bytes = null;
            if (Jpg)
                bytes = _ScreenShot.EncodeToJPG();
            else
                bytes = _ScreenShot.EncodeToPNG();
            LastShotFileName = GetNewFilePath(width, height);

            string dir = System.IO.Path.GetDirectoryName(LastShotFileName);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            if (System.IO.File.Exists(LastShotFileName))
                System.IO.File.Delete(LastShotFileName);

            System.IO.FileStream fileStream = new System.IO.FileStream(LastShotFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.BinaryWriter writer = new System.IO.BinaryWriter(fileStream);
            writer.Write(bytes);
            writer.Close();
            fileStream.Close();

        }

        protected override void OnDestroy()
        {
            if (_RenderTexture != null) Destroy(_RenderTexture);
            if (_ScreenShot != null) Destroy(_ScreenShot);
            base.OnDestroy();
        }


#if UNITY_EDITOR

        [ContextMenu("Take a Shot")]
        public void TakeAShot()
        {
            if (_Camera == null)
                _Camera = GetComponent<Camera>();
            SaveShot();
        }

#endif
    }
}