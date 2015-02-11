using UnityEngine;
using System.Collections;
using UnityEditor;
using Skill.Framework.UI;

namespace Skill.Editor.UI
{
    public class AudioClipCurveData
    {
        public const float MinRangeTime = 0.001f;

        private int _Resolution;
        private AudioClip _Clip;

        private float _StartTime;
        private float _EndTime;
        private float[] _Samples;

        private AnimationCurve _MinCurve;
        private AnimationCurve _MaxCurve;

        public int Resolution { get { return _Resolution; } }
        public AudioClip Clip { get { return _Clip; } }
        public float StartTime { get { return _StartTime; } }
        public float EndTime { get { return _EndTime; } }
        public AnimationCurve MinCurve { get { return _MinCurve; } }
        public AnimationCurve MaxCurve { get { return _MaxCurve; } }
        public float[] Samples { get { return _Samples; } }

        public void Build(AudioClip clip, int resolution, float startTime, float endTime)
        {
            if (_Clip != clip)
            {
                _Clip = clip;
                _Samples = null;
            }
            _Resolution = Mathf.Max(resolution, 16);
            _StartTime = startTime;
            _EndTime = endTime;
            _StartTime = Mathf.Max(0, _StartTime);
            if (_Clip != null)
                _EndTime = Mathf.Min(_Clip.length, _EndTime);

            if (_StartTime > _EndTime - MinRangeTime)
            {
                _StartTime = _EndTime - MinRangeTime;
                if (_StartTime < 0)
                {
                    _StartTime = 0;
                    _EndTime = MinRangeTime;
                }
            }

            _MinCurve = null;
            _MaxCurve = null;

            if (_Samples == null)
            {
                _Samples = new float[_Clip.samples * _Clip.channels];

                // it worked in unity 3.5f but in 4 not working !!! figure it later

                //string path = AssetDatabase.GetAssetPath(_Clip);
                ///AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
                                
                //workaround to prevent the error in the function getData
                //when Audio Importer loadType is "compressed in memory"
                //if (audioImporter.loadType != AudioImporterLoadType.StreamFromDisc)
                //{
                //    AudioImporterLoadType audioLoadTypeBackup = audioImporter.loadType;
                //    audioImporter.loadType = AudioImporterLoadType.StreamFromDisc;
                //    AssetDatabase.ImportAsset(path);

                //    //getData after the loadType changed
                //    _Clip.GetData(_Samples, 0);

                //    //restore the loadType (end of workaround)
                //    audioImporter.loadType = audioLoadTypeBackup;
                //    AssetDatabase.ImportAsset(path);
                //}
                //else
                //{
                //    _Clip.GetData(_Samples, 0);
                //}

            }


            float[] _MinBarHeights = new float[_Resolution];
            float[] _MaxBarHeights = new float[_Resolution];

            for (int i = 0; i < _Resolution; i++) _MinBarHeights[i] = _MaxBarHeights[i] = -1;

            int startIndex = (int)Mathf.Max(0, (float)(_Samples.Length - 1) * (_StartTime / _Clip.length));
            int endIndex = (int)Mathf.Min(_Samples.Length - 1, (float)(_Samples.Length - 1) * (_EndTime / _Clip.length));

            int size = endIndex - startIndex;
            int index = 0;
            float step = (float)size / _Resolution;
            float stepCounter = 0;
            int i2 = 0;
            int speed = Mathf.Max(1, size / 100000);

            while (index < _Resolution && i2 <= endIndex)
            {
                float barHeight = (_Samples[startIndex + i2] + 1f) * 0.5f;
                if (_MinBarHeights[index] < 0)
                {
                    _MinBarHeights[index] = _MaxBarHeights[index] = barHeight;
                }
                else
                {
                    _MinBarHeights[index] = Mathf.Min(_MinBarHeights[index], barHeight);
                    _MaxBarHeights[index] = Mathf.Max(_MaxBarHeights[index], barHeight);
                }

                i2 += speed;
                stepCounter += speed;
                while (stepCounter > step)
                {
                    stepCounter -= step;
                    index++;
                }
            }


            _MinCurve = new AnimationCurve();
            _MaxCurve = new AnimationCurve();
            for (int i = 0; i < _Resolution; i++)
            {
                if (_MinBarHeights[i] >= 0)
                {
                    Keyframe key = new Keyframe();
                    key.time = i;
                    key.value = _MinBarHeights[i];
                    _MinCurve.AddKey(key);
                }
                if (_MaxBarHeights[i] >= 0)
                {
                    Keyframe key = new Keyframe();
                    key.time = i;
                    key.value = _MaxBarHeights[i];
                    _MaxCurve.AddKey(key);
                }
            }
            for (int i = 0; i < _MinCurve.length; i++) _MinCurve.SmoothTangents(i, 0.5f);
            for (int i = 0; i < _MaxCurve.length; i++) _MaxCurve.SmoothTangents(i, 0.5f);

        }

        public void Reset()
        {
            _Clip = null;
            _Samples = null;
            _MinCurve = null;
            _MaxCurve = null;
        }
    }
}
