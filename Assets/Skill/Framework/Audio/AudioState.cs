using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework.Audio
{
    [System.Serializable]
    public class AudioState
    {

        internal const float MINFADETIME = 0.001f;

        [SerializeField]
        [HideInInspector]
        public string Name;

        [SerializeField]
        [HideInInspector]
        public string NextState;

        [SerializeField]
        [HideInInspector]
        public AudioClip Clip;

        [SerializeField]
        [HideInInspector]
        public Skill.Framework.Audio.SoundCategory Category = Skill.Framework.Audio.SoundCategory.Music;

        [SerializeField]
        [HideInInspector]
        public AudioTransition[] Transitions;

        [SerializeField]
        [HideInInspector]
        public float[] BreakPoints;

        [SerializeField]
        [HideInInspector]
        public float VolumeFactor = 1.0f;

        [SerializeField]
        [HideInInspector]
        public float Begin = 0.0f;

        [SerializeField]
        [HideInInspector]
        public float End = 0.0f;

        [SerializeField]
        [HideInInspector]
        public float X = 0.0f;

        [SerializeField]
        [HideInInspector]
        public float Y = 0.0f;

        [SerializeField]
        [HideInInspector]
        public bool SaveTime = false;

        private AudioSource _Audio;
        private float _Length;
        private float _FadeOutTime;
        private float _FadeInTime;
        private float _FadeEndTime;
        private float _SavedTime;

        public void Validate(AudioSource audio)
        {
            _Audio = audio;
            if (Clip != null)
            {
                if (Begin < 0) Begin = 0;
                if (End > Clip.length) End = Clip.length;
                if (Begin > End) Begin = End;

                if (BreakPoints != null)
                {
                    for (int i = 0; i < BreakPoints.Length; i++)
                        BreakPoints[i] = Mathf.Clamp(BreakPoints[i], 0, Clip.length);

                    SortBreakPoints();
                }
            }
            else
            {
                Begin = 0;
                End = 0;
            }

            _Length = End - Begin;
            _FadeOutTime = 0;
            _FadeInTime = 0;
        }

        public void SortBreakPoints()
        {
            if (BreakPoints != null && BreakPoints.Length > 1)
                Skill.Framework.Utility.QuickSort(BreakPoints, FloatComparer.Instance);
        }


        private float GetVolume()
        {
            if (Skill.Framework.Global.Instance != null)
                return Mathf.Clamp01(VolumeFactor * Skill.Framework.Global.Instance.Settings.Audio.GetVolume(Category));
            else
                return VolumeFactor;
        }

        public bool IsPlaying
        {
            get
            {
                if (_Audio != null)
                    return _Audio.clip != null && _Audio.isPlaying;
                return false;
            }
        }

        public float AudioTime
        {
            get
            {
                if (_Audio != null)
                    return _Audio.time;
                return 0;
            }
        }

        public void Stop()
        {
            if (_Audio != null)
            {
                if (IsPlaying)
                    _SavedTime = _Audio.time;
                _Audio.Stop();
            }
        }

        public void EndTime(float deltaTime, float fadeOut)
        {
            if (_Audio != null)
            {
                _Audio.SetScheduledEndTime(AudioSettings.dspTime + deltaTime);

                if (fadeOut > MINFADETIME && IsPlaying)
                {
                    _FadeOutTime = Mathf.Max(0, Mathf.Min(deltaTime, fadeOut));
                    _FadeEndTime = _Audio.time + deltaTime - Begin;
                    _SavedTime = _Audio.time + deltaTime;
                }
                else
                {
                    _FadeOutTime = 0;
                    _FadeEndTime = _Length;
                }
            }
        }

        public void StartTime(float deltaTime, float fadeIn)
        {
            if (_Audio != null && Clip != null)
            {
                _Audio.PlayScheduled(AudioSettings.dspTime + deltaTime);

                if (fadeIn > MINFADETIME)
                {
                    _FadeInTime = Mathf.Max(0, Mathf.Min(_Length, fadeIn));
                }
                else
                {
                    _FadeInTime = 0;
                }
            }
        }

        public void Initialize(bool fadeIn)
        {
            if (_Audio != null)
            {
                _Audio.clip = Clip;
                _Audio.loop = false;

                if (SaveTime)
                {
                    if (_SavedTime > End) _SavedTime = Begin;
                    else if (_SavedTime < Begin) _SavedTime = Begin;
                }
                else
                {
                    _SavedTime = Begin;
                }
                _Audio.time = _SavedTime;

                if (fadeIn)
                    _Audio.volume = 0;
                else
                    _Audio.volume = GetVolume();
            }
            _Length = End - Begin;
            _FadeOutTime = 0;
            _FadeInTime = 0;
            _FadeEndTime = _Length;
        }
        public void Update(float volumeFactor)
        {
            if (_Audio != null && _Audio.isPlaying)
            {
                float vol = GetVolume() * volumeFactor;
                float time = _Audio.time - Begin;
                if (_FadeInTime > MINFADETIME && time < _FadeInTime)
                    _Audio.volume = Mathf.Lerp(0, vol, time / _FadeInTime);
                else if (_FadeOutTime > MINFADETIME && time > (_FadeEndTime - _FadeOutTime))
                    _Audio.volume = Mathf.Lerp(0, vol, (_FadeEndTime - time) / _FadeOutTime);
                else
                    _Audio.volume = vol;
            }
        }

        class FloatComparer : IComparer<float>
        {
            private static FloatComparer _Instance;
            public static FloatComparer Instance
            {
                get
                {
                    if (_Instance == null) _Instance = new FloatComparer();
                    return _Instance;
                }
            }

            private FloatComparer()
            {

            }
            public int Compare(float x, float y)
            {
                return x.CompareTo(y);
            }
        }

    }


    [System.Serializable]
    public class AudioTransition
    {
        /// <summary> Destination AudioState </summary>
        [SerializeField]
        public string Destination;

        /// <summary> Name of trigger </summary>
        [SerializeField]
        public string Trigger = string.Empty;

        [SerializeField]
        public float FadeIn = 0;

        [SerializeField]
        public float FadeOut = 0;

        [SerializeField]
        public bool WaitForBreakPoint = true;

        [SerializeField]
        public bool CrossFade = false;

        public AudioController.Trigger TriggerRef { get; internal set; }

    }
}