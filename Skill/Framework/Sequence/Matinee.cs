using UnityEngine;
using System.Collections;
using System;
using Skill.Framework.Sequence;

namespace Skill.Framework.Sequence
{


    /// <summary>
    /// Matinee is a tool for keyframing the properties of Actors in your scene over time, including their position.
    /// It can also be used to author cinematic sequences in your level.
    /// </summary>
    public class Matinee : Skill.Framework.DynamicBehaviour
    {
        #region Editor data
        [HideInInspector]
        [SerializeField]
        public float[] EditorData = null;
        #endregion

        /// <summary> End time of matinee to play</summary>
        public float EndTime = 5.0f;
        /// <summary> Is loop? </summary>
        public bool Loop = false;
        /// <summary> Playback speed </summary>
        public float Speed = 1.0f;
        /// <summary> Is game enters cutscene mode? </summary>
        public bool Cutscene = true;


        /// <summary> Is Matinee playing? </summary>
        public bool IsPlaying { get; private set; }
        /// <summary> Is Matinee paused? </summary>
        public bool IsPaused { get; private set; }

        private Track[] _Tracks;
        private float _PlayTime;
        private float _LastTime;


        /// <summary>
        /// Occurs when matinee play/pause or stoppped.
        /// </summary>
        public event EventHandler PlaybackChanged;
        protected virtual void OnPlaybackChanged()
        {
            if (PlaybackChanged != null) PlaybackChanged(this, EventArgs.Empty);
        }

        /// <summary> Awake </summary>
        protected override void Awake()
        {
            base.Awake();
            enabled = false;
            _Tracks = GetComponentsInChildren<Track>();

            _PlayTime = 0;
            IsPaused = false;
            IsPlaying = false;
            _NextStop = false;
        }

        /// <summary>
        /// Seek to specific time
        /// </summary>
        /// <param name="seekTime">time to seek (0 - Length)</param>
        public void Seek(float seekTime)
        {
            seekTime = Mathf.Clamp(seekTime, 0, EndTime);
            if (_Tracks != null)
            {
                foreach (var t in _Tracks)
                    t.Seek(seekTime);
            }
            _PlayTime = seekTime;
        }


        /// <summary> Stop playing Matinee </summary>
        /// <param name="rollBack">RollBack to start frame?</param>
        public void Stop(bool rollBack = false)
        {
            if (IsPlaying)
            {
                if (rollBack)
                    foreach (var t in _Tracks) t.Rollback();

                IsPlaying = false;
                IsPaused = false;
                if (Cutscene)
                    Skill.Framework.Global.CutSceneEnable = false;
                enabled = false;
                _NextStop = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Start playing matinee from current(seek) time </summary>
        public void Play()
        {
            if (!IsPlaying)
            {
                _LastTime = Time.realtimeSinceStartup;
                Skill.Framework.Global.CutSceneEnable = Cutscene;
                IsPlaying = true;
                IsPaused = false;
                enabled = true;
                _NextStop = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Pause playback of matinee </summary>
        public void Pause()
        {
            if (IsPlaying && !IsPaused)
            {
                IsPaused = true;
                enabled = false;
                _NextStop = false;
                OnPlaybackChanged();
            }
        }

        /// <summary> Update </summary>
        protected override void Update()
        {
            if (!Skill.Framework.Global.IsGamePaused)
            {
                if (IsPlaying && !IsPaused)
                {
                    float deltaTime = Time.realtimeSinceStartup - _LastTime;
                    StepForward(deltaTime);
                }
                base.Update();
            }
            _LastTime = Time.realtimeSinceStartup;
        }

        private bool _NextStop;
        // step forward playback with stepTime
        private void StepForward(float stepTime)
        {
            _PlayTime += stepTime * Speed;

            if (_NextStop)
                Stop();

            if (_PlayTime >= EndTime)
            {
                if (Loop)
                {
                    _PlayTime -= EndTime;
                    _NextStop = false;
                    foreach (var t in _Tracks) t.Seek(_PlayTime);

                }
                else
                {
                    // let last keys evaluate
                    _NextStop = true;
                }
            }
            else if (_PlayTime <= 0)
            {
                if (Loop)
                {
                    _PlayTime += EndTime;
                    _NextStop = false;
                    foreach (var t in _Tracks) t.Seek(_PlayTime);
                }
                else
                {
                    // let last keys evaluate
                    _NextStop = true;
                }
            }

            if (IsPlaying)
            {
                foreach (var t in _Tracks) t.Evaluate(_PlayTime);
            }
        }

        public void GetTimeBounds(out float minTime, out float maxTime)
        {
            minTime = 0;
            maxTime = 1.0f;

            Track[] tracks = GetComponentsInChildren<Track>();
            if (tracks.Length > 0)
            {
                minTime = float.MaxValue;
                maxTime = float.MinValue;

                foreach (var t in tracks)
                {
                    float tminTime, tmaxTime;
                    t.GetTimeBounds(out tminTime, out tmaxTime);

                    minTime = Mathf.Min(minTime, tminTime);
                    maxTime = Mathf.Max(maxTime, tmaxTime);
                }
            }
        }
    }

}