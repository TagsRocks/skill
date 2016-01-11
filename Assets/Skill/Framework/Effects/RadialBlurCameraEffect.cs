using UnityEngine;
using System.Collections;


namespace Skill.Framework.Effects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class RadialBlurCameraEffect : MonoBehaviour
    {
        public static RadialBlurCameraEffect Instance { get; private set; }

        public Transform RelativeTo;
        public Shader BlurShader = null;
        public float Smoothing = 0.1f;
        public float UVDistance = 0.6f;
        public float Strength = 2;
        public float Range = 30.0f;

        private Vector4 _PreParameters;
        private Vector4 _Parameters;
        private bool _Support;

        private static Material _Material = null;
        protected Material Material
        {
            get
            {
                if (_Material == null)
                {
                    _Material = new Material(BlurShader);
                    _Material.hideFlags = HideFlags.DontSave;
                }
                return _Material;
            }
        }


        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        void OnDestroy()
        {
            if (_Material)
            {
                DestroyImmediate(_Material);
            }
        }

        // --------------------------------------------------------

        protected void Start()
        {
            _Support = true;
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects)
            {
                _Support = false;                
            }
            // Disable if the shader can't run on the users graphics card
            if (!BlurShader || !Material.shader.isSupported)
            {
                _Support = false;                
            }
            enabled = false;
        }


        // Called by the camera to apply the image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_Support)
            {
                if (Material != null)
                {
                    if (_PreParameters != _Parameters)
                    {
                        Material.SetVector("_Parameters", _Parameters);
                        _PreParameters = _Parameters;
                    }
                    Graphics.Blit(source, destination, Material);
                }
                else
                    Graphics.Blit(source, destination);
            }
        }


        private Skill.Framework.SmoothingParameters _Smoothing = new Skill.Framework.SmoothingParameters();
        private Skill.Framework.Smoothing _DistanceSmoothing;
        private Skill.Framework.Smoothing _StrengthSmoothing;
        private Skill.Framework.TimeWatch _BlurTimeTW;
        private Skill.Framework.TimeWatch _DisableTimeTW;

        public void Blur(Vector3 position, float time)
        {
            if (_Support)
            {
                if (Global.Instance != null)
                {
                    if (!Global.Instance.Settings.Quality.RadialBlur)
                        return;
                }

                if (RelativeTo == null) RelativeTo = transform;
                float distance = Vector3.Distance(RelativeTo.position, position);
                if (distance < Range)
                {
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);

                    screenPoint.x = Mathf.Clamp(screenPoint.x, 0, Screen.width);
                    screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);

                    screenPoint.x /= Screen.width;
                    screenPoint.y /= Screen.height;

                    float factor = 1.0f - (distance / Range);
                    _Smoothing.SmoothType = Skill.Framework.SmoothType.Damp;
                    _Smoothing.SmoothTime = Smoothing;
                    _BlurTimeTW.Begin(time);
                    _DistanceSmoothing.Reset(UVDistance * factor);
                    _StrengthSmoothing.Reset(Strength * factor);
                    _Parameters.z = screenPoint.x;
                    _Parameters.w = screenPoint.y;
                    enabled = true;
                }
            }
        }


        void Update()
        {
            if (_BlurTimeTW.IsEnabled)
            {
                if (_BlurTimeTW.IsOver)
                {
                    _DistanceSmoothing.Target = 0;
                    _StrengthSmoothing.Target = 0;
                    _DisableTimeTW.Begin(2);
                    _BlurTimeTW.End();
                }

            }

            _DistanceSmoothing.Update(_Smoothing);
            _StrengthSmoothing.Update(_Smoothing);

            _Parameters.x = _DistanceSmoothing.Current;
            _Parameters.y = _StrengthSmoothing.Current;

            if (_DisableTimeTW.IsEnabledAndOver)
            {
                _Parameters.x = 0;
                _Parameters.y = 0;
                _DisableTimeTW.End();
                enabled = false;
            }
        }
    }

}