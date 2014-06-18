using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor.UI.Extended
{
    public class AudioPreviewCurve : Skill.Editor.UI.EditorControl
    {
        private int _Resolution;
        private AudioClip _Clip;
        private float _StartTime;
        private float _EndTime;
        private bool _Changed;
        private AudioClipCurveData _Data;

        public int Resolution
        {
            get { return _Resolution; }
            set
            {
                if (_Resolution != value)
                {
                    _Resolution = Mathf.Max(16, value);
                    _Ranges = new Rect(0, 0, _Resolution, 1);
                    _Changed = true;
                }
            }
        }

        private Rect _Ranges;
        public Rect Ranges { get { return _Ranges; } set { _Ranges = value; } }

        public AudioClip Clip
        {
            get { return _Clip; }
            set
            {
                if (_Clip != value)
                {
                    _Clip = value;
                    _Changed = true;
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
        public Color BackgroundColor { get; set; }
        public Color Color { get; set; }

        public bool PreviewBackground { get; set; }

        public AudioPreviewCurve()
        {
            PreviewBackground = true;
            _Data = new AudioClipCurveData();
            Resolution = 800;
            BackgroundColor = new Color(0, 0, 0, 0);
            Color = new Color(0.3f, 0.3f, 1.0f, 1.0f);
        }

        protected override void Render()
        {
            if (_Changed)
                UpdatePreview();

            if (PreviewBackground)
                GUI.Box(RenderArea, string.Empty, Skill.Editor.Resources.Styles.PreviewBackground);
            if (_Data.MinCurve != null)
            {                
                Rect ra = RenderArea;
                EditorGUIUtility.DrawRegionSwatch(ra, _Data.MinCurve, _Data.MaxCurve, Color, BackgroundColor, Ranges);
            }
        }

        public void SetTime(float startTime, float endTime)
        {
            if (_StartTime != startTime || _EndTime != endTime)
            {
                _StartTime = startTime;
                _EndTime = endTime;
                _Changed = true;
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
            }
            else
            {
                ValidateTime();
                _Data.Build(_Clip, _Resolution, _StartTime, _EndTime);
            }
            _Changed = false;

        }
    }
}
