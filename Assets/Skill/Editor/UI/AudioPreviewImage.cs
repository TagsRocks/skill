using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.UI;


namespace Skill.Editor.UI
{
    public class AudioPreviewImage : Skill.Framework.UI.Image
    {

        private int _TextureWidth;
        private int _TextureHeight;
        private AudioClip _Clip;
        private Texture2D _PreviewTexture;

        private float _StartTime;
        private float _EndTime;
        private Color[] _BackgroundColors;
        private Color _BackgroundColor;
        private Color _MiddleColor;
        private Color _SideColor;
        private int _HalfHeight;
        private bool _Changed;

        private AudioClipCurveData _Data;

        public bool PreviewBackground { get; set; }

        public int TextureWidth
        {
            get { return _TextureWidth; }
            set
            {
                if (_TextureWidth != value)
                {
                    _TextureWidth = Mathf.Max(16, value);
                    DestroyPreviewTexture();
                    _Changed = true;
                }
            }
        }
        public int TextureHeight
        {
            get { return _TextureHeight; }
            set
            {
                if (_TextureHeight != value)
                {
                    _TextureHeight = Mathf.Max(16, value);
                    DestroyPreviewTexture();
                    _Changed = true;
                }
            }
        }
        public AudioClip Clip
        {
            get { return _Clip; }
            set
            {
                if (_Clip != value)
                {
                    _Clip = value;
                    _Changed = true;
                    DestroyPreviewTexture();
                    if (_Clip != null)
                    {
                        _StartTime = 0;
                        _EndTime = _Clip.length;
                    }
                }
            }
        }

        public float StartTime { get { return _StartTime; } }
        public float EndTime { get { return _EndTime; } }
        public Color BackgroundColor
        {
            get { return _BackgroundColor; }
            set
            {
                if (_BackgroundColor != value)
                {
                    _Changed = true;
                    _BackgroundColor = value;
                    DestroyPreviewTexture();
                }
            }
        }
        public Color MiddleColor
        {
            get { return _MiddleColor; }
            set
            {
                if (_MiddleColor != value)
                {
                    _Changed = true;
                    _MiddleColor = value;
                    DestroyPreviewTexture();
                }
            }
        }
        public Color SideColor
        {
            get { return _SideColor; }
            set
            {
                if (_SideColor != value)
                {
                    _Changed = true;
                    _SideColor = value;
                    DestroyPreviewTexture();
                }
            }
        }

        private void DestroyPreviewTexture()
        {
            if (_PreviewTexture != null)
                GameObject.DestroyImmediate(_PreviewTexture);
            _PreviewTexture = null;
            _BackgroundColors = null;
        }



        public AudioPreviewImage()
        {
            _Data = new AudioClipCurveData();
            TextureWidth = 1024;
            TextureHeight = 128;
            BackgroundColor = new Color(0, 0, 0, 0);
            _MiddleColor = new Color(1.0f, 0.61f, 0.0f, 1.0f);
            _SideColor = new Color(1.0f, 0.82f, 0.0f, 1.0f);
            DestroyPreviewTexture();
            PreviewBackground = true;
        }

        protected override void Render()
        {
            if (_Changed)
                UpdatePreview();
            base.Texture = _PreviewTexture;
            if (PreviewBackground)
                GUI.Box(RenderArea, string.Empty, Skill.Editor.Resources.Styles.PreviewBackground);
            base.Render();
        }

        public void SetTime(float startTime, float endTime)
        {
            if (_StartTime != startTime || _EndTime != endTime)
            {
                _StartTime = startTime;
                _EndTime = endTime;

                _Changed = true;
                ValidateTime();
            }
        }

        private void ValidateTime()
        {
            _StartTime = Mathf.Max(0, _StartTime);
            if (_Clip != null)
                _EndTime = Mathf.Min(_Clip.length, _EndTime);

            if (_StartTime > _EndTime - AudioClipCurveData.MinRangeTime)
            {
                _StartTime = _EndTime - AudioClipCurveData.MinRangeTime;
                if (_StartTime < 0)
                {
                    _StartTime = 0;
                    _EndTime = AudioClipCurveData.MinRangeTime;
                }
            }
        }
        private void UpdatePreview()
        {
            if (_Clip == null)
            {
                _Data.Reset();
                DestroyPreviewTexture();
            }
            else
            {
                ValidateTime();
                if (_PreviewTexture == null)
                    _PreviewTexture = new Texture2D(_TextureWidth, _TextureHeight, TextureFormat.RGBA32, false);
                _Data.Build(_Clip, _TextureWidth, _StartTime, _EndTime);

                if (_BackgroundColors == null)
                {
                    _BackgroundColors = new Color[_TextureWidth * _TextureHeight];
                    for (int ci = 0; ci < _BackgroundColors.Length; ci++)
                        _BackgroundColors[ci] = _BackgroundColor;
                }
                _PreviewTexture.SetPixels(_BackgroundColors);


                _HalfHeight = _TextureHeight / 2;
                int preY = _HalfHeight;
                for (int i = 0; i < _TextureWidth; i++)
                {
                    int min = (int)(_Data.MinCurve.Evaluate(i) * _TextureHeight);
                    int max = (int)(_Data.MaxCurve.Evaluate(i) * _TextureHeight);

                    for (int j = min; j <= max; j++)
                        _PreviewTexture.SetPixel(i, j, GetColor(j));
                    if (preY < min)
                    {
                        for (int j = preY; j <= min; j++)
                            _PreviewTexture.SetPixel(i, j, GetColor(j));
                        preY = max;
                    }
                    else if (preY > max)
                    {
                        for (int j = max; j <= preY; j++)
                            _PreviewTexture.SetPixel(i, j, GetColor(j));
                        preY = min;
                    }
                }
                _PreviewTexture.Apply();
            }
            _Changed = false;

        }
        private Color GetColor(int y)
        {
            return Color.Lerp(_MiddleColor, _SideColor, (float)Mathf.Abs(_HalfHeight - y) / _HalfHeight);
        }


        public static Texture2D CreateAudioWaveform(AudioClip aud, int width, int height, Color color)
        {
            width = Mathf.Max(width, 16);
            height = Mathf.Max(height, 16);

            float[] samples = new float[aud.samples * aud.channels];

            string path = AssetDatabase.GetAssetPath(aud);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;

            //workaround to prevent the error in the function getData
            //when Audio Importer loadType is "compressed in memory"
            if (audioImporter.loadType != AudioImporterLoadType.StreamFromDisc)
            {
                AudioImporterLoadType audioLoadTypeBackup = audioImporter.loadType;
                audioImporter.loadType = AudioImporterLoadType.StreamFromDisc;
                AssetDatabase.ImportAsset(path);

                //getData after the loadType changed
                aud.GetData(samples, 0);

                //restore the loadType (end of workaround)
                audioImporter.loadType = audioLoadTypeBackup;
                AssetDatabase.ImportAsset(path);
            }
            else
            {
                //getData after the loadType changed
                aud.GetData(samples, 0);
            }

            Texture2D img = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color[] xy = new Color[width * height];
            img.SetPixels(xy);

            for (var i = 0; i < samples.Length; i++)
            {
                img.SetPixel((int)((width * i) / samples.Length), (int)(height * (samples[i] + 1f) / 2), color);
            }

            img.Apply();


            return img;
        }
    }
}
