using UnityEngine;
using System.Collections;


namespace Skill.Framework.Modules
{
    [RequireComponent(typeof(Camera))]
    public class ScreenQualityCamera : MonoBehaviour
    {
        public static ScreenQualityCamera Instance { get; private set; }

        public Renderer Plane;
        public float PlaneDistance = 0.5f;
        public float QualityStep = 0.1f; // between 0.1f , 0.5f
        public Camera SceneCamera;
        public bool AutoApply = true;

        private RenderTexture _RenderTexture;
        private float _PreQuality;
        private Camera _LastSceneCamera;
        private Camera _MyCamera;
        private ScreenSizeChange _ScreenSizeChange;
        private RenderTextureFormat _RenderTextureFormat;
        private int _DefaultScreenWidth;
        private int _DefaultScreenHeight;
        private bool _Rebuild;
        private bool _Apply;
        private bool _SupportsRenderTarget;
        private bool _ReallySupportsRenderTarget;
        /// <summary>
        ///  turn it off if your rendertarget goes to black
        /// </summary>
        public bool SupportsRenderTarget
        {
            get { return _SupportsRenderTarget; }
            set
            {
                if (value)
                    value = _ReallySupportsRenderTarget;
                if (_SupportsRenderTarget != value)
                {
                    _SupportsRenderTarget = value;
                    _Rebuild = true;
                }
            }
        }

        /// <summary> Width of screen after applying quality </summary>
        public int Width { get; private set; }

        /// <summary> Height of screen after applying quality</summary>
        public int Height { get; private set; }

        void Awake()
        {
            Instance = this;
            useGUILayout = false;
            _PreQuality = 1.0f;

            if (SceneCamera == null)
                SceneCamera = Camera.main;

            if (Plane == null)
                throw new System.ArgumentNullException("you must specify a valid plane infront of ScreenQualityCamera");

            if (!Plane.transform.IsChildOf(transform))
                throw new System.InvalidOperationException("plane must be child of ScreenQualityCamera (and very close to it)");

            _MyCamera = GetComponent<Camera>();

            _DefaultScreenWidth = Screen.width;
            _DefaultScreenHeight = Screen.height;


            _ReallySupportsRenderTarget = _SupportsRenderTarget = SystemInfo.supportsRenderTextures && SystemInfo.supportsImageEffects;
            _Rebuild = true;
            _Apply = true;

            if (_SupportsRenderTarget)
            {
                _RenderTextureFormat = RenderTextureFormat.RGB565;
                if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
                    _RenderTextureFormat = RenderTextureFormat.ARGB32;
                else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565))
                    _RenderTextureFormat = RenderTextureFormat.RGB565;
                else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                    _RenderTextureFormat = RenderTextureFormat.ARGBHalf;
                else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB4444))
                    _RenderTextureFormat = RenderTextureFormat.ARGB4444;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (_ScreenSizeChange.IsChanged && Plane != null)
            {
                float w = MathHelper.FrustumWidthAtDistance(Camera.main, PlaneDistance);
                float h = MathHelper.FrustumHeightAtDistance(Camera.main, PlaneDistance);

                Plane.transform.localPosition = new Vector3(0, 0, PlaneDistance);
                Plane.transform.localScale = new Vector3(w, h, 1);
                Plane.transform.localPosition = new Vector3(0, 0, PlaneDistance);
            }

            float quality = _PreQuality;
            if (Global.Instance != null)
            {
                if (QualityStep > 0.5f) QualityStep = 0.5f;
                if (QualityStep < 0.1f) QualityStep = 0.1f;
                quality = Global.Instance.Settings.Quality.ScreenQuality;
                quality = Mathf.Clamp(quality - Mathf.Repeat(quality, QualityStep), 0.1f, 1.0f);

            }
            if (_PreQuality != quality || (_RenderTexture == null && _SupportsRenderTarget) || (SceneCamera != null && SceneCamera != _LastSceneCamera))
                _Rebuild = true;

            if (_Rebuild)
            {
                if (_Apply || (AutoApply && Input.touchCount == 0))
                {
                    Rebuild(quality);
                }
            }

            if (_RenderTexture != null)
            {
                if (!_RenderTexture.IsCreated())
                {
                    _RenderTexture.Create();
                    _LastSceneCamera.targetTexture = _RenderTexture;
                }
            }
        }

        private void Rebuild(float quality)
        {
            _Rebuild = false;
            _Apply = false;
            _LastSceneCamera = SceneCamera;
            _PreQuality = quality;

            if (quality >= 0.98f)
            {
                if (_LastSceneCamera != null)
                    _LastSceneCamera.targetTexture = null;
                if (_RenderTexture != null)
                    Destroy(_RenderTexture);
                _RenderTexture = null;

                if (!_SupportsRenderTarget)
                    Screen.SetResolution(_DefaultScreenWidth, _DefaultScreenHeight, Screen.fullScreen);

                this.Width = _DefaultScreenWidth;
                this.Height = _DefaultScreenHeight;

                _MyCamera.enabled = false;
            }
            else
            {

                quality = Mathf.Clamp(quality, 0.1f, 1.0f);
                this.Width = Mathf.Max(Mathf.Min(Mathf.FloorToInt(_DefaultScreenWidth * _PreQuality), UnityEngine.SystemInfo.maxTextureSize), 256);
                this.Height = Mathf.Max(Mathf.Min(Mathf.FloorToInt(_DefaultScreenHeight * _PreQuality), UnityEngine.SystemInfo.maxTextureSize), 256);

                if (this.Width % 2 == 1) this.Width++;
                if (this.Height % 2 == 1) this.Height++;

                if (this.Width > SystemInfo.maxTextureSize) this.Width = SystemInfo.maxTextureSize;
                if (this.Height > SystemInfo.maxTextureSize) this.Height = SystemInfo.maxTextureSize;

                if (_LastSceneCamera != null)
                    _LastSceneCamera.targetTexture = null;
                if (_RenderTexture != null)
                    Destroy(_RenderTexture);
                _RenderTexture = null;

                if (_SupportsRenderTarget && _LastSceneCamera != null)
                {
                    _RenderTexture = new RenderTexture(this.Width, this.Height, 24, _RenderTextureFormat);
                    _RenderTexture.generateMips = false;
                    _RenderTexture.Create();
                    _LastSceneCamera.targetTexture = _RenderTexture;
                    _LastSceneCamera.enabled = true;
                    if (Plane != null)
                        Plane.material.mainTexture = _RenderTexture;
                    _MyCamera.enabled = true;
                }
                else
                {
                    Screen.SetResolution(this.Width, this.Height, Screen.fullScreen);
                    _MyCamera.enabled = false;
                }
            }
        }

        public void Apply()
        {
            _Apply = true;
        }

        public Vector2 ToScreenSpace(Vector2 point)
        {
            point.x = (point.x * this.Width) / Screen.width;
            point.y = (point.y * this.Height) / Screen.height;
            return point;
        }

        public Vector2 ToCameraSpace(Vector2 point)
        {
            point.x = (point.x * Screen.width) / this.Width;
            point.y = (point.y * Screen.height) / this.Height;
            return point;
        }

    }
}