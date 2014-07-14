using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Sound key data
    /// </summary>
    [System.Serializable]
    public class SoundKey : IPropertyKey<AudioClip>, ITrackKey
    {
        /// <summary> time to set </summary>
        public float Time = 0;
        /// <summary> Clip to play </summary>
        public AudioClip Clip;
        /// <summary> Volume factor </summary>   
        public float VolumeFactor = 1.0f;


        [ExposeProperty(0, "Time", "play time")]
        public float ExTime { get { return Time; } set { Time = value; } }
        [ExposeProperty(2, "Volume Factor")]
        public float ExVolumeFactor { get { return VolumeFactor; } set { VolumeFactor = value; } }
        [ExposeProperty(1, "Audio clip")]
        public AudioClip ExClip { get { return Clip; } set { Clip = value; } }


        /// <summary> Execution Time </summary>
        public float FireTime { get { return Time; } set { Time = value; if (Time < 0) Time = 0; } }

        public AudioClip ValueKey { get { return Clip; } set { Clip = value; } }
    }

    public class SoundTrack : Track
    {
        [HideInInspector]
        [SerializeField]
        public SoundKey[] Keys;

        [HideInInspector]
        [SerializeField]
        public AudioSource Source;

        [HideInInspector]
        [SerializeField]
        public Skill.Framework.Audio.SoundCategory Category = Skill.Framework.Audio.SoundCategory.Cinematic;

        [HideInInspector]
        [SerializeField]
        public bool PlayOneShot = false;


        [ExposeProperty(0, "Source", "Source AudioSource")]
        public AudioSource ExSource { get { return Source; } set { Source = value; } }

        [ExposeProperty(1, "Category", "Sound Category")]
        public Skill.Framework.Audio.SoundCategory ExCategory { get { return Category; } set { Category = value; } }

        [ExposeProperty(2, "PlayOneShot", "user PlayOneShot method")]
        public bool ExPlayOneShot { get { return PlayOneShot; } set { PlayOneShot = value; } }

        public override TrackType Type { get { return TrackType.Sound; } }        

        public override float Length
        {
            get
            {
                if (Keys != null && Keys.Length > 0)
                {
                    if (!Application.isPlaying)
                        SortKeys();

                    float length = Keys[Keys.Length - 1].FireTime;
                    if (Keys[Keys.Length - 1].Clip != null)
                        length += Keys[Keys.Length - 1].Clip.length;
                    return length;
                }
                return 0;
            }
        }


        private void Execute(SoundKey key)
        {
            SoundKey sKey = (SoundKey)key;
            if (Source != null && sKey.ValueKey != null)
            {
                float volume = 1.0f;
                if (Skill.Framework.Global.Instance != null)
                    volume = Skill.Framework.Global.Instance.Settings.Audio.GetVolume(Category);
                if (PlayOneShot)
                {
                    Source.PlayOneShot(sKey.ValueKey, volume * sKey.VolumeFactor);
                }
                else
                {
                    if (Source.isPlaying) Source.Stop();
                    Source.clip = sKey.Clip;
                    Source.Play();
                }
                if (Audio.AudioSubtitle.Instance != null)
                    Audio.AudioSubtitle.Instance.Show(sKey.ValueKey);
            }
        }

        public override void Stop()
        {
            if (Source != null)
                Source.Stop();
            Source.clip = null;
        }

        protected float CurrecntTime { get; private set; }

        private int _Index;
        public override void Evaluate(float time)
        {
            int evaluatedIndex = -1;
            float preTime = CurrecntTime;
            CurrecntTime = time;
            float deltaTime = CurrecntTime - preTime;
            if (deltaTime > 0)
            {
                if (Keys != null)
                {
                    if (_Index < 0) _Index = 0;
                    while (_Index < Keys.Length)
                    {
                        float t = Keys[_Index].FireTime;
                        if (t <= CurrecntTime)
                        {
                            evaluatedIndex = _Index;
                            if (t < CurrecntTime && t >= preTime)
                            {
                                Execute(Keys[_Index]);
                            }
                            _Index++;
                        }
                        else
                        {
                            _Index--;
                            break;
                        }
                    }
                }
            }
            Evaluate(evaluatedIndex);
        }

        private void Evaluate(int evaluatedIndex)
        {
            if (_Index < 0)
                Rollback();
            else
            {
                if (_Index >= Keys.Length)
                    _Index = Keys.Length - 1;
                if (evaluatedIndex != _Index && _Index >= 0 && _Index < Keys.Length)
                {
                    Execute(Keys[_Index]);
                }
            }
        }
        
        public override void Seek(float time)
        {
            CurrecntTime = time;
            if (Keys != null && Keys.Length > 0)
                _Index = FindMaxIndexBeforeTime(Keys, time);
            else
                _Index = -1;
            Evaluate(-1);
        }

        public override void SortKeys()
        {
            if (Keys != null && Keys.Length > 1)
                Skill.Framework.MathHelper.QuickSort(Keys, new TrackKeyComparer<SoundKey>());
        }
    }
}