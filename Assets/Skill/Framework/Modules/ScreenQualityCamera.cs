using UnityEngine;
using System.Collections;


namespace Skill.Framework.Modules
{
    [RequireComponent(typeof(Camera))]
    public class ScreenQualityCamera : MonoBehaviour
    {
        public Renderer Plane;
        public float PlaneDistance = 0.5f;
        public float QualityStep = 0.1f; // between 0.1f , 0.5f
        public Camera SceneCamera;


        private RenderTexture _RenderTexture;
        private float _PreQuality;
        private Camera _Camera;
        private ScreenSizeChange _ScreenSizeChange;

        // Use this for initialization
        void Start()
        {
            useGUILayout = false;
            _PreQuality = 1.0f;

            if (Plane == null)
                throw new System.ArgumentNullException("you must specify a valid plane infront of ScreenQualityCamera");

            if (!Plane.transform.IsChildOf(transform))
                throw new System.InvalidOperationException("plane must be child of ScreenQualityCamera (and very close to it)");
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
                quality = Global.Instance.Settings.Quality.ScreenQuality;
                quality = Mathf.Clamp(quality - Mathf.Repeat(quality, QualityStep), 0.1f, 4.0f);

            }
            if (_PreQuality != quality || _RenderTexture == null || (SceneCamera != null && SceneCamera != _Camera))
                Rebuild(quality);
        }

        private void Rebuild(float quality)
        {
            quality = Mathf.Clamp(quality, 0.1f, 4.0f);
            _PreQuality = quality;
            int width = Mathf.Max(Mathf.Min(Mathf.FloorToInt(Screen.width * _PreQuality), UnityEngine.SystemInfo.maxTextureSize), 256);
            int height = Mathf.Max(Mathf.Min(Mathf.FloorToInt(Screen.height * _PreQuality), UnityEngine.SystemInfo.maxTextureSize), 256);

            if (_Camera != null)
                _Camera.targetTexture = null;
            if (_RenderTexture != null)
                Destroy(_RenderTexture);

            _Camera = SceneCamera;


            _RenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            _RenderTexture.generateMips = false;
            _Camera.targetTexture = _RenderTexture;
            if (Plane != null)
                Plane.material.mainTexture = _RenderTexture;
        }
    }
}