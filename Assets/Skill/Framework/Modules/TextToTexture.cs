using UnityEngine;
using System.Collections;
using Skill.Framework;
namespace Skill.Framework.Modules
{
    public class TextToTexture : DynamicBehaviour
    {
        public Camera RenderCamera;
        public GUIText Text;
        public int DeltaFontSize = 10;
        public int Width = 512;
        public int Height = 512;
        public bool GenerateNormalMap = false;
        public float NormalMapStrength = 2;

        private Texture2D _Diffuse;
        private Texture2D _NormalMap;

        public Texture2D Diffuse { get { return _Diffuse; } }
        public Texture2D NormalMap { get { return _NormalMap; } }

        protected override void Start()
        {
            base.Start();
            _Diffuse = new Texture2D(Width, Height, TextureFormat.RGB24, true);
        }

        private void CalcFontSize()
        {
            GUIStyle style = new GUIStyle();
            style.font = this.Text.font;
            style.fontStyle = this.Text.fontStyle;

            GUIContent content = new GUIContent() { text = this.Text.text };
            this.Text.fontSize = 200;
            int fontSize = 200;
            if (!string.IsNullOrEmpty(this.Text.text))
            {
                for (fontSize = 2; fontSize < this.Text.fontSize; fontSize++)
                {
                    style.fontSize = fontSize;
                    Vector2 size = style.CalcSize(content);
                    size = style.CalcScreenSize(size);
                    if (size.x >= this.Width - 1 || size.y >= this.Height - 1)
                    {
                        break;
                    }

                }
                this.Text.fontSize = fontSize + DeltaFontSize;
            }
            else
            {
                this.Text.fontSize = 8;
            }
        }

        public void Render(string text)
        {
            if (text == null) text = string.Empty;
            //Camera cam = Camera.main;
            Camera cam = RenderCamera;
            cam.enabled = true;
            float preAspect = cam.aspect;
            float preFieldOfView = cam.fieldOfView;
            float preDepth = cam.depth;
            Rect preRect = cam.rect;
            Vector3 _PrePos = cam.transform.position;
            Color _PreBackColor = cam.backgroundColor;
            Material preSky = RenderSettings.skybox;

            this.Text.text = text;
            this.Text.enabled = true;
            this.Text.color = Color.white;
            CalcFontSize();
            RenderTexture renderTexture = new RenderTexture(Width, Height, 24);

            RenderSettings.skybox = null;
            cam.transform.position = this.transform.position;
            cam.backgroundColor = Color.black;
            cam.targetTexture = renderTexture;
            cam.Render();
            RenderTexture.active = renderTexture;
            _Diffuse.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            cam.targetTexture = null;
            RenderTexture.active = null;

            _Diffuse.Apply();
            this.Text.enabled = false;


            cam.aspect = preAspect;
            cam.fieldOfView = preFieldOfView;
            cam.depth = preDepth;
            cam.rect = preRect;
            cam.transform.position = _PrePos;
            cam.backgroundColor = _PreBackColor;
            RenderSettings.skybox = preSky;
            cam.enabled = false;
            if (GenerateNormalMap)
                CreateNormalMap(NormalMapStrength);
        }

        private void CreateNormalMap(float strength)
        {
            if (_NormalMap == null)
                _NormalMap = new Texture2D(Width, Height, TextureFormat.RGB24, true);

            strength = Mathf.Clamp(strength, 0.0F, 10.0F);

            float xLeft;
            float xRight;
            float yUp;
            float yDown;
            float yDelta;
            float xDelta;
            for (int by = 0; by < _NormalMap.height; by++)
            {
                for (int bx = 0; bx < _NormalMap.width; bx++)
                {
                    xLeft = _Diffuse.GetPixel(bx - 1, by).grayscale * strength;
                    xRight = _Diffuse.GetPixel(bx + 1, by).grayscale * strength;
                    yUp = _Diffuse.GetPixel(bx, by - 1).grayscale * strength;
                    yDown = _Diffuse.GetPixel(bx, by + 1).grayscale * strength;
                    xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    yDelta = ((yUp - yDown) + 1) * 0.5f;
                    _NormalMap.SetPixel(bx, by, new Color(xDelta, yDelta, 1.0f, yDelta));
                }
            }
            _NormalMap.Apply();
        }
    }
}